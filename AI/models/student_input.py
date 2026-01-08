from pydantic import BaseModel, Field, field_validator

ALLOWED_LEVELS = {"NLQF1", "NLQF2", "NLQF3", "NLQF4", "NLQF5", "NLQF6", "NLQF7", "NLQF8"}
ALLOWED_LANGUAGES = {"Nederlands", "Engels", "Niet van toepassing"}
ALLOWED_LOCATIONS = {"Tilburg", "Breda", "Den Bosch"}
ALLOWED_PERIODS = {"P1", "P2", "P3", "P4"}

class StudentInput(BaseModel):
    current_study: str = Field(..., description="Je huidige studie")
    interests: list[str] = Field(..., description="Je interesses in zinnen en/of woorden")
    wanted_study_credit_range: tuple[int, int] = Field(..., description="(min max) study credits")
    location_preference: list[str] = Field(..., description="Je locatievoorkeur: 'Tilburg', 'Breda', 'Den Bosch'")
    learning_goals: list[str] = Field(..., description="Je leerdoelen in zinnen en/of woorden")
    level_preference: list[str] = Field(..., description="NLQF# niveaus")
    preferred_language: str = Field(..., description="Je taalsvoorkeur: 'Nederlands', 'Engels', 'Niet van toepassing'")
    preferred_period: list[str] = Field(..., description="Je voorkeur voor de periode waarin de VKM wordt gegeven")

    @field_validator("wanted_study_credit_range")
    def validate_credit_range(value):
        if not (isinstance(value, (list, tuple)) and len(value) == 2):
            raise ValueError("Credit range must contain exactly two integers: (min, max)")
        min_credits, max_credits = value
        if min_credits < 0:
            raise ValueError("Minimun aantal credits kan niet negatief zijn")
        if max_credits < min_credits:
            raise ValueError("Maximum aantal credits kan niet lager zijn dan het minimum aantal credits")
        if max_credits > 30:
            raise ValueError("Het is niet mogelijk meer dan 30 studiepunten te verkrijgen met een VKM")
        return value

    @field_validator("current_study")
    def validate_current_study(value):
        if not value.strip():
            raise ValueError("Huidige studie kan niet leeg zijn")
        return value

    @field_validator("learning_goals", "interests")
    def validate_lists(value):
        if not value or not all(v.strip() for v in value):
            raise ValueError("Leerdoelen en interesses kunnen niet leeg zijn")
        return [v.strip() for v in value]

    @field_validator("location_preference")
    def validate_locations(value):
        if not value:
            raise ValueError("Tenminste één locatievoorkeur is vereist")
        for location in value:
            if location not in ALLOWED_LOCATIONS:
                raise ValueError(f"Locatievoorkeur '{location}' is ongeldig. Geldige opties zijn: {', '.join(ALLOWED_LOCATIONS)}")
        return value

    @field_validator("level_preference")
    def validate_levels(value):
        if not value:
            raise ValueError("Tenminste één niveauvoorkeur is vereist")
        for level in value:
            if level not in ALLOWED_LEVELS:
                raise ValueError(f"Niveauvoorkeur '{level}' is ongeldig. Geldige opties zijn: {', '.join(ALLOWED_LEVELS)}")
        return value

    @field_validator("preferred_language")
    def validate_language(value):
        if value not in ALLOWED_LANGUAGES:
            raise ValueError(f"Taalvoorkeur '{value}' is ongeldig. Geldige opties zijn: {', '.join(ALLOWED_LANGUAGES)}")
        return value
    
    @field_validator("preferred_period")
    def validate_period(value):
        if not value:
            raise ValueError("Tenminste één periodevoorkeur is vereist")
        for period in value:
            if period not in ALLOWED_PERIODS:
                raise ValueError(f"Periodevoorkeur '{period}' is ongeldig. Geldige opties zijn: {', '.join(ALLOWED_PERIODS)}")
        return value
