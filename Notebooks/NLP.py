import re
import string
from langdetect import detect, LangDetectException
from nltk.corpus import stopwords
from nltk.stem import PorterStemmer, SnowballStemmer, WordNetLemmatizer


def hard_nlp(text):
    # Returning if not a string.
    if not isinstance(text, str):
        return text 
    
    # All text to lower case --> Consistency
    text = text.lower()

    # Removing all numbers
    text = re.sub(r'\d+', '', text)

    # Removing all punctuation
    text = text.translate(str.maketrans("", "", string.punctuation))

    # Stop words removal
    # Loading stopwords
    stopwords_en = set(stopwords.words("english"))
    stopwords_nl = set(stopwords.words("dutch"))
    stopwords_both = stopwords_en.union(stopwords_nl)

    # Language detection (try with fallback)
    try:
        lang = detect(text)
    except LangDetectException:
        lang = "fallback"

    words = text.split()

    # Stopword removal
    if lang == "nl":
        cleaned = [w for w in words if w not in stopwords_nl]
    elif lang == "en":
        cleaned = [w for w in words if w not in stopwords_en]
    else:
        # Fallback, using combined stopwords library if unable to detect language, could result in loss of some meaning...
        cleaned = [w for w in words if w not in stopwords_both]

    # Stemming and Lemmatization
    stem_en = PorterStemmer()
    lemmatizer_en = WordNetLemmatizer()
    stem_nl = SnowballStemmer("dutch")

    cleaned_words = []
    for w in cleaned:
        if lang == "nl":
            # For Dutch we stem only because there is no Dutch lemmatizer available
            w = stem_nl.stem(w)
        elif lang == "en":
            # For English we lemmatize first then stem the word (Normal pipeline)
            w = lemmatizer_en.lemmatize(w)
            w = stem_en.stem(w)
        else:
            # As Fallback we use the English pipeline
            w = lemmatizer_en.lemmatize(w)
            w = stem_en.stem(w)

        cleaned_words.append(w)

    return " ".join(cleaned_words)

def soft_nlp(text):
    # Returning if not a string.
    if not isinstance(text, str):
        return text 
    
    # All text to lower case --> Consistency
    text = text.lower()

    # Removing all numbers
    text = re.sub(r'\d+', '', text)

    # Removing all punctuation
    text = text.translate(str.maketrans("", "", string.punctuation))

    words = text.split()

    return " ".join(words)

