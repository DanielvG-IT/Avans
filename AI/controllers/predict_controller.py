from fastapi import APIRouter
from models.student_input import StudentInput

# Importing services
from services.predict_service import clean_prediction_data, vectorize_student_input, filter_matches_top_5

router = APIRouter(
    prefix="/predict",
    tags=["predict"]
)

# Endpoint: predict/ . POST: student input from front-end questionaire is taken in and recommendations are send back. 
@router.post("/")
# Data gets run through model field_testers to validate input. Things like negative credit points will return and code below controller doesn't run. -->Models\student_input.py
def predict(data: StudentInput):

    # First we make one big string of the properties: current study, interests and learning goals. Then we will also remove the trailing white spaces. 
    cleaned_prediction_data = clean_prediction_data(data)

    # Now vectorizing user input using SBERT model
    vectorized_student_input = vectorize_student_input(cleaned_prediction_data)

    # Filtering (metadata first) + cosine similarity + top 5
    filtered_top_5_matches = filter_matches_top_5(vectorized_student_input, data)

    return {
        "filtered_top_5_matches": filtered_top_5_matches,
    }