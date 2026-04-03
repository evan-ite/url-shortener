# One-Day Learning Exercise: URL Shortener

The app: A simple URL shortener (like bit.ly) built with .NET, backed by Redis, deployed on Kubernetes.

## What the end product looks like

A minimal web app with two pages:
- **Home page** — a form where you paste a long URL and get back a short code (e.g. localhost/abc123)
- **Redirect** — visiting localhost/{code} redirects you to the original URL

That's it. No auth, no database, no fancy UI.

## How each technology is used

- **.NET** — ASP.NET Core minimal API that handles the two routes (POST /shorten and GET /{code})
- **Redis** — stores the mapping of shortCode → originalURL. This is Redis's sweet spot: simple key-value storage with optional TTL (expiry)
- **Kubernetes** — you run both the .NET app and Redis as separate deployments in a local cluster, and they talk to each other via a Kubernetes Service

## The Kubernetes setup you're aiming for

```
your cluster
├── deployment: url-shortener-api   (your .NET app, 1 pod)
├── service: url-shortener-service  (exposes the API, type: NodePort or LoadBalancer)
├── deployment: redis               (official redis image, 1 pod)
└── service: redis-service          (ClusterIP — only reachable inside the cluster)
```

Your .NET app connects to Redis using the hostname `redis-service` — Kubernetes DNS handles the rest.

## Hints

**Getting a local cluster running fast**
Use minikube or kind. Both spin up a local K8s cluster in minutes.

**Connecting .NET to Redis**
Use the StackExchange.Redis NuGet package. Your connection string will just be `redis-service:6379` when running inside the cluster.

```csharp
// Program.cs sketch
var redis = ConnectionMultiplexer.Connect("redis-service:6379");
var db = redis.GetDatabase();

app.MapPost("/shorten", (string url) => {
    var code = Guid.NewGuid().ToString()[..6];
    db.StringSet(code, url);
    return code;
});

app.MapGet("/{code}", (string code) => {
    var url = db.StringGet(code);
    return url.HasValue ? Results.Redirect(url!) : Results.NotFound();
});
```

**Deploying your .NET app to minikube**
You'll need to build a Docker image and make it available to your cluster. Look up: "minikube docker env" — it lets you build directly into minikube's image registry without pushing to Docker Hub.

## Useful docs

- ASP.NET Core minimal APIs
- StackExchange.Redis basics
- Kubernetes: connecting apps with services

## Stretch goals (if you finish early)

- Add a TTL to your Redis keys so short links expire after 24 hours (`db.StringSet(code, url, TimeSpan.FromDays(1))`)
- Scale your .NET deployment to 2 replicas and verify both pods handle requests
- Add a `/stats/{code}` endpoint that tracks how many times a link was clicked (hint: `db.StringIncrement`)
