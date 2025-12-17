from models.student_input import StudentInput

def clean_prediction_data(data: StudentInput):
    big_string = ". ".join([
        data.current_study,
        ". ".join(data.interests),
        ". ".join(data.learning_goals),
    ])
    return big_string.strip()