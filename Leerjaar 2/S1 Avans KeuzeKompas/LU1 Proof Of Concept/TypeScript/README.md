# 🎓 Proof of Concept

Dit project is een Proof of Concept (PoC) voor een studie-informatiesysteem
waarin studenten modules kunnen doorzoeken, filteren, bekijken en opslaan als
favoriet. De focus ligt op een werkende basisapplicatie met frontend, backend en
een document database.

---

## ✨ Functionaliteiten (Proof of Concept Scope)

De PoC bevat de volgende user stories:

### 🔍 Zoeken & Filteren

- Student kan modules doorzoeken op naam/trefwoord.
- Student kan filteren op:
  - Studiepunten (15 of 30 EC)
  - Niveau (NLQF-5 of NLQF-6)
  - Thema

### 📖 Detailpagina Module

- Student kan op een module klikken en de beschrijving en details zien.  
  Details bevatten:
  - Naam module
  - Aantal EC’s
  - Niveau
  - Type (verdiepend / verbredend)

### ⭐ Favorietenlijst

- Student kan modules opslaan als favoriet.
- Student kan favorietenlijst bekijken en modules verwijderen.

---

## 🛠 Technische randvoorwaarden

1. Frontend & Backend
   - Gecodeerd in TypeScript.
   - Frontend: component-based.
   - Backend: opgezet volgens Onion Architecture.
2. Versiebeheer
   - Frontend en backend in Git.
3. Hosting
   - Frontend en backend afzonderlijk in de cloud (App Service of container).
   - Let op CORS-configuratie.
4. Authenticatie & security
   - Communicatie via JWT-token.
   - Geen API-keys hardcoded.
5. CI/CD
   - Automatisch testen en uitrollen bij release branches.
6. Testing
   - Minimaal 1 happy path en 1 unhappy path integratietest.
   - Minimaal 5 unit-tests (frontend of backend).
   - Minimaal 5 systeemtesten.
   - Testplan incl. traceability matrix naar functionele requirements.
7. Data & validatie
   - Document database online.
   - Data persistent met get/create/update-acties.
   - Inputvalidatie in frontend én backend.

---

## 📋 Belangrijk

Dit project is een Proof of Concept. De scope is bewust beperkt tot zoeken,
filteren, detailpagina en favorieten met persistente data in MongoDB.

## 👨‍💻 Auteur

Daniël van Ginneken  
Avans Hogeschool - HBO Software Development
