import re
import string
from langdetect import detect, LangDetectException
from nltk.corpus import stopwords
from nltk.stem import PorterStemmer, SnowballStemmer, WordNetLemmatizer

# Hard NLP for TF-IDF approach
def hard_nlp(text):
    if not isinstance(text, str):
        return text

    # All to lower casing and removing numbers, symbols and punctuations
    text = text.lower()
    text = re.sub(r"\d+", "", text)
    text = text.translate(str.maketrans("", "", string.punctuation))

    # Setting up stopword removal for both Dutch and English
    stopwords_en = set(stopwords.words("english"))
    stopwords_nl = set(stopwords.words("dutch"))
    stopwords_both = stopwords_en.union(stopwords_nl)

    # Using langDetect to detect which language is put it for diff stopword removal
    try:
        lang = detect(text)
    except LangDetectException:
        lang = "fallback"

    words = text.split()

    if lang == "nl":
        cleaned = [w for w in words if w not in stopwords_nl]
    elif lang == "en":
        cleaned = [w for w in words if w not in stopwords_en]
    else:
        cleaned = [w for w in words if w not in stopwords_both]

    # Initiliazing stemming and lemmaztization
    stem_en = PorterStemmer()
    lemmatizer_en = WordNetLemmatizer()
    stem_nl = SnowballStemmer("dutch")

    # Again using the lang detect, which language are we working with?
    cleaned_words = []
    for w in cleaned:
        if lang == "nl":
            w = stem_nl.stem(w)
        elif lang == "en":
            w = lemmatizer_en.lemmatize(w)
            w = stem_en.stem(w)
        else:
            w = lemmatizer_en.lemmatize(w)
            w = stem_en.stem(w)

        cleaned_words.append(w)

    return " ".join(cleaned_words)

# Soft NLP for SBERT approach
def soft_nlp(text):
    if not isinstance(text, str):
        return text

    # Minimal preprocessing, removing trailing white spaces only
    return text.strip()

from ..functs.NLP import *
