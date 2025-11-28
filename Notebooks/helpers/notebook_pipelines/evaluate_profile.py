from helpers.functs.nlp_backmap import build_token_backmap, make_pretty_term
from sklearn.feature_extraction.text import TfidfVectorizer
from helpers.functs.StudentProfile import StudentProfile
from sklearn.metrics.pairwise import cosine_similarity
from sklearn.decomposition import TruncatedSVD
from helpers.functs.NLP import hard_nlp
import pandas as pd
import numpy as np
import ast
import random

df = pd.read_csv("../Data/Cleaned/cleaned_dataset_hard-NLP.csv")
raw_df = pd.read_csv("../Data/Raw/Uitgebreide_VKM_dataset.csv")


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


def _build_base_text_df():
    big_string = (
        df["name"].fillna("")
        + " "
        + df["description"].fillna("")
        + " "
        + df["learningoutcomes"].fillna("")
        + " "
        + df["module_tags"].apply(lambda x: " ".join(x) if isinstance(x, list) else "")
    )

    big_df = pd.DataFrame({"id": df["id"], "text": big_string})
    return big_df


def _build_vectorizer_and_module_matrix(big_df: pd.DataFrame):
    vectorizer = TfidfVectorizer(
        max_features=1000000,
        ngram_range=(1, 2),
        stop_words=None,
    )
    X_modules_tfidf = vectorizer.fit_transform(big_df["text"])
    return vectorizer, X_modules_tfidf


def _strength_phrase(score: float, is_dutch: bool = True) -> str:
    if is_dutch:
        if score >= 0.6:
            return random.choice(
                [
                    "sluit extreem goed aan bij jouw profiel",
                    "is een bijna perfecte match met jouw interesses",
                    "past heel sterk bij wat jij leuk vindt",
                ]
            )
        elif score >= 0.4:
            return random.choice(
                [
                    "sluit goed aan bij jouw profiel",
                    "is een sterke match met jouw interesses",
                    "lijkt behoorlijk goed bij jou te passen",
                ]
            )
        else:
            return random.choice(
                [
                    "kan een interessante extra optie zijn",
                    "heeft een gematigde overlap met jouw interesses",
                    "kan alsnog relevant zijn op basis van delen van jouw profiel",
                ]
            )
    else:
        if score >= 0.6:
            return random.choice(
                [
                    "fits your profile extremely well",
                    "is a near-perfect match for your interests",
                    "aligns very strongly with what you like",
                ]
            )
        elif score >= 0.4:
            return random.choice(
                [
                    "fits your profile well",
                    "is a strong match for your interests",
                    "seems to match you quite well",
                ]
            )
        else:
            return random.choice(
                [
                    "could be an interesting additional option",
                    "has a moderate overlap with your interests",
                    "might still be relevant based on parts of your profile",
                ]
            )


def _motivation_sentence(row: pd.Series, is_dutch: bool = True) -> str:
    score = row["score"]
    words = row["Motivation"]
    module_name = row["module_name"]

    if is_dutch:
        base_options = [
            "Deze module {strength}. ",
            "{module} {strength}. ",
            "Op basis van jouw antwoorden {strength}. ",
        ]
    else:
        base_options = [
            "This module {strength}. ",
            "{module} {strength}. ",
            "Based on your answers, this module {strength}. ",
        ]

    strength = _strength_phrase(score, is_dutch=is_dutch)
    base_template = random.choice(base_options)

    if "{module}" in base_template:
        base_text = base_template.format(module=module_name, strength=strength)
    else:
        base_text = base_template.format(strength=strength)

    if isinstance(words, str) and words.strip():
        if is_dutch:
            profile_templates = [
                "In jouw studentenprofiel noem je **{words}**, wat goed aansluit bij deze module.",
                "Je profiel vertelt over **{words}**, en deze interesses komen sterk terug in deze module.",
                "We zien dat **{words}** uit jouw antwoorden sterk overlappen met deze module.",
            ]
        else:
            profile_templates = [
                "Your student profile mentions **{words}**, which match well with this module.",
                "In your profile you talk about **{words}**, and these interests align with this module.",
                "We found that **{words}** from your answers overlap strongly with this module.",
            ]
        profile_part = random.choice(profile_templates).format(words=words)
        return base_text + profile_part
    else:
        if is_dutch:
            fallback_options = [
                "Ook al vonden we geen heel specifieke trefwoorden, jouw totale profiel suggereert dat deze module interessant voor je kan zijn.",
                "We vonden geen hele sterke individuele woordmatches, maar jouw algemene profiel wijst toch in de richting van deze module.",
                "Er zijn geen heel duidelijke trefwoorden, maar op basis van het bredere profiel lijkt deze module alsnog bij je te passen.",
            ]
        else:
            fallback_options = [
                "Even though we did not find very specific keyword matches, the overall profile still suggests this module could be interesting.",
                "There are no very strong individual word matches, but your general profile still points towards this module.",
                "We did not find very clear keyword overlaps, but the broader profile still connects you to this module.",
            ]
        fallback = random.choice(fallback_options)
        return base_text + fallback


