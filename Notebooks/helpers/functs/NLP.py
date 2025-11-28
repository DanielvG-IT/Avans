import re
import string
from langdetect import detect, LangDetectException
from nltk.corpus import stopwords
from nltk.stem import PorterStemmer, SnowballStemmer, WordNetLemmatizer


def hard_nlp(text):
    if not isinstance(text, str):
        return text

    text = text.lower()
    text = re.sub(r"\d+", "", text)
    text = text.translate(str.maketrans("", "", string.punctuation))

    stopwords_en = set(stopwords.words("english"))
    stopwords_nl = set(stopwords.words("dutch"))
    stopwords_both = stopwords_en.union(stopwords_nl)

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

    stem_en = PorterStemmer()
    lemmatizer_en = WordNetLemmatizer()
    stem_nl = SnowballStemmer("dutch")

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


def soft_nlp(text):
    if not isinstance(text, str):
        return text

    text = text.lower()
    text = re.sub(r"\d+", "", text)
    text = text.translate(str.maketrans("", "", string.punctuation))
    words = text.split()
    return " ".join(words)
from ..functs.NLP import *
