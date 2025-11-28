from typing import List, Tuple


class StudentProfile:
    def __init__(
        self,
        current_study: str,
        interests: List[str],
        wanted_study_credit_range: Tuple[int, int],
        location_preference: List[str],
        learning_goals: List[str],
        level_preference: List[str],
        preferred_language: str,
    ):
        self.current_study = current_study
        self.interests = interests
        self.wanted_study_credit_range = wanted_study_credit_range
        self.location_preference = location_preference
        self.learning_goals = learning_goals
        self.level_preference = level_preference
        self.preferred_language = preferred_language

    def to_text(self) -> str:
        parts = []
        if self.interests:
            parts += self.interests
        if self.learning_goals:
            parts += self.learning_goals
        if self.current_study:
            parts.append(str(self.current_study))
        return " ".join(parts)

    def __repr__(self):
        return (
            f"interests={self.interests}, "
            f"wanted_study_credit_range={self.wanted_study_credit_range}, "
            f"location_preference={self.location_preference}, "
            f"learning_goals={self.learning_goals}, "
            f"preferred_language={self.preferred_language})"
        )
