# StorePilot

A local retail management system designed for single-store operation, built with React, TypeScript, ASP.NET Core Web API, Entity Framework Core, and SQLite.

**Project Status:** Under Active Development

## Current Implementation

- ASP.NET Core Web API foundation.
- Local SQLite database with EF Core migrations.
- Application startup and database initialization.
- Health checks and setup status endpoints.
- Initial store setup and settings management.
- Store logo upload and storage.
- Centralized exception handling.
- Structured logging with Serilog.

## Technology Stack

**Frontend**
- React
- TypeScript
- Vite

**Backend**
- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core

**Database**
- SQLite

## Architecture

The backend is organized into:

- **Domain:** Core business entities.
- **Application:** Application contracts and use cases.
- **Infrastructure:** Persistence and external implementations.
- **Api:** HTTP endpoints and application startup.

## Deployment Direction

The target deployment model is a locally running API with a local SQLite database and a React interface hosted through a desktop shell.

Core retail workflows and desktop packaging are still under development.