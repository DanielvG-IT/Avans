# HealthQuestBackend

## Overview
HealthQuestBackend powers the HealthQuest application, offering APIs and services to manage health-related data and operations.

## Project Structure
```plaintext
.gitignore
HealthQuestBackend.sln
LICENSE
README.md
.github/
    workflows/
        pipeline.yml
Docs/
    Design/
    Tests/
HQB.Tests/
    HQB.Tests.csproj
    Controllers/
    TestResults/
HQB.WebApi/
    appsettings.Development.json
    appsettings.json
    HQB.WebApi.csproj
    Program.cs
    Controllers/
    Interfaces/
    Models/
    Properties/
    Repositories/
    Services/
```

## Getting Started
Follow these steps to set up the HealthQuestBackend:

1. **Clone the repository:**
   ```sh
   git clone https://github.com/L1Q3-HealthQuest/HealthQuestBackend.git
   ```
2. **Navigate to the project directory:**
   ```sh
   cd HealthQuestBackend
   ```
3. **Build the solution:**
   ```sh
   dotnet build
   ```
4. **Run the application:**
   ```sh
   dotnet run --project HQB.WebApi/HQB.WebApi.csproj
   ```

## Support
If you encounter any issues, please open an issue on GitHub.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.