# LU2 - Minimal Viable Product

This repository contains two core projects developed for the LU2 course in the 2D Graphics module:

- **CoreGame**  
  A Unity-based project containing game logic, scenes, and assets. It leverages Unity’s systems (including the input system and visual scripting features) to deliver an interactive 2D experience.

- **CoreLink**  
  A .NET solution comprised of several projects, including a web API and unit tests. CoreLink demonstrates minimal viable product (MVP) patterns and serves as a backbone for linking your application components.

---

## CoreGame

**Location:** [CoreGame](CoreGame/CoreGame.sln)

### Overview

CoreGame is the Unity game project. It includes:

- Game logic and behaviors in [Assets/Code/CoreLinkMethodes.cs](CoreGame/Assets/Code/CoreLinkMethodes.cs)
- Scenes under the [Assets/Scenes](CoreGame/Assets/Scenes) directory.
- Integrated packages such as the Input System and Visual Scripting (see packages in the [Library/PackageCache](CoreGame/Library/PackageCache) folder).

### Getting Started

1. Open the project in Unity.
2. Configure input settings via **Edit > Project Settings > Input System Package** if needed.
3. Run the scene from the Unity Editor to test the game functionalities.

---

## CoreLink

**Location:** [CoreLink](CoreLink/CoreLink.sln)

### Overview

CoreLink contains several projects:

- **CoreLink.WebApi** – A .NET web API project.
- **CoreLink.Tests** – A project containing unit tests that ensure the correct behavior of your application core.
- Additional supporting projects are available in the solution to manage the linking logic.

### Getting Started

1. Open the solution in Visual Studio or another .NET IDE.
2. To run the web API, build the [CoreLink.WebApi](CoreLink/CoreLink.WebApi/CoreLink.WebApi.csproj) project and start debugging.
3. Execute tests in [CoreLink.Tests](CoreLink/CoreLink.Tests/CoreLink.Tests.csproj) using the integrated test runner in your IDE.

---

## Additional Information

- **Documentation:** Detailed project documentation may be available in the [Documentation](CoreGame/Library/PackageCache/com.unity.visualscripting@1b53f46e931b/Editor/VisualScripting.Core/Documentation/DocumentationGenerator.cs) (for CoreGame) and in various markdown files under the package caches (e.g. [ProjectWideActions.md](CoreGame/Library/PackageCache/com.unity.inputsystem@920b46832575/Documentation~/ProjectWideActions.md)).
- **Build Instructions:**  
  For CoreLink, execute the build script specified in [package.json](package.json) or use your IDE’s build functionality.
- **Contributing:**  
  Please see each project’s guidelines for contributing to better understand the coding practices and setup instructions.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For questions or feedback, please contact [dcj.vanginneken@student.avans.nl](mailto:dcj.vanginneken@student.avans.nl).
