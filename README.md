# URL Shortener

A simple URL shortener built with .NET, Redis, PostgreSQL, and Kubernetes.

## Stack

- **Backend** — ASP.NET Core minimal API (.NET 10)
- **Cache** — Redis via `IDistributedCache`
- **Database** — PostgreSQL with Entity Framework Core
- **Frontend** — Plain HTML + JavaScript
- **Container orchestration** — Kubernetes (minikube for local development)

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/)
- [minikube](https://minikube.sigs.k8s.io/)
- [kubectl](https://kubernetes.io/docs/tasks/tools/)

## Running locally with Docker Compose

1. Copy the environment file and fill in your values:
   ```bash
   cp .env.example .env
   ```

2. Start Redis and PostgreSQL:
   ```bash
   docker compose up -d
   ```

3. Set up user secrets for the database connection string:
   ```bash
   dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Database=urlshortener;Username=<user>;Password=<password>"
   ```

4. Run the app:
   ```bash
   dotnet watch
   ```

5. Open `http://localhost:5292` in your browser.

## Running on Kubernetes (minikube)

1. Start minikube:
   ```bash
   minikube start
   ```

2. Point your terminal to minikube's Docker:
   ```bash
   eval $(minikube docker-env)
   ```

3. Build the Docker image:
   ```bash
   docker build -t url-shortener:latest .
   ```

4. Create your secrets file from the example and fill in base64 encoded values:
   ```bash
   cp secret.example.yaml k8s/secret.yaml
   # encode values with: echo -n "yourvalue" | base64
   ```

5. Apply all Kubernetes config:
   ```bash
   kubectl apply -f k8s/
   ```

6. Get the app URL:
   ```bash
   minikube service app-service --url
   ```

## Project structure

```
url-shortener/
├── Api/
│   └── UrlEndpoints.cs     # API routes
├── Data/
│   ├── AppDbContext.cs      # EF Core database context
│   └── ShortenedUrl.cs      # Database model
├── Migrations/              # EF Core migrations
├── wwwroot/
│   └── index.html           # Frontend
├── k8s/                     # Kubernetes config files
├── Program.cs               # App setup and configuration
├── Dockerfile
└── docker-compose.yml
```
