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

def top5_via_cosine_similarity(vectorized_student_input):
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

    # Picking only top 5
    top_5 = embedded_modules.sort_values(by='similarity_score', ascending=False).head(5)
    return json.loads(top_5.to_json(orient="records"))