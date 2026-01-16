from sklearn.metrics.pairwise import cosine_similarity
from sentence_transformers import SentenceTransformer
from AI.models.student_input import StudentInput
import pandas as pd
import numpy as np
import json
import ast
import hashlib
import random
import re

import torch

def get_top_5_prediction(
    data: StudentInput,
    metadata_path: str = "data/cleaned/cleaned_dataset_soft-NLP.csv",
):
    combined_student_input = combine_student_input(data)
    vectorized_student_input = vectorize_student_input(combined_student_input)
    top_5 = filter_matches_top_5(vectorized_student_input, data, metadata_path=metadata_path)
    return add_motivation(vectorized_student_input, top_5, data, metadata_path=metadata_path)
    

def combine_student_input(data: StudentInput):
    big_string = ". ".join([
        data.current_study,
        ". ".join(data.interests),
        ". ".join(data.learning_goals),
    ])
    return big_string.strip()

def vectorize_student_input(input: str):
    model = SentenceTransformer("paraphrase-multilingual-MiniLM-L12-v2")
    device = 'cuda' if torch.cuda.is_available() else 'cpu'
    vectorized_student_input = model.encode(input, show_progress_bar=True, device=device)
    return vectorized_student_input

def ordered_module_matches(vectorized_student_input):
    # Loading previously saved sentence embedded dataframe
    embedded_modules = pd.read_pickle('data/processed/sentence_embedded_dataframe.pkl')
    
    # Converting stringified numpy arrays back to actual numpy arrays
    embedded_modules["sentence_embedding_vector"] = embedded_modules["sentence_embedding_vector"].apply(
        lambda x: (
            np.asarray(x, dtype=float)
            if isinstance(x, (list, np.ndarray))
            else (
                np.asarray(ast.literal_eval(x), dtype=float)
                if isinstance(x, str) and x.strip().startswith("[") and "," in x
                else (np.fromstring(x.strip("[]"), sep=" ", dtype=float) if isinstance(x, str) else np.asarray(x, dtype=float))
            )
        )
    )

    # Stacking individual vectors into a 2D matrix
    module_matrix = np.stack(embedded_modules["sentence_embedding_vector"].values)

    scores = cosine_similarity(np.asarray(vectorized_student_input).reshape(1, -1), module_matrix)[0]

    # Adding score to dataframe
    embedded_modules['similarity_score'] = scores

    # Dropping vector column before returning
    embedded_modules = embedded_modules.drop(columns=['sentence_embedding_vector'])

    ordered_matches = embedded_modules.sort_values(by='similarity_score', ascending=False)
    return json.loads(ordered_matches.to_json(orient="records"))

