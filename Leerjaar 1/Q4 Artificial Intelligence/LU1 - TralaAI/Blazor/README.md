# blazor-ui

**Beschrijving**  
Een Blazor WebAssembly-app die de gebruiker interface biedt. Roept via een Razor-service de Core API aan om data op te halen en voorspellingen te tonen.

---

## Vereisten
- [.NET 7 SDK](https://dotnet.microsoft.com/download)
- Node.js (optioneel, voor bundling)

## Installatie & Configuratie
1. Clone de repo  
   ```bash
   git clone https://github.com/<jouw-org>/tralaAI-blazor-ui.git
   cd blazor-ui
   ```
   
2. Pas user secrets aan (Core API URL):
   ```json
   {
   "ApiBaseUrl": "http://localhost:5000/api"
   }
   ```

## Runnen
```bash
dotnet run
```
