from pydantic import BaseModel, Field, field_validator

class StudentInput(BaseModel):
    current_study: str = Field(...,description="Je huidige studie")
    interests: list[str] = Field(...,description="Je interesses in zinnen en/of woorden")
    wanted_study_credit_range: tuple[int, int] = Field(...,description="(min max) study credits")
    location_preference: list[str] = Field(...,description="Je locatievoorkeur: 'Tilburg', 'Breda', 'Den Bosch'")
    learning_goals: list[str] = Field(...,description="Je leerdoelen in zinnen en/of woorden")
    level_preference: list[str] = Field(...,description="NLQF# niveaus")
    preferred_language: str = Field(...,description="Je taalsvoorkeur: 'Nederlands', 'Engels', 'Niet van toepassing'")

    @field_validator("wanted_study_credit_range")
    @classmethod
    def validate_credite_range(cls, value):
        min_credits, max_credits = value

        if min_credits < 0:
            raise ValueError("Minimun aantal credits kan niet negatief zijn")
        
        if max_credits < min_credits:
            raise ValueError("Maximum aantal credits kan niet lager zijn dan het minimum aantal credits")
        
        if max_credits > 30:
            raise ValueError("Het is niet mogelijk meer dan 30 studiepunten te verkgrijgen met een VKM")
        
        return value