def filter_matches_top_5(
    vectorized_student_input,
    data: StudentInput,
    metadata_path: str = "data/cleaned/cleaned_dataset_soft-NLP.csv",
):
    metadata_df = pd.read_csv(metadata_path)

    # Ensure join keys have compatible dtypes
    metadata_df["id"] = metadata_df["id"].astype(int)

    # Apply level filter
    if getattr(data, "level_preference", None) and "level" in metadata_df.columns:
        metadata_df = metadata_df[metadata_df["level"].isin(data.level_preference)]

    # Apply study credit range filter
    min_credits, max_credits = data.wanted_study_credit_range
    if "studycredit" in metadata_df.columns:
        metadata_df = metadata_df[(metadata_df["studycredit"] >= min_credits) & (metadata_df["studycredit"] <= max_credits)]

    # Apply location filter (metadata stores a string like "['Den Bosch']" in cleaned CSV)
    if getattr(data, "location_preference", None) and "location" in metadata_df.columns:
        preferred_locations = set(data.location_preference)

        def normalize_locations(value):
            if isinstance(value, list):
                return value
            if isinstance(value, str):
                s = value.strip()
                if s.startswith("[") and s.endswith("]"):
                    try:
                        parsed = ast.literal_eval(s)
                        if isinstance(parsed, list):
                            return parsed
                    except (ValueError, SyntaxError):
                        pass
                return [s]
            if pd.isna(value):
                return []
            return [str(value)]

        metadata_df["_locations"] = metadata_df["location"].apply(normalize_locations)
        metadata_df = metadata_df[metadata_df["_locations"].apply(lambda locs: any(loc in preferred_locations for loc in locs))]
        metadata_df = metadata_df.drop(columns=["_locations"])

    # Apply preferred language filter only if the metadata contains a language column.
    # If your datasets don't have this column yet, this filter is skipped.
    preferred_language = getattr(data, "preferred_language", None)
    if preferred_language and preferred_language != "Niet van toepassing":
        for candidate_col in ("language", "taal", "preferred_language"):
            if candidate_col in metadata_df.columns:
                metadata_df = metadata_df[metadata_df[candidate_col] == preferred_language]
                break

    # Apply preferred period filter.
    # If the metadata contains a dedicated period column, use it.
    # Otherwise, derive the period from the start_date column.
    preferred_periods = getattr(data, "preferred_period", None)
    if preferred_periods:
        preferred_periods = set(preferred_periods)

        # 1) Direct period column in dataset
        for candidate_col in ("period", "periode", "preferred_period"):
            if candidate_col in metadata_df.columns:
                metadata_df = metadata_df[metadata_df[candidate_col].isin(preferred_periods)]
                break
        else:
            # 2) Derive from start_date
            if "start_date" in metadata_df.columns:
                def date_to_period(dt: pd.Timestamp | None):
                    if dt is None or pd.isna(dt):
                        return None
                    month = int(dt.month)
                    # Dutch school year quarters (school year starts in September)
                    # P1: Sep–Nov, P2: Dec–Feb, P3: Mar–May, P4: Jun–Aug
                    if month in (9, 10, 11):
                        return "P1"
                    if month in (12, 1, 2):
                        return "P2"
                    if month in (3, 4, 5):
                        return "P3"
                    if month in (6, 7, 8):
                        return "P4"
                    return None

                start_dates = pd.to_datetime(metadata_df["start_date"], errors="coerce")
                metadata_df["_period"] = start_dates.apply(date_to_period)
                metadata_df = metadata_df[metadata_df["_period"].isin(preferred_periods)]
                metadata_df = metadata_df.drop(columns=["_period"])

    candidate_ids = set(metadata_df["id"].tolist())
    if not candidate_ids:
        return []

    embedded_modules = pd.read_pickle('data/processed/sentence_embedded_dataframe.pkl')
    embedded_modules["id"] = embedded_modules["id"].astype(int)
    embedded_modules = embedded_modules[embedded_modules["id"].isin(candidate_ids)]
    if embedded_modules.empty:
        return []

    # Converting stringified numpy arrays back to actual numpy arrays
    embedded_modules["sentence_embedding_vector"] = embedded_modules["sentence_embedding_vector"].apply(
        lambda x: (
            np.asarray(x, dtype=float)
            if isinstance(x, (list, np.ndarray))
            else (
                np.asarray(ast.literal_eval(x), dtype=float)
                if isinstance(x, str) and x.strip().startswith("[") and "," in x
                else (np.fromstring(x.strip("[]"), sep=" ", dtype=float) if isinstance(x, str) else np.asarray(x, dtype=float))
            )
        )
    )

    module_matrix = np.stack(embedded_modules["sentence_embedding_vector"].values)
    scores = cosine_similarity(np.asarray(vectorized_student_input).reshape(1, -1), module_matrix)[0]

    result = pd.DataFrame({
        "id": embedded_modules["id"].values,
        "similarity_score": scores,
    }).sort_values(by="similarity_score", ascending=False).head(5)

    return json.loads(result.to_json(orient="records"))