def run_evaluation_multi(students, matching_models_list, top_n: int = 5, k: int = 5):
    if len(students) != len(matching_models_list):
        raise ValueError("students and matching_models_list must have the same length")

    big_df = _build_base_text_df()
    vectorizer, X_modules_tfidf = _build_vectorizer_and_module_matrix(big_df)

    n_components = 200
    svd = TruncatedSVD(n_components=n_components, random_state=42)
    X_modules_reduced = svd.fit_transform(X_modules_tfidf)

    feature_names = vectorizer.get_feature_names_out()

    all_results = []
    precisions = []

    for student, matching_models in zip(students, matching_models_list):
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

        studentInterests = student.to_text()
        student_hardNLP = hard_nlp(studentInterests)

        token_map = build_token_backmap(studentInterests, student_hardNLP)
        pretty_term = make_pretty_term(token_map)

        X_interests_tfidf = vectorizer.transform([student_hardNLP])
        X_student_reduced = svd.transform(X_interests_tfidf)

        X_student_vec = X_student_reduced
        if X_student_vec.ndim == 1:
            X_student_vec = X_student_vec.reshape(1, -1)

        scores_global = cosine_similarity(X_student_vec, X_modules_reduced)[0]

        candidate_ids = set(filtered_df["id"].tolist())
        candidate_mask = big_df["id"].isin(candidate_ids)

        scores_candidates = scores_global[candidate_mask.values]
        idx_candidates = np.where(candidate_mask.values)[0]

        if len(idx_candidates) == 0:
            raise ValueError("No modules remain after filtering; cannot compute recommendations.")

        order = np.argsort(-scores_candidates)[:top_n]
        top_idx = idx_candidates[order]

        student_vec = X_interests_tfidf.toarray().flatten()

        def top_shared_terms(module_idx, top_k: int = 5) -> str:
            module_vec = X_modules_tfidf[module_idx].toarray().flatten()
            mask = (student_vec > 0) & (module_vec > 0)
            if not mask.any():
                return ""
            shared_scores = (student_vec * module_vec) * mask
            top_idx_local = np.argsort(-shared_scores)[:top_k]

            terms = []
            for i in top_idx_local:
                if shared_scores[i] > 0:
                    term = feature_names[i]
                    if term not in terms:
                        terms.append(term)
            return ", ".join(pretty_term(t) for t in terms)

        motivation_list = [top_shared_terms(i) for i in top_idx]

        module_ids = big_df.iloc[top_idx]["id"].values

        if "id" in raw_df.columns:
            module_names = []
            for mid in module_ids:
                row_match = raw_df[raw_df["id"] == mid]
                if not row_match.empty and "name" in row_match.columns:
                    module_names.append(row_match.iloc[0]["name"])
                else:
                    module_names.append("")
        else:
            module_names = big_df.iloc[top_idx]["text"].values

        recs = pd.DataFrame(
            {
                "rank": list(range(1, len(top_idx) + 1)),
                "module_id": module_ids,
                "module_name": module_names,
                "score": scores_candidates[order],
                "Motivation": motivation_list,
            }
        )

        use_dutch = True
        if hasattr(student, "preferred_language"):
            lang = str(student.preferred_language).strip().lower()
            if lang in ["en", "eng", "english"]:
                use_dutch = False

        recs["motivation_full"] = recs.apply(
            lambda row: _motivation_sentence(row, is_dutch=use_dutch), axis=1
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
