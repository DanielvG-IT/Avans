import random
from typing import Callable

import pandas as pd
import numpy as np
from sentence_transformers import SentenceTransformer
from sklearn.metrics.pairwise import cosine_similarity


def strength_phrase_se(score: float, is_dutch: bool = True) -> str:
    # Map the similarity score to a short strength phrase

    if is_dutch:
        if score >= 0.6:
            return random.choice([
                "sluit extreem goed aan bij jouw profiel",
                "is een bijna perfecte match met jouw interesses",
                "past heel sterk bij wat jij leuk vindt",
            ])
        elif score >= 0.4:
            return random.choice([
                "sluit goed aan bij jouw profiel",
                "is een sterke match met jouw interesses",
                "lijkt behoorlijk goed bij jou te passen",
            ])
        else:
            return random.choice([
                "kan een interessante extra optie zijn",
                "heeft een gematigde overlap met jouw interesses",
                "kan alsnog relevant zijn op basis van delen van jouw profiel",
            ])
    else:
        if score >= 0.6:
            return random.choice([
                "fits your profile extremely well",
                "is a near-perfect match for your interests",
                "aligns very strongly with what you like",
            ])
        elif score >= 0.4:
            return random.choice([
                "fits your profile well",
                "is a strong match for your interests",
                "seems to match you quite well",
            ])
        else:
            return random.choice([
                "could be an interesting additional option",
                "has a moderate overlap with your interests",
                "might still be relevant based on parts of your profile",
            ])


def _split_into_snippets(text: str, min_len: int = 40) -> list[str]:
    # Split the profile text into short sentence snippets

    if not isinstance(text, str):
        return []

    raw = text.replace("\n", " ")
    # Split on clear sentence enders; keep semicolons/colons as is
    for sep in ["?", "!", "."]:
        raw = raw.replace(sep, ".")

    parts = [p.strip() for p in raw.split(".")]

    # Keep only sentences that are long enough but not extremely long
    snippets: list[str] = []
    for p in parts:
        if not p:
            continue
        if " " not in p:
            continue

        tokens = p.split()
        # Skip very short fragments (e.g., single words like "Business")
        if len(tokens) < 6:
            continue

        # Trim very long sentences: keep only the first ~20 words
        if len(tokens) > 20:
            p = " ".join(tokens[:20]) + " ..."

        # Skip snippets that contain very odd long tokens (noise words)
        if any(len(tok) > 20 for tok in tokens):
            continue

        if len(p) >= min_len:
            snippets.append(p)
    return snippets if snippets else [raw.strip()] if raw.strip() else []


def _best_profile_snippets_for_module(
    student_profile_text: str,
    module_text: str,
    model: SentenceTransformer,
    device: str = "cpu",
    top_k: int = 3,
) -> list[str]:
    # Find the profile snippet that is most similar to the module text
    snippets = _split_into_snippets(student_profile_text)
    if not snippets:
        return []

    # Embed profile snippets and module text
    snippet_embeddings = model.encode(snippets, device=device)
    module_embedding = model.encode([module_text], device=device)[0].reshape(1, -1)

    sims = cosine_similarity(module_embedding, np.vstack(snippet_embeddings))[0]

    order = np.argsort(-sims)
    # Take only the best snippet with similarity > 0 for readability. Otherwise too cluttered
    for idx in order:
        if sims[idx] > 0:
            return [snippets[idx]]
    return []


