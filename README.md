## Storygame

A reading tracker application. Backend: ASP.NET Core 10. Frontend: React 19 with React Router 7. Database: MongoDB.

---

## Running with Docker

The full stack runs as four containers: MongoDB, the .NET backend, the Node.js frontend, and an nginx reverse proxy. The proxy terminates TLS and routes `/api/*` to the backend and everything else to the frontend.

### Prerequisites

- Docker and Docker Compose installed
- `openssl` available on your machine (comes with Git for Windows, macOS, and most Linux distributions)

### 1. Generate a self-signed TLS certificate

The application uses HTTPS-only cookies, so the proxy must serve HTTPS. Run this once from the project root:

```bash
mkdir certs
openssl req -x509 -newkey rsa:4096 \
  -keyout certs/key.pem \
  -out certs/cert.pem \
  -days 365 \
  -nodes \
  -subj "/CN=localhost"
```

### 2. Build and start the containers

```bash
docker compose up --build
```

The first build takes a few minutes — the .NET SDK image and npm dependencies need to be downloaded.

### 3. Open the application

Navigate to `https://localhost` in your browser. You will get a certificate warning because the cert is self-signed — accept it to proceed.

To dismiss the warning in Chrome without clicking through: type `thisisunsafe` anywhere on the warning page.

### 4. Stopping

```bash
docker compose down
```

MongoDB data is stored in a named Docker volume (`mongodb_data`) and persists between restarts. To remove it as well:

```bash
docker compose down -v
```

---

## Local development (without Docker)

### Requirements

- .NET 10 SDK
- Node.js 20+
- MongoDB running on `localhost:27017`

### Backend

```bash
cd src/Storygame.Web
dotnet run
```

The API starts on `http://localhost:5263`.

### Frontend

```bash
cd src/client
npm install
npm run dev
```

The dev server starts on `http://localhost:5173` and proxies `/api/*` to the backend.
