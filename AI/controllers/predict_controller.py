from fastapi import APIRouter
from models.student_input import StudentInput

router = APIRouter(
    prefix="/predict",
    tags=["predict"]
)

@router.post("/")
def predict(data: StudentInput):
    return {
        "message": "Your input data is valid",
        "data": data
        }