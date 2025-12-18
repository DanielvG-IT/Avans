from sentence_transformers import SentenceTransformer
from models.student_input import StudentInput
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