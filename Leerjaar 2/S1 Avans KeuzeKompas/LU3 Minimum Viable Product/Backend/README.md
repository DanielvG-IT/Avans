# CompassGPT Backend

This is the backend API for the CompassGPT project, built with [NestJS](https://nestjs.com/).

## Features

- RESTful API for modules, users, authentication, and more
- Database integration (Prisma ORM)
- Modular architecture

## Requirements

- Node.js 18+
- npm
- (Optional) Docker

## Installation

1. Clone the repository:
   ```bash
   git clone <repo-url>
   cd Backend
   ```
2. Install dependencies:
   ```bash
   npm install
   ```

## Running Locally

### Start the development server

```bash
npm run start:dev
```

The server will start on the default port (see `main.ts`).

### Using Docker

Build and run the container:

```bash
docker build -t compassgpt-backend .
docker run -p 3000:3000 compassgpt-backend
```

## Configuration

- Environment variables can be set in a `.env` file (see documentation or ask the team for required variables).
- Database configuration is managed via Prisma.

## Project Structure

- `src/` – Source code
- `test/` – Tests
- `prisma/` – Prisma schema and migrations

## License

See [LICENSE](LICENSE).
