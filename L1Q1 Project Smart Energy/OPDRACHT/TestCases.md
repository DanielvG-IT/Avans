# Basis eisen

Hieronder staan de testen die betrekking hebben op de basis eisen voor opdrachtvariant F.

---

| **Testcase Id**     | PSE-BA-1 |
|---------------------|-----------|
| **Titel**           | Weergave van naam op de webpagina |
| **Beschrijving**    | Verifiëren dat de naam van de gebruiker bovenaan de pagina correct wordt weergegeven |
| **Precondities**    | N/A |
| **Invoer**          | N/A |
| **Stappen**         | Open de browser en ga naar <http://localhost:5013>. Klik in het menu op Opladen EV |
| **Verwachte uitkomst** | De webpagina wordt correct geladen. De naam van de student wordt bovenaan de webpagina getoond |

| **Testcase Id**     | PSE-BA-2 |
|---------------------|-----------|
| **Titel**           | Beschrijving van het algoritme |
| **Beschrijving**    | Controleren of de beschrijving van het algoritme correct en volledig wordt weergegeven onder de naam van de gebruiker |
| **Precondities**    | N/A |
| **Invoer**          | N/A |
| **Stappen**         | Open de browser en ga naar <http://localhost:5013>. Klik in het menu op Opladen EV |
| **Verwachte uitkomst** | De webpagina wordt correct geladen. De webpagina bevat een beschrijving van het algoritme. |

| **Testcase Id**     | PSE-BA-3 |
|---------------------|-----------|
| **Titel**           | Analyse van piekvermogen voor één dag |
| **Beschrijving**    | Verifiëren dat de laadsessies correct worden geregistreerd en correct worden weergegeven |
| **Precondities**    | N/A |
| **Invoer**          | N/A |
| **Stappen**         | Open de browser en ga naar <http://localhost:5013>. Klik in het menu op Opladen EV |
| **Verwachte uitkomst** | Er wordt een lijst met laadsessies getoond met alle bijbehorende variabelen |

| **Testcase Id**     | PSE-EX-1 |
|---------------------|-----------|
| **Titel**           | Selectie van meerdere huishouden |
| **Beschrijving**    | Verifiëren dat de gebruiker tussen verschillende huishoudens kan kiezen en dat de juiste analyse voor elk huishouden wordt uitgevoerd |
| **Precondities**    | N/A |
| **Invoer**          | Meter ID in decimaal |
| **Stappen**         | Open de browser en ga naar <http://localhost:5013>. Klik in het menu op Opladen EV. Voer meterId in bij invulveld. |
| **Verwachte uitkomst** | De webpagina wordt correct geladen. De juiste gegevens voor het geselecteerde huishouden worden geanalyseerd en weergegeven. Na het invoeren van een ander meterId wordt de webpagina opnieuw geladen met de informatie van die meter |
