# StorePilot

A desktop-oriented store management system designed for local, single-store operation. Built with React, TypeScript, ASP.NET Core Web API, Entity Framework Core, and SQLite.

StorePilot aims to provide a practical solution for managing the daily operations of a retail store, including inventory, sales, purchases, and financial transactions.

> **Project status:** Under active development. The initial application foundation and startup flow are being developed. Core business modules are not yet complete.

## Overview

StorePilot is designed for a single store running locally on one computer.

The planned deployment model consists of:
- A React-based user interface.
- An ASP.NET Core Web API running locally.
- A local SQLite database.
- A desktop application shell using WebView2 to host the frontend.

The system is initially designed for a single store and a single local operator. Multi-user access, multiple branches, and remote hosting are outside the current scope.

## Technology Stack

### Frontend
- React
- TypeScript
- Vite

### Backend
- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core

### Database
- SQLite

### Testing
- xUnit
- Integration testing

## Planned Features

| Module | Status |
|---|---|
| Application startup and health checks | In progress |
| Initial store setup and branding | In progress |
| Dashboard | Planned |
| Products and inventory | Planned |
| Categories and units | Planned |
| Sales and sales returns | Planned |
| Purchases and purchase returns | Planned |
| Customer accounts and payments | Planned |
| Cashbox and financial transactions | Planned |
| Printing and reports | Planned |
| Desktop packaging and installation | Planned |

Feature status will be updated as development progresses.

## Architecture

The backend follows a layered architecture:

- **Domain:** Core business concepts and rules.
- **Application:** Use cases, contracts, and application-level abstractions.
- **Infrastructure:** Database access, persistence, and external implementation details.
- **API:** HTTP endpoints and application entry point.

The frontend is maintained separately from the backend and communicates with it through HTTP API endpoints.

## Repository Structure

```text
StorePilot/
├── src/
│   ├── frontend/
│   │   └── storepilot-web/
│   └── backend/
│       ├── StorePilot.API/
│       ├── StorePilot.Application/
│       ├── StorePilot.Domain/
│       ├── StorePilot.Infrastructure/
│       ├── StorePilot.UnitTests/
│       ├── StorePilot.IntegrationTests/
│       └── backend.slnx
│
├── desktop/
├── docs/
├── tests/
├── .gitignore
└── README.md
```

## Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js (LTS)
- npm

### Backend

From the repository root:

```bash
dotnet restore src/backend/backend.slnx
dotnet run --project src/backend/StorePilot.API/StorePilot.API.csproj
```

The API is configured to run locally. During development, the frontend uses a Vite proxy to communicate with the API.

### Frontend

Open a separate terminal:

```bash
cd src/frontend/storepilot-web
npm install
npm run dev
```

Vite starts the frontend development server. The API proxy target is configured in `vite.config.ts`.

> The API address and project launch configuration may vary by development environment. Check the backend launch settings and Vite proxy configuration if the default local address is unavailable.

## Testing

Run backend tests from the repository root:

```bash
dotnet test src/backend/backend.slnx
```

Frontend build:

```bash
cd src/frontend/storepilot-web
npm run build
```

## Local Data

Application data is stored locally on the machine running StorePilot.

The planned local data structure includes:
- SQLite database files.
- Application settings.
- Branding assets.
- Application logs and backups.

Runtime data and customer information should not be committed to this repository.

## Development Notes

StorePilot is being developed incrementally. Architecture, implementation details, and feature status may evolve as the system grows.

The repository is intended to document the development of the application and its implementation decisions.

## License

No open-source license has been assigned yet. All rights reserved unless otherwise stated.