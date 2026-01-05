from sklearn.metrics.pairwise import cosine_similarity
from sentence_transformers import SentenceTransformer
from models.student_input import StudentInput
import pandas as pd
import numpy as np
import json
import ast

import torch

def clean_prediction_data(data: StudentInput):
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