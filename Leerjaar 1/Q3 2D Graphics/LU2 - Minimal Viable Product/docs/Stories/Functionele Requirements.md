# Functionele eisen

## Must have

- [x] Als gebruiker wil ik mezelf kunnen registeren op basis van een gebruikersnaam en wachtwoord.
  - [x] Er bestaat nog geen gebruiker met deze gebruikersnaam. De gebruikersnaam is uniek.
  - [x] Wachtwoord moet minimaal 10 karakters lang zijn.
  - [x] Wachtwoord moet minstens 1 lowercase, uppercase, cijfer en niet-alphanumeriek karakter bevatten.
- [x] Als gebruiker wil ik kunnen inloggen op basis van mijn gebruikersnaam en wachtwoord.
  - [x] De gebruiker ziet een foutmelding wanneer de gebruikersnaam of wachtwoord niet correct is.
- [x] Als gebruiker wil ik een nieuwe lege 2D-wereld aan kunnen maken.
  - [x] De gebruiker moet ingelogd zijn.
  - [x] De gebruiker moet een naam invoeren voor de nieuwe 2D-wereld.
  - [x] De naam voor de nieuwe 2D-wereld mag niet identiek zijn aan de naam van een al bestaande 2D-wereld van de gebruiker.
  - [x] De naam voor de nieuwe 2D-wereld is minimaal 1 karakter en maximaal 25 karakters lang.
  - [x] De gebruiker mag niet meer dan 5 eigen 2D-werelden hebben.
  - [x] De nieuwe 2D-wereld wordt opgeslagen.
- [x] Als gebruiker wil ik een overzicht van mijn bestaande 2D-werelden kunnen bekijken.
  - [x] De gebruiker moet ingelogd zijn.
  - [x] Het overzicht toont de naam van de bestaande 2D-werelden van de gebruiker.
- [ ] Als gebruiker wil ik één van mijn bestaande 2D-werelden kunnen bekijken.
  - [ ] De gebruiker moet ingelogd zijn.
  - [ ] 2D-objecten die gekoppeld zijn aan deze 2D-wereld worden correct getoond op basis van de attributen van het 2D-object.
  - [ ] De gebruiker kan alleen eigen 2D-werelden bekijken.
- [ ] Als gebruiker wil ik een 2D-object aan mijn openstaande 2D-wereld kunnen toevoegen.
  - [ ] De gebruiker moet ingelogd zijn.
  - [ ] De gebruiker kan kiezen uit minimaal 3 beschikbare 2D-objecten.
  - [ ] Het nieuwe 2D-object wordt opgeslagen.
- [ ] Als gebruiker wil ik een door mij gemaakte 2D-wereld kunnen verwijderen.
  - [ ] De gebruiker moet ingelogd zijn.
  - [ ] 2D-objecten die gekoppeld zijn aan deze 2D-wereld worden ook verwijderd.

## Should have

- [ ] Als gebruiker wil ik de positie, rotatie of schaal van een bestaand 2D-object in een door mij gemaakte 2D-wereld kunnen aanpassen.
  - [ ] De gebruiker moet ingelogd zijn.
  - [ ] De aanpassing van het 2D-object wordt getoond.
  - [ ] De aanpassing van het 2D-object wordt opgeslagen.
- [ ] Als gebruiker wil ik een 2D-object in een door mij gemaakte 2D-wereld kunnen verwijderen.
  - [ ] De gebruiker moet ingelogd zijn.
  - [ ] Het 2D-object wordt niet meer getoond.
  - [ ] Verwijderen van het 2D-object wordt opgeslagen.
- [ ] Als gebruiker wil ik de camera kunnen bewegen terwijl ik een 2D-wereld bekijk zodanig dat ik heel de 2D-wereld kan bekijken.
  - [ ] De gebruiker kan de camera naar links, rechts, boven en onder bewegen met de bijbehorende pijltjestoetsen op het toetsenbord.
- [ ] Als gebruiker wil ik de grootte van mijn 2D-wereld specifiëren in maximale lengte (X) en maximale hoogte (Y) wanneer ik een nieuwe lege 2D-wereld aanmaak.
  - [x] De maximale lengte moet een geheel getal zijn tussen 20 en 200.
  - [x] De maximale hoogte moet een geheel getal zijn tussen 10 en 100.
  - [ ] De positie van 2D-objecten in deze 2D-wereld mag zich niet buiten de maximale lengte of maximale hoogte van de 2D-wereld bevinden.
- [ ] Als gebruiker wil ik een door mij gemaakte 2D-wereld kunnen delen met een andere gebruiker op basis van hun gebruikersnaam.
  - [ ] De gebruiker voert de gebruikersnaam in van de user waarmee ze hun 2D-wereld willen delen.
  - [ ] De gebruiker ontvangt geen feedback over de ingegeven gebruikersnaam, enkel bevestiging dat de 2D-wereld met deze user nu gedeeld is.
  - [ ] Indien de gebruikersnaam bestaat dan kan de gebruiker van dit account de gedeelde 2D-wereld vanaf nu bekijken.

## Nice to have

- [ ] Als gebruiker wil ik dat door mij aangemaakte, aangepaste en verwijderde 2D-objecten pas worden opgeslagen wanneer ik kies om mijn uitgevoerde acties op te slaan in plaats van dat deze onmiddellijk opgeslagen worden.
  - [ ] Data over aangemaakte, aangepaste en verwijderde 2D-objecten worden pas opgeslagen wanneer de gebruiker op een Save-knop klikt.
