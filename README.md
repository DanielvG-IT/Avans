<div align="center">

# Avans University — CS Portfolio

**Daniël van Ginneken**  
Bachelor of Science in Computer Science  
Avans University of Applied Sciences, Breda · 2024 – present

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.md)
[![Commit Activity](https://img.shields.io/github/commit-activity/m/DanielvG-IT/Avans)](https://github.com/DanielvG-IT/Avans/commits/main)
[![Last Commit](https://img.shields.io/github/last-commit/DanielvG-IT/Avans)](https://github.com/DanielvG-IT/Avans/commits/main)
[![Languages](https://img.shields.io/github/languages/count/DanielvG-IT/Avans)](https://github.com/DanielvG-IT/Avans)

</div>

---

## Table of Contents

- [About](#about)
- [Projects Overview](#projects-overview)
- [Year 1 Projects](#year-1-projects)
  - [L1Q1 — Smart Energy](#l1q1--smart-energy)
  - [L1Q2 — Robotics Prototyping](#l1q2--robotics-prototyping)
  - [L1Q3 — 2D Graphics & Secure Communication](#l1q3--2d-graphics--secure-communication)
  - [L1Q4 — Artificial Intelligence & Data Science](#l1q4--artificial-intelligence--data-science)
- [Year 2 Projects](#year-2-projects)
  - [L2Q3 — Performance Efficiency Research](#l2q3--performance-efficiency-research)
  - [L2S1 — KeuzeKompas Study Guidance System](#l2s1--keuzekompass-study-guidance-system)
- [Year 3](#year-3)
- [Year 4](#year-4)
- [Tech Stack Summary](#tech-stack-summary)
- [Getting Started](#getting-started)
- [License](#license)

---

## About

This repository is a living portfolio of every project I have built during my Computer Science degree at Avans University of Applied Sciences. It spans IoT systems, robotics, game development, full-stack web applications, machine learning pipelines, and published research — each project representing a distinct engineering challenge that pushed me to learn something new.

The progression from year one to year two mirrors my own growth: early projects focus on getting things to work, later projects focus on getting things to work *well* — with proper architecture, rigorous testing, and production-ready deployment.

---

## Projects Overview

| Year | Quarter / Semester | Project Name | Tech Highlights | Status |
|------|--------------------|--------------|----------------|--------|
| Year 1 | Q1 | Smart Energy | C#, Blazor, .NET, IoT | ✅ Completed |
| Year 1 | Q2 | Robotics Prototyping | C#, .NET, MQTT, Arduino | ✅ Completed |
| Year 1 | Q3 | 2D Graphics & Secure Communication | Unity, C#, .NET WebAPI | ✅ Completed |
| Year 1 | Q4 | AI & Data Science | Python, FastAPI, Blazor, YOLO | ✅ Completed |
| Year 2 | Q3 | Performance Efficiency Research | Next.js, TypeScript, Python, LaTeX | ✅ Completed |
| Year 2 | S1 | KeuzeKompas Study Guidance | React, NestJS, FastAPI, scikit-learn | ✅ Completed |
| Year 3 | — | Upcoming modules | — | ⏳ Pending |
| Year 4 | — | Upcoming modules | — | ⏳ Pending |

---

## Year 1 Projects

### L1Q1 — Smart Energy

`C#` `Blazor` `.NET` `IoT` `Web`

Smart Energy is an end-to-end energy monitoring platform built around a physical smart meter gateway that I soldered, flashed, and installed in my own meter cabinet. The gateway reads electricity and gas consumption every five minutes and pushes the data to a central server, where a Blazor web application renders real-time charts and historical trends.

The project taught me how hardware and software meet: reading P1 telegrams from a Dutch smart meter, handling serial communication reliably, and designing a UI that stays useful even when data arrives at irregular intervals. By making my own home's energy consumption visible, the system provides the kind of actionable insight that underpins the energy transition — you cannot reduce what you cannot see.

**Key technical decisions:**
- Chose Blazor Server for the dashboard to keep the real-time data flow simple (server push over WebSockets rather than polling from the browser)
- Modelled measurement data in a separate `SmartEnergy.Library` class library so the gateway and the web frontend share the same domain types without duplication
- Hardware prototyping done with off-the-shelf IoT modules, connected and soldered independently

---

### L1Q2 — Robotics Prototyping

`C#` `.NET` `MQTT` `Arduino` `Raspberry Pi` `Blazor` `SQL`

CropBotics is an autonomous plant-health monitoring robot built on a Raspberry Pi / Arduino stack. The robot drives alongside a row of plants, samples each one with a colour sensor, classifies health status by pixel analysis, and avoids obstacles — all while streaming live telemetry to a Blazor web dashboard called FarmView via MQTT.

The project required integrating three distinct domains — low-level embedded C for the Arduino, higher-level .NET business logic on the Raspberry Pi, and a web frontend — while keeping them loosely coupled so each component can be tested and updated independently. FarmView stores historical sensor readings in a SQL database, enabling trend analysis beyond what a live feed alone can provide.

**Key technical decisions:**
- MQTT as the communication backbone: lightweight, asynchronous, and broker-mediated so the robot and dashboard never need a direct connection
- Clean interface segregation (`IInitializable`, `IUpdatable`, `IWaitable`, `IMessageHandler`) on the robot side, making each subsystem (drive, detection, comms, alerts) independently swappable
- Blazor Server dashboard with SQL persistence for historical trend queries and real-time display

---

### L1Q3 — 2D Graphics & Secure Communication

`Unity` `C#` `.NET WebAPI` `TLS` `SQL`

This module produced two complementary deliverables: **HealthQuest**, a Unity 2D game developed with a team of seven for a real hospital client, and **CoreLink**, a solo minimum-viable-product combining a Unity game engine with a .NET backend.

HealthQuest simulates a child-friendly virtual hospital to reduce anxiety in children before actual medical procedures. It pairs immersive 2D animations with a .NET Web API backend that tracks user progress and session data under TLS encryption — because the users are minors and the data is sensitive. This was my first experience delivering software for a real stakeholder with real privacy requirements, and it shaped how I think about security by default rather than security as an afterthought.

CoreLink demonstrated the same front-to-back architecture in a solo setting, with the addition of a dedicated unit test suite (`CoreLink.Tests`), proving the pattern was repeatable and not just a team effort.

**Key technical decisions:**
- TLS-encrypted API communication from the first commit — not retrofitted at the end
- Backend separated from the Unity project into its own .NET solution so it could be deployed independently and tested without a game engine
- Unity Input System (new) used over legacy input for forward compatibility
- Role-based access enforced at the API layer, not just hidden in the frontend

---

### L1Q4 — Artificial Intelligence & Data Science

`Python` `FastAPI` `C#` `Blazor` `.NET WebAPI` `scikit-learn` `YOLO` `Jupyter`

The AI & Data Science module split into two parallel tracks. **TralaAI** is a three-tier production system: a Blazor web frontend, a .NET REST API, and a Python FastAPI service that hosts the machine-learning models. It demonstrated that ML inference belongs in its own deployable service — keeping the Python ecosystem isolated while the .NET layer handles orchestration and business rules.

The **Data Science** curriculum ran alongside it as a structured Jupyter notebook series covering the full supervised-learning pipeline: univariate and bivariate analysis, decision trees, random forests, neural networks, and object detection with YOLO on a traffic-sign dataset. Working with real-world datasets (financial fraud detection, gym member tracking, life expectancy) made the statistical theory concrete.

**Key technical decisions:**
- FastAPI chosen for the ML backend over integrating Python into .NET: keeps dependencies clean, enables independent scaling, and lets the model code live in its native ecosystem
- TralaAI frontend containerised with Docker from the start so deployment is not an afterthought
- YOLO v8 and v11 pre-trained weights used for transfer learning on the traffic sign dataset rather than training from scratch — a practical engineering choice given training time constraints
- Separate `.NET ApiTest` project for automated endpoint validation independent of the Blazor frontend

---

## Year 2 Projects

### L2Q3 — Performance Efficiency Research

`Next.js` `React` `TypeScript` `Tailwind CSS` `Python` `Node.js` `Puppeteer` `LaTeX` `Android`

This project is a full academic research study comparing Server-Side Rendering (SSR) against Client-Side Rendering (CSR) for performance and energy efficiency on mobile devices. A custom Next.js prototype with three route variants (`/ssr`, `/csr`, `/` with `static`, `dynamic`, and `massive` data scenarios) served as the controlled test subject. Automated Puppeteer scripts drove a physical Samsung Galaxy A53 through 30 benchmark runs per condition, capturing Web Vitals (LCP, FCP, TBT) alongside battery telemetry via Android's Perfetto tracing framework.

Python analysis scripts then extracted energy metrics from the raw traces, ran Mann-Whitney U statistical tests for significance, and generated box-plot visualisations — producing a complete, reproducible dataset. The findings were written up as a formal LaTeX paper and presented as a conference-style poster. This is the project that pushed me furthest into scientific method: designing controls, choosing the right non-parametric statistics, and writing up null and alternative hypotheses rather than just reporting which number was bigger.

**Key technical decisions:**
- Physical device testing rather than emulator: emulators do not model battery draw or thermal throttling realistically
- Mann-Whitney U over t-test: the performance distributions are non-normal, so a rank-based test is more appropriate
- Perfetto trace analysis for energy measurement: it is the only tool on Android that gives per-process power attribution at sub-second granularity
- Full raw data and scripts committed alongside the paper so results are independently reproducible

---

### L2S1 — KeuzeKompas Study Guidance System

`React` `Vite` `TypeScript` `NestJS` `Prisma` `FastAPI` `Python` `scikit-learn` `Docker` `MySQL`

KeuzeKompas (Compass) is a production-ready microservices application that helps Avans students discover study tracks and elective modules suited to their profile. The system is delivered across four progressive milestones: a JavaScript proof-of-concept, a TypeScript architectural refactor, a Python ML recommendation model, and a fully integrated three-service MVP called **CompassGPT**.

The ML service trained two recommendation strategies — a Bag-of-Words NLP model and a semantic-embedding model — against a cleaned dataset of student profiles and module descriptions. Evaluation notebooks (`4.1/4.2_evaluation_*.ipynb`) and optimisation notebooks (`5.1/5.2_optimization_*.ipynb`) compared both approaches before the better-performing model was deployed into the FastAPI inference service. The React frontend, NestJS API, and Python AI service are all independently containerised, communicate over HTTP, and can be scaled or replaced without touching the others.

**Key technical decisions:**
- NestJS for the backend: its module system enforces separation of concerns at the framework level, which matters when multiple developers are contributing
- Prisma ORM with a typed schema so database changes propagate as TypeScript compile errors rather than runtime surprises
- Two NLP strategies evaluated in parallel before committing to one, with a dedicated comparison notebook (`6_comparison.ipynb`) making the decision transparent
- Cypress E2E tests in the PoC stage validated the full user journey before moving to the TypeScript rewrite
- Docker Compose ties all three services together for one-command local development

---

## Year 3

Year 3 modules are scheduled to begin in the 2026–2027 academic year. This section will be populated as projects are completed. Planned areas include advanced software architecture, distributed systems, and professional internship work.

---

## Year 4

Year 4 is the final year of the programme, focused on a graduation internship and thesis. Projects and research will appear here once work begins in 2027–2028.

---

## Tech Stack Summary

| Domain | Technologies Used |
|--------|-------------------|
| **Languages** | C#, TypeScript, JavaScript, Python |
| **Frontend** | Blazor Server, React 19 + Vite, Unity (C#) |
| **Backend** | .NET WebAPI, NestJS, FastAPI, Express |
| **Databases** | SQL Server, MySQL, Prisma ORM |
| **ML / Data Science** | scikit-learn, Pandas, YOLO v8/v11, Jupyter, Puppeteer |
| **Messaging** | MQTT |
| **DevOps** | Docker, Docker Compose, GitHub Actions |
| **Hardware** | Raspberry Pi, Arduino, Samsung Galaxy A53 (Android) |
| **Testing** | xUnit (.NET), Cypress (E2E), statistical benchmarking |
| **Research** | LaTeX, BibTeX, Perfetto (Android tracing), Mann-Whitney U |

---

## Getting Started

```sh
git clone https://github.com/DanielvG-IT/Avans.git
cd Avans
```

Each project lives in its own folder and has its own dependency chain. Navigate to the project you want and follow its local README for setup instructions.

```
Avans/
├── Leerjaar 1/
│   ├── Q1 Smart Energy/
│   │   ├── LU1 Smart Energy/          # Blazor + IoT gateway
│   │   └── LU2 Software Ontwikkelen/  # C# fundamentals exercises
│   ├── Q2 Robotics Prototyping/
│   │   ├── CropBotics/                # Robot logic (.NET + MQTT)
│   │   └── FarmView/                  # Blazor dashboard + SQL
│   ├── Q3 2D Graphics & Secure Communication/
│   │   ├── LU1 HealthQuest/           # Unity game + .NET WebAPI
│   │   └── LU2 Minimal Viable Product/# CoreLink — Unity + .NET
│   └── Q4 Artificial Intelligence/
│       ├── LU1 TralaAI/               # FastAPI ML + Blazor + .NET API
│       └── LU2 Data Science/          # Jupyter notebooks + datasets
├── Leerjaar 2/
│   ├── Q3 Performance Efficiency/
│   │   ├── prototype/                 # Next.js SSR/CSR test app
│   │   ├── experiment/                # Puppeteer + Android benchmark
│   │   ├── analysis/                  # Python stats + visualisation
│   │   └── paper/                     # LaTeX research paper
│   └── S1 KeuzeKompas/
│       ├── LU1 Proof of Concept/      # JS + TS PoC implementations
│       ├── LU2 Prototype/             # Python ML recommendation model
│       └── LU3 MVP/                   # React + NestJS + FastAPI
├── Leerjaar 3/                        # ⏳ Pending
└── Leerjaar 4/                        # ⏳ Pending
```

> Per-project README files contain environment requirements, environment variable templates, and run commands specific to each tech stack.

---

## License

This repository is licensed under the MIT License — see the [LICENSE](LICENSE.md) file for details. Avans University of Applied Sciences owns the rights to the projects in this repository. The code is provided as-is and is not guaranteed to work in all environments. Use at your own risk.

---

<div align="center">

Made by [Daniël van Ginneken](https://github.com/DanielvG-IT) · Avans University of Applied Sciences, Breda

</div>
