from fastapi import APIRouter
from models.student_input import StudentInput
from services.clean_and_stringify import clean_prediction_data

router = APIRouter(
    prefix="/predict",
    tags=["predict"]
)

@router.post("/")
def predict(data: StudentInput):

    # First we make one big string of the properties: current study, interests and learning goals. Then we will also remove the trailing white spaces. 
    cleaned_prediction_data = clean_prediction_data(data)
    return {
        "message": "Cleaned data:",
        "data": cleaned_prediction_data
    }