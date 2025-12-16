from typing import Union
from fastapi import FastAPI
from controllers.predict import router as predict_router

app = FastAPI()
app.include_router(predict_router)

@app.get("/")
def read_root():
    return {"Hello": "World"}

# @app.get('/predict')
# def predict(input_data: Union[str, int, float]):
#   return 'Wouter was not here yet!'