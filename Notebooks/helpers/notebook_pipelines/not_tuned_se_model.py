from helpers.functs.StudentProfile import StudentProfile
from helpers.functs.motivation_se import add_motivation_column_se
from sentence_transformers import SentenceTransformer
from sklearn.metrics.pairwise import cosine_similarity
from helpers.functs.NLP import soft_nlp
import pandas as pd
import numpy as np
import ast
from typing import Iterable, List, Tuple

# Load datasets once at module import (similar pattern as BOW helper)
df = pd.read_csv("../Data/Cleaned/cleaned_dataset_soft-NLP.csv")
raw_df = pd.read_csv("../Data/Raw/Uitgebreide_VKM_dataset.csv")
embedded_modules = pd.read_csv("../Data/Vectorized/sentence_embedded_dataframe.csv")


def _normalize_locations(series: pd.Series) -> pd.Series:
    def _to_list(val):
        try:
            parsed = ast.literal_eval(str(val))
            if isinstance(parsed, list):
                return [str(x).strip().lower() for x in parsed]
            return [str(parsed).strip().lower()]
        except Exception:
            return [str(val).strip().lower()]

    return series.apply(_to_list)


def _build_filtered_df(student: StudentProfile) -> pd.DataFrame:
    filtered_df = df.copy()

    if hasattr(student, "wanted_study_credit_range") and student.wanted_study_credit_range is not None:
        min_cred, max_cred = student.wanted_study_credit_range
        filtered_df = filtered_df[
            (filtered_df["studycredit"] >= min_cred)
            & (filtered_df["studycredit"] <= max_cred)
        ]

    if hasattr(student, "location_preference") and student.location_preference:
        all_locs_filtered = _normalize_locations(filtered_df["location"])
        loc_prefs_norm = [str(x).strip().lower() for x in student.location_preference]
        loc_mask = all_locs_filtered.apply(lambda lst: any(x in loc_prefs_norm for x in lst))
        filtered_df = filtered_df[loc_mask]

    if hasattr(student, "level_preference") and student.level_preference:
        level_prefs = [str(x).strip().lower() for x in student.level_preference]
        filtered_df = filtered_df[
            filtered_df["level"].astype(str).str.lower().isin(level_prefs)
        ]

    filtered_df = filtered_df[filtered_df["available_spots"] > 0]

    return filtered_df


def _prepare_embedded_modules() -> pd.DataFrame:
    em = embedded_modules.copy()
    if "sentence_embedding_vector" not in em.columns:
        raise KeyError("embedded_modules must contain 'sentence_embedding_vector' column.")
    if "id" not in em.columns:
        raise KeyError("embedded_modules must contain 'id' column.")

    em["sentence_embedding_vector"] = em["sentence_embedding_vector"].apply(
        lambda x: np.fromstring(x.strip("[]"), sep=" ") if isinstance(x, str) else np.array(x)
    )
    return em


_EMBEDDED_MODULES_PREPARED = _prepare_embedded_modules()


def _recommend_for_student(
    student: StudentProfile,
    model: SentenceTransformer,
    top_n: int = 5,
) -> Tuple[pd.DataFrame, pd.DataFrame]:
    """Run sentence-embedding recommendation for a single student.

    Returns (recs, filtered_df).
    """

    filtered_df = _build_filtered_df(student)

    studentInterests = student.to_text()
    student_softNLP = soft_nlp(studentInterests)

    embedded_student_input = model.encode(student_softNLP, show_progress_bar=False)
    if isinstance(embedded_student_input, list):
        embedded_student_input = np.array(embedded_student_input)
    if embedded_student_input.ndim == 1:
        embedded_student_input = embedded_student_input.reshape(1, -1)

    module_matrix = np.stack(_EMBEDDED_MODULES_PREPARED["sentence_embedding_vector"].values)

    scores_global = cosine_similarity(embedded_student_input, module_matrix)[0]

    candidate_ids = set(filtered_df["id"].tolist())
    candidate_mask = _EMBEDDED_MODULES_PREPARED["id"].isin(candidate_ids)

    scores_candidates = scores_global[candidate_mask.values]
    idx_candidates = np.where(candidate_mask.values)[0]

    if len(idx_candidates) == 0:
        raise ValueError("No modules remain after filtering; cannot compute recommendations.")

    order = np.argsort(-scores_candidates)[:top_n]
    top_idx = idx_candidates[order]

    module_ids = _EMBEDDED_MODULES_PREPARED.iloc[top_idx]["id"].values

    module_names: List[str] = []
    for mid in module_ids:
        row_match = raw_df[raw_df["id"] == mid]
        if not row_match.empty and "name" in row_match.columns:
            module_names.append(row_match.iloc[0]["name"])
        else:
            module_names.append("")

    recs = pd.DataFrame(
        {
            "rank": list(range(1, len(top_idx) + 1)),
            "module_id": module_ids,
            "module_name": module_names,
            "score": scores_candidates[order],
        }
    ).reset_index(drop=True)

    return recs, filtered_df


def run_evaluation_multi(
    students: Iterable[StudentProfile],
    matching_models_list: Iterable[Iterable[int]],
    top_n: int = 5,
    k: int = 5,
):
    """Sentence-embedding equivalent van run_evaluation_multi voor BOW.

    - Neemt een lijst van StudentProfile-objecten en bijbehorende ground-truth lijsten.
    - Geeft per student een dict met de recommendations (inclusief motivatie) en precision@k terug.
    - Retourneert ook de gemiddelde precision@k over alle studenten.
    """

    students = list(students)
    matching_models_list = list(matching_models_list)
    if len(students) != len(matching_models_list):
        raise ValueError("students and matching_models_list must have the same length")

    # Gebruik hetzelfde sentence-embedding model als in notebook 3.2
    # en voor de gegenereerde embeddings in sentence_embedded_dataframe.csv
    model = SentenceTransformer("all-MiniLM-L6-v2")

    all_results = []
    precisions: List[float] = []

    for student, matching_models in zip(students, matching_models_list):
        recs, _ = _recommend_for_student(student, model=model, top_n=top_n)

        student_profile_text = student.to_text()
        recs = add_motivation_column_se(
            recs=recs,
            student_profile_text=student_profile_text,
            preferred_language=getattr(student, "preferred_language", "NL"),
            raw_df=raw_df,
            model=model,
        )

        relevant_ids = set(matching_models)
        recommended_ids = recs["module_id"].tolist()
        kk = min(k, len(recommended_ids))
        top_k_ids = recommended_ids[:kk]

        hits = sum(1 for mid in top_k_ids if mid in relevant_ids)
        precision_at_k = hits / kk if kk > 0 else 0.0

        print(f"Relevant module IDs (ground truth): {sorted(relevant_ids)}")
        print(f"Top-{kk} recommended IDs: {top_k_ids}")
        print(f"Hits in top-{kk}: {hits}")
        print(f"precision@{kk}: {precision_at_k:.3f}")
        print("-" * 50)

        all_results.append({
            "student": student,
            "recs": recs,
            "precision_at_k": precision_at_k,
        })
        precisions.append(precision_at_k)

    avg_precision_at_k = float(np.mean(precisions)) if precisions else 0.0
    return all_results, avg_precision_at_k
