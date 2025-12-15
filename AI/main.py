from typing import Union
from fastapi import FastAPI

app = FastAPI()

@app.get("/")
def read_root():
    return {"Hello": "World"}

@app.get('/predict')
def predict(input_data: Union[str, int, float]):
  return 'Wouter was not here yet!'