def add_motivation(
    vectorized_student_input,
    top_5_modules: list,
    data: StudentInput,
    metadata_path: str = "data/cleaned/cleaned_dataset_soft-NLP.csv",
):
    """
    Adds motivation snippets to each recommended module by finding which parts 
    of the student input best match the module description.
    """
    if not top_5_modules:
        return []
    
    # Load metadata to get module descriptions
    metadata_df = pd.read_csv(metadata_path)
    metadata_df["id"] = metadata_df["id"].astype(int)
    
    # Load sentence transformer model
    model = SentenceTransformer("paraphrase-multilingual-MiniLM-L12-v2")
    device = 'cuda' if torch.cuda.is_available() else 'cpu'
    
    # Split student input into meaningful chunks
    student_chunks = []
    
    # Add current study as a chunk
    if data.current_study and data.current_study.strip():
        student_chunks.append({
            "text": data.current_study,
            "category": "studie"
        })
    
    # Add each interest as a separate chunk
    for interest in data.interests:
        if interest and interest.strip():
            student_chunks.append({
                "text": interest,
                "category": "interesse"
            })
    
    # Add each learning goal as a separate chunk
    for goal in data.learning_goals:
        if goal and goal.strip():
            student_chunks.append({
                "text": goal,
                "category": "leerdoel"
            })
    
    # Vectorize all student chunks
    if student_chunks:
        chunk_texts = [chunk["text"] for chunk in student_chunks]
        chunk_vectors = model.encode(chunk_texts, show_progress_bar=False, device=device)
        
        for i, chunk in enumerate(student_chunks):
            chunk["vector"] = chunk_vectors[i]
    
    # Template sentences for each category
    templates = {
        "interesse": [
            "Deze module sluit goed aan bij jouw interesse in: {}",
            "Deze module past perfect bij jouw interesse: {}",
            "Op basis van jouw interesse in {} is deze module een goede match",
            "Deze module komt overeen met jouw interesse: {}"
        ],
        "leerdoel": [
            "Deze module helpt je met jouw leerdoel: {}",
            "Deze module ondersteunt jouw ambitie om: {}",
            "Op basis van jouw leerdoel '{}' is deze module relevant",
            "Deze module draagt bij aan jouw doel: {}"
        ],
        "studie": [
            "Deze module sluit aan bij jouw achtergrond in: {}",
            "Deze module past bij jouw studie: {}",
            "Op basis van jouw ervaring met {} is deze module interessant",
            "Deze module bouwt voort op jouw kennis van: {}"
        ]
    }
    
    # Process each module in top 5
    results = []
    for module in top_5_modules:
        module_id = module["id"]
        
        # Get module description from metadata
        module_row = metadata_df[metadata_df["id"] == module_id]
        
        if module_row.empty:
            results.append({
                **module,
                "motivation": ""
            })
            continue
        
        # Build module description from available fields
        module_description_parts = []
        for col in ["modulename", "description", "content", "learninggoals"]:
            if col in module_row.columns:
                value = module_row.iloc[0][col]
                if pd.notna(value) and str(value).strip():
                    module_description_parts.append(str(value))
        
        module_description = ". ".join(module_description_parts)
        
        if not module_description.strip() or not student_chunks:
            results.append({
                **module,
                "motivation": ""
            })
            continue
        
        # Vectorize module description
        module_vector = model.encode(module_description, show_progress_bar=False, device=device)
        
        # Calculate similarity between each student chunk and the module
        chunk_similarities = []
        for chunk in student_chunks:
            similarity = cosine_similarity(
                chunk["vector"].reshape(1, -1),
                module_vector.reshape(1, -1)
            )[0][0]
            
            chunk_similarities.append({
                "text": chunk["text"],
                "category": chunk["category"],
                "relevance_score": float(similarity)
            })
        
        # Sort by relevance and get the best match
        chunk_similarities.sort(key=lambda x: x["relevance_score"], reverse=True)
        
        if chunk_similarities:
            best_match = chunk_similarities[0]
            category = best_match["category"]
            text = best_match["text"]
            
            # Split text into smaller keywords/phrases
            # Split by common delimiters: comma, semicolon, 'en', 'and'
            text_parts = re.split(r'[,;]|\s+en\s+|\s+and\s+', text)
            text_parts = [part.strip() for part in text_parts if part.strip()]
            
            # Take up to 3 most relevant parts (or all if less than 3)
            keywords = text_parts[:min(3, len(text_parts))]
            
            # Format keywords with bold markdown
            formatted_keywords = ", ".join([f"**{kw}**" for kw in keywords])
            
            # Select random template for this category
            template = random.choice(templates.get(category, templates["interesse"]))
            motivation_text = template.format(formatted_keywords)
            
            results.append({
                **module,
                "motivation": motivation_text
            })
        else:
            results.append({
                **module,
                "motivation": ""
            })
    
    return results

