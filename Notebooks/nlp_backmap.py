import re
from collections import defaultdict
from typing import Dict, Set


def build_token_backmap(original_text: str, normalized_text: str) -> Dict[str, Set[str]]:
    """Bouw een mapping van NLP-token -> set van originele woorden.

    Parameters
    ----------
    original_text : str
        De originele studentinput (voor NLP), bijvoorbeeld uit `student.to_text()`.
    normalized_text : str
        De genormaliseerde tekst na NLP, bijvoorbeeld uit `hard_nlp(original_text)`.

    Returns
    -------
    Dict[str, Set[str]]
        Een dictionary waarbij de sleutel de NLP-token is (bijv. 'teken') en de
        waarde een set met originele varianten (bijv. {'tekening'}).
    """
    orig_tokens = re.findall(r"[A-Za-zÀ-ÿ]+", original_text.lower())
    nlp_tokens = normalized_text.split()

    token_map: Dict[str, Set[str]] = defaultdict(set)
    min_len = min(len(orig_tokens), len(nlp_tokens))

    for i in range(min_len):
        orig = orig_tokens[i]
        norm = nlp_tokens[i]
        token_map[norm].add(orig)

    return token_map


def make_pretty_term(token_map: Dict[str, Set[str]]):
    """Maak een functie die een NLP-term naar een 'mooie' vorm terugvertaalt.

    Voorbeeld:
        pretty = make_pretty_term(token_map)
        pretty("teken") -> "tekening" (als dat in de input stond)

    Als er geen mapping bestaat, wordt de NLP-term zelf teruggegeven.
    """

    def _pretty(term: str) -> str:
        if term in token_map and token_map[term]:
            return sorted(token_map[term])[0]
        return term

    return _pretty
