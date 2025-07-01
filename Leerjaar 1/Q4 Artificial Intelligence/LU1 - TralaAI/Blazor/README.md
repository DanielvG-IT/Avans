# Blazor

**Beschrijving**  
Een Blazor Server-app die de gebruiker interface biedt. Roept via een service de API aan om data op te halen en voorspellingen te tonen.

---

## Vereisten
- [.NET 9 SDK](https://dotnet.microsoft.com/download)

## Installatie & Configuratie
1. Clone de repo  
   ```bash
   git clone https://github.com/TralaAI/Blazor.git
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
