from fastapi import APIRouter
from AI.models.student_input import StudentInput

# Importing services
from AI.services.predict_service import get_top_5_prediction

router = APIRouter(
    prefix="/predict",
    tags=["predict"]
)

# Endpoint: predict/ . POST: student input from front-end questionaire is taken in and recommendations are send back. 
@router.post("/")
# Data gets run through model field_testers to validate input. Things like negative credit points will return and code below controller doesn't run. -->Models\student_input.py
def predict(data: StudentInput):
    filtered_top_5_matches = get_top_5_prediction(data)

    return {
        "filtered_top_5_matches": filtered_top_5_matches,
    }