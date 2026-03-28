# SvigermorApi

A lightweight REST API that receives a cold or neutral string and returns a warm, heartfelt Danish message fit for a mother-in-law. Transformation is powered by Google Gemini 2.0 Flash.

## Tech stack

- ASP.NET Core (.NET 9) — Minimal APIs
- Google Gemini 2.0 Flash (via HTTP)
- Docker

## Project structure

```
SvigermorApi.Core/   # Domain models and use case interfaces
SvigermorApi.Api/    # Entry point, endpoints, DI wiring, Gemini HTTP client
```

## Running locally

### With Docker

```zsh
cp docker-compose.override.yml.example docker-compose.override.yml
# Add your GEMINI_API_KEY to docker-compose.override.yml
docker compose up --build
```

### Without Docker

```zsh
export GEMINI_API_KEY=your-api-key-here
dotnet run --project SvigermorApi.Api
```

## Environment variables

| Variable         | Description              |
|-----------------|--------------------------|
| `GEMINI_API_KEY` | Your Gemini API key      |

> `docker-compose.override.yml` is gitignored — never commit secrets.
