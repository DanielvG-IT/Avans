# KeuzeKompas — LU1 Proof of Concept (JavaScript)

A small JavaScript proof-of-concept project created for Avans course work (LU1). This repository demonstrates a minimal, readable structure and a working local workflow to validate core ideas before expanding into a full application.

## Contents

-   Short description of intent
-   How to run locally
-   Project structure
-   Development notes and conventions
-   Contribution and license

## Status

Proof of concept — suitable for experimentation and classroom demonstration. Not production-ready.

## Features

-   Minimal, modular JavaScript code
-   Simple dev workflow to run and test ideas quickly
-   Clear project structure for students to extend

## Prerequisites

-   Node.js >= 16 (for local tooling, optional if using only a browser)
-   npm or yarn (optional, if using tooling)

## Getting started

1. Clone the repository
   git clone <repo-url>
2. Install dependencies (if package.json exists)
   npm install
3. Run a dev server (if configured)
   npm start
4. Open index.html in a browser if the project is static:
   open index.html or use a simple server:
   npx http-server . -c-1

Adjust the commands above to match scripts in package.json if present.

## Project structure (suggested)

-   README.md — this file
-   package.json — optional, dev scripts and deps
-   /src — source JavaScript modules
    -   index.js — app entrypoint
    -   utils/ — helper modules
-   /public or /static — static assets and index.html
-   /tests — unit or integration tests
-   /docs — notes and design sketches

Feel free to adapt structure to your needs. Keep logic modular and well-documented.

## Development notes

-   Use ES modules for clarity (import / export)
-   Keep DOM manipulation separate from business logic for easier testing
-   Add small unit tests for core functions
-   Commit messages: short, present-tense, prefixed with scope when useful (e.g., "ui: update header layout")

## Contributing

-   Create a branch per feature or bug: feature/<name> or fix/<issue>
-   Open a pull request with a short description of changes
-   Keep PRs focused and small for easier review

## License

MIT — include LICENSE file and add your name and year.

## Contact / Maintainer

Replace with your name and email in the repository header or LICENSE file.

Notes

-   This README is a template — tailor sections (scripts, commands, structure) to the actual project contents.
