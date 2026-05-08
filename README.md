# Storygame

A reading tracker built as a sandbox for exploring architectural and infrastructure needs. Users register, browse a catalog, add books to their library, and track reading progress across ebooks, paperbacks, and audiobooks.

## What this project is for

Storygame is intentionally a real, working application — not a tutorial or a minimal example. The goal isn't the reading tracker itself; it's the system around it.

Building good architectural tools — a custom message queue, an event store, a cache layer, a request throttler — only makes sense once you have something that genuinely needs them. Without a real workload, design decisions are speculation. Storygame is the workload: enough domain, enough flows, and enough moving parts to surface concrete problems that justify concrete solutions.

When something hurts here — a slow query, a fragile coordination point, a missing observability hook — that pain becomes the brief for the next library or service to build.

## What it isn't

- Not a polished consumer product. UI gaps, hardcoded data, and TODO comments are intentional; smoothing them over too early hides the problems worth solving.
- Not a reference implementation. Some choices are deliberately naive so they can be replaced later (in-memory session storage, in-memory catalog, an in-process event dispatcher).
- Not stable. APIs, schemas, and project layout change as the architecture evolves.

## Features

The application currently supports:

- **Passwordless authentication** — registration with email verification, then login via one-time codes. Emails are stored in an in-process inbox you can read at `/mail` instead of being sent out.
- **Session management** — server-side session store with logout, expiration, user-agent binding, and CSRF protection on top of cookie auth.
- **Book catalog** — a fixed catalog of titles, each available as text edition, audiobook, or both. Filterable by title and format.
- **Personal library** — users add catalog books or custom books in a chosen format (ebook, paperback, audiobook).
- **Progress tracking** — start tracking a book, update the current page or minute, see percent complete and finished state.
- **Statistics projections** — every progress update fans out to daily, weekly, monthly, and yearly counters via event handlers.
- **Traffic simulator** — a console app that spins up multiple user scenarios (binge reader, casual sampler, abandoner, audiobook listener) against a running instance.

## Tech stack

**Backend** — ASP.NET Core 10, MongoDB, Serilog. Hand-rolled CQRS with a dispatcher and Scrutor-based handler registration. Events are dispatched in-process to multiple handlers (for example, the statistics projectors).

**Frontend** — React 19 with React Router 7 in SPA mode, TypeScript, Tailwind CSS 4.

**Infrastructure** — Docker Compose runs MongoDB, the backend, the frontend, and an nginx reverse proxy that terminates TLS and routes `/api/*` to the backend.

## Architecture at a glance

The solution is split into small projects by responsibility:

- `Storygame.Catalog`, `Storygame.Library`, `Storygame.Tracking`, `Storygame.Users` — domain logic, each with its commands, queries, and events.
- `Storygame.Cqrs` — dispatcher, command/query/event interfaces.
- `Storygame.Storage` — MongoDB repositories implementing domain interfaces.
- `Storygame.Web` — HTTP endpoints, auth, rate limiting.
- `Storygame.Contracts.WebApi` — DTOs and request/response shapes.
- `Storygame.Client` — typed HTTP client used by the traffic simulator and integration tests.
- `Storygame.TrafficSimulator` — load-generating scenarios.

Tests live in `Storygame.Tests.Unit` (handlers in isolation, mocked repositories) and `Storygame.Tests.Integration` (end-to-end through the typed client against a `WebApplicationFactory`).

## Running with Docker

This is the recommended way to run the full stack — four containers behind nginx with HTTPS.

### Prerequisites

- Docker and Docker Compose
- `openssl` (ships with Git for Windows, macOS, and most Linux distributions)

### 1. Generate a self-signed TLS certificate

The app uses HTTPS-only cookies, so the proxy needs a cert. Run this once from the project root:

```bash
mkdir certs
openssl req -x509 -newkey rsa:4096 \
  -keyout certs/key.pem \
  -out certs/cert.pem \
  -days 365 \
  -nodes \
  -subj "/CN=localhost"
```

### 2. Start the containers

```bash
docker compose up --build
```

The first build pulls the .NET SDK image and installs npm packages — a few minutes. Subsequent builds are fast.

### 3. Open the app

Go to `https://localhost`. Accept the certificate warning (in Chrome, type `thisisunsafe` anywhere on the warning page to bypass it).

### 4. Stop

```bash
docker compose down
```

MongoDB data lives in a named volume (`mongodb_data`) and survives restarts. To wipe it:

```bash
docker compose down -v
```

## Local development without Docker

Useful when iterating on backend or frontend code without rebuilding containers.

### Required tools

- .NET 10 SDK
- Node.js 20+
- MongoDB running locally on `localhost:27017` (Docker works: `docker run -p 27017:27017 mongo:8`)

### Backend

```bash
cd src/Storygame.Web
dotnet run
```

The API serves on `http://localhost:5263`.

### Frontend

```bash
cd src/client
npm install
npm run dev
```

The dev server runs on `http://localhost:5173` and proxies `/api/*` to the backend.

### Reading dev emails

In Development mode, all "sent" emails are kept in memory and exposed at `GET /api/mail/{email}`. The frontend has a UI for this at `/mail`. Use it to grab verification codes and login confirmation keys.

### Running tests

```bash
dotnet test src/Storygame.Tests.Unit/Storygame.Tests.Unit.csproj
dotnet test src/Storygame.Tests.Integration/Storygame.Tests.Integration.csproj
```

The same tests run on every push and pull request via GitHub Actions (`.github/workflows/dotnet.yml`).

### Generating traffic

With the backend running on HTTPS (default `https://localhost:7121`), run:

```bash
cd src/Storygame.TrafficSimulator
dotnet run
```

The simulator registers fake users and runs a mix of reading scenarios against the API.

## Recommended editor tools

- An IDE with C# support: Visual Studio, Rider, or VS Code with the C# Dev Kit.
- Node.js for the frontend toolchain (Vite, TypeScript, Tailwind).
- Optional: MongoDB Compass or `mongosh` for inspecting the database.

## Contributing

Issues and pull requests are welcome. Because the project's purpose is exploring architectural needs rather than shipping features, the most valuable contributions are usually:

- Identifying friction points or scaling concerns in the current code.
- Proposing or prototyping infrastructure pieces (cache, queue, event store, observability) that the app could plausibly use.
- Adding scenarios to the traffic simulator that stress new parts of the system.

Feature work on the reading-tracker side is welcome too, as long as it gives the architecture something concrete to react to.
