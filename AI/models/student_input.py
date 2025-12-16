from pydantic import BaseModel

class StudentInput(BaseModel):
    current_study: str #bijv informatica
    interests: list[str] #Keywords en zin(nen)
    wanted_study_credit_range: list[int] #bijv 15-30
    location_preference: list[str] #Den Bosch, Breda, Tilburg
    laerning_goals: list[str] #Keywords en zin(nen)
    level_preference: list[str] #NLQF niveaus
    preferred_language: str #Voor nu alleen maar: engels, nederlands, 'maakt niet uit'