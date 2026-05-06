# Posterpresentatie - Talk Track (4-5 min)

## 1) Opening (20 sec)

"Wat voelt sneller op je telefoon, en wat is uiteindelijk zuiniger?"

Mijn onderzoek vergelijkt SSR en CSR in dezelfde Next.js-app. Niet alleen op snelheid, maar vooral op wat het device zelf moet doen.

## 2) Introductie + eerder onderzoek (35-45 sec)

- Veel SSR/CSR-vergelijkingen kijken vooral naar latency en UX.
- Minder onderzoek kijkt naar client-side energie op een echte smartphone.
- Eerder onderzoek suggereert dat SSR sterker wordt bij zwaardere pagina's, terwijl CSR aantrekkelijk blijft voor shell-first gedrag.
- Dat is precies de research gap van deze poster.

## 3) Onderzoeksvraag (15-20 sec)

Onderzoeksvraag:

"Wat verandert er op het device tussen SSR en CSR?"

Concreet bedoel ik: energie, timing en geheugen.

## 4) Methode (35-45 sec)

- Eén prototype, twee renderpaden.
- Zelfde storefront en interacties, alleen de verwerkingslocatie verandert.
- 60 valide runs op een Samsung Galaxy A53.
- Drie workloads: 72, 6.000 en 24.000 records.
- Analyse met median/IQR en U-test met Bonferroni-correctie.

## 5) Resultaten (60-75 sec)

Kernpatroon:

- SSR gebruikte in alle drie scenario's minder totale client-side energie dan CSR.

Concreet voorbeeld:

- In het massieve scenario zat SSR op 5770 mJ en CSR op 9048 mJ.
- Dus: dezelfde app kan functioneel gelijk zijn, maar toch veel meer van de telefoon vragen.

Nuance:

- In de dynamische workload voelde CSR eerder snel.
- Maar over de hele run bleef SSR zuiniger.

Zin voor niet-technisch publiek:

"Sneller voelen en zuiniger zijn zijn niet automatisch hetzelfde."

## 6) Conclusie + aanbevelingen (45-60 sec)

Conclusie:

- Binnen deze benchmark is SSR de beste default wanneer batterij en device-cost centraal staan.

Aanbevelingen:

- Kies SSR als energie-efficiency belangrijk is of workloads zwaarder worden.
- Kies CSR als shell-first gedrag belangrijker is.
- Maak renderkeuzes niet op gevoel alleen; combineer energie, geheugen en timing.

## 7) Vervolg + discussie openen (20-30 sec)

Vervolg:

- Herhaal op meerdere devices en netwerken.
- Voeg waar mogelijk hardware-based power measurement toe.

Discussievraag:

"Wanneer kies jij voor een snellere eerste indruk, ook als de totale device-cost hoger uitvalt?"

## Korte Q&A cheat sheet

Vraag: "Is SSR dan altijd beter?"
Antwoord: Nee. Voor vroege perceptie kan CSR aantrekkelijk zijn. Het hangt af van je prioriteit.

Vraag: "Waarom meten op een echt toestel?"
Antwoord: Omdat energie- en geheugengedrag op echte hardware vaak anders uitvalt dan op desktop of in een lab.

Vraag: "Wat is je belangrijkste praktijkadvies?"
Antwoord: Meet niet alleen perceived speed. Meet ook wat het device echt kost.
