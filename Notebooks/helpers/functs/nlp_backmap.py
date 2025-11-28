import re
from collections import defaultdict
from typing import Dict, Set


def build_token_backmap(original_text: str, normalized_text: str) -> Dict[str, Set[str]]:
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
    def _pretty(term: str) -> str:
        if term in token_map and token_map[term]:
            return sorted(token_map[term])[0]
        return term

    return _pretty
from ..functs.nlp_backmap import *
