from fastapi import APIRouter

router = APIRouter(
    prefix="/predict",
    tags=["predict"]
)

@router.post("/")
def predict(data: dict):
    return {"result": data}