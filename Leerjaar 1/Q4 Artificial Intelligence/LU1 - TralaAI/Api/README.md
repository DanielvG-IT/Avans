# core-api

**Beschrijving**  
Deze ASP .NET Core Web API verwerkt inkomende requests van de Blazor-frontend, beheert de database (CRUD) en maakt HTTP-calls naar de Python FastAPI voor voorspellingsresultaten.

---

## Vereisten
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server database

## Installatie & Configuratie
1. Clone de repo  
   ```bash
   git clone https://github.com/TralaAI/core-api.git
   cd core-api
   ```
2. Pas user secrets aan:
   ```json
     {
      "ConnectionStrings": {
        "DefaultConnection": "Server=...;Database=...;User Id=...;Password=..."
      },
      "FastApi": {
        "BaseUrl": "http://localhost:8000"
      }
    }
   ```

## Runnen
   ```bash
dotnet build
dotnet run --urls http://localhost:5000

   ```
