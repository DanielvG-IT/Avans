from typing import Union
from fastapi import FastAPI
from controllers.predict_controller import router as predict_router

app = FastAPI()
app.include_router(predict_router)

@app.get("/")
def read_root():
    return {"Hello": "World"}


@app.get("/health")
def health_check():
    return {"status": "ok"}