def motivation_sentence_se(
    row: pd.Series,
    student_text: str,
    module_text: str,
    model: SentenceTransformer,
    device: str = "cpu",
    is_dutch: bool = True,
    formatter: Callable[[str], str] | None = None,
) -> str:
        # Build one motivation sentence for a recommended module

    score = float(row.get("score", 0.0))
    module_name = str(row.get("module_name", "deze module"))

    if formatter is None:
        def formatter(x: str) -> str:  # type: ignore[no-redef]
            return x

    strength = strength_phrase_se(score, is_dutch=is_dutch)

    # Pick the best matching profile snippets for this module
    best_snippets = _best_profile_snippets_for_module(
        student_profile_text=student_text,
        module_text=module_text,
        model=model,
        top_k=3,
    )

    if is_dutch:
        base_options = [
            "Deze module {strength}. ",
            "{module} {strength}. ",
            "Op basis van jouw antwoorden {strength}. ",
        ]
        if best_snippets:
            profile_templates = [
                "Vooral omdat je aangeeft: \"{snippet}\".",
                "Dit sluit aan bij wat je vertelt: \"{snippet}\".",
                "Met name jouw beschrijving \"{snippet}\" komt sterk terug in deze module.",
                "We herkennen hierin wat je schreef: \"{snippet}\".",
                "Je profiel benadrukt vooral \"{snippet}\", wat hier goed bij past.",
            ]
        else:
            profile_templates = [
                "We vonden geen heel specifieke zinnen, maar jouw totale profiel sluit nog steeds goed aan.",
                "Ook al sprongen er geen losse zinnen uit, je algemene profiel past bij deze module.",
            ]
    else:
        base_options = [
            "This module {strength}. ",
            "{module} {strength}. ",
            "Based on your answers, this module {strength}. ",
        ]
        if best_snippets:
            profile_templates = [
                "Especially because you mention: \"{snippet}\".",
                "This aligns with what you wrote: \"{snippet}\".",
                "In particular, your description \"{snippet}\" matches this module well.",
                "We see a strong link with what you wrote: \"{snippet}\".",
                "Your profile highlights \"{snippet}\", which fits nicely here.",
            ]
        else:
            profile_templates = [
                "We did not find very specific sentences, but your overall profile still matches well.",
                "Even without clear individual sentences, your general profile fits this module.",
            ]

    base_template = random.choice(base_options)
    strength_text = strength

    if "{module}" in base_template:
        base_text = base_template.format(module=module_name, strength=strength_text)
    else:
        base_text = base_template.format(strength=strength_text)

    if best_snippets:
        # Use only the best snippet to keep the text readable
        best = formatter(best_snippets[0])
        profile_part = random.choice(profile_templates).format(snippet=best)
        return base_text + profile_part

    # Fallback if we could not find any good snippet
    fallback = random.choice(profile_templates)
    return base_text + fallback


def add_motivation_column_se(
    recs: pd.DataFrame,
    student_profile_text: str,
    preferred_language: str | None = None,
    raw_df: pd.DataFrame | None = None,
    model: SentenceTransformer | None = None,
    device: str = "cpu",
) -> pd.DataFrame:
    # Add a motivation text column to the recommendations DataFrame

    use_dutch = True
    if isinstance(preferred_language, str):
        lang = preferred_language.strip().lower()
        if lang in ["en", "eng", "english"]:
            use_dutch = False

    if model is None:
        model = SentenceTransformer("paraphrase-multilingual-mpnet-base-v2")

    recs_out = recs.copy()

    # For each row, build a short text that describes the module
    def build_module_text(row: pd.Series) -> str:
        if raw_df is not None and "module_id" in row and "id" in raw_df.columns:
            mid = row["module_id"]
            match = raw_df[raw_df["id"] == mid]
            if not match.empty:
                name = str(match.iloc[0].get("name", ""))
                desc = str(match.iloc[0].get("description", ""))
                los = str(match.iloc[0].get("learningoutcomes", ""))
                return " ".join([name, desc, los]).strip()
        # fallback: alleen de module_name uit recs
        return str(row.get("module_name", ""))

    recs_out["motivation_full"] = recs_out.apply(
        lambda row: motivation_sentence_se(
            row,
            student_text=student_profile_text,
            module_text=build_module_text(row),
            model=model,
            is_dutch=use_dutch,
        ),
        axis=1,
    )

    return recs_out
