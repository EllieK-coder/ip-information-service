# IP Information Service

REST microservice that resolves IPv4 addresses to country information, keeps that data fresh through a scheduled job, and reports aggregate statistics per country.

Built as an integration on a clean-architecture skeleton for the **Novibet Academy — ExpressYourself** assignment.

---

## What it does

| | Endpoint / Trigger | Description |
|---|---|---|
| **Get IP Info** | `GET /ip/{address}` | Returns country info for an IPv4 address, resolved through a **cache → database → external service** fallback chain |
| **Refresh Job** | Quartz, hourly | Re-fetches country data for stored IPs in batches, updates only what changed, and invalidates the corresponding cache entries |
| **Get Report** | `GET /report` | Per-country address count and most recent update timestamp, optionally scoped to specific country codes |

Country data is sourced from [ip2c.org](https://ip2c.org), a free web service polled with one HTTP GET per address.

---

## Architecture

```
IPService (API)          Controllers · Quartz job · DI · GlobalExceptionHandler
      ↓
IPService.Application    CQRS handlers · interfaces · decorators · validation
      ↓                        ↑                        ↑
IPService.Domain          Infrastructure            Gateway
entities · exceptions      EF Core · caching        ACL for ip2c.org
```

**The Application layer defines the interfaces; Infrastructure and Gateway implement them.** Dependencies point inward — the Domain depends on nothing, and every handler can be tested with no database, no network and no HTTP.

### Key patterns

- **CQRS via MediatR** — controllers dispatch a command or query; they never reference a handler
- **Decorators for cross-cutting concerns** — caching and logging wrap handlers as generic decorators. A query opts into caching by implementing `ICacheableQuery`; the DI container resolves the chain from the generic constraint, so there is no conditional logic anywhere
- **Anti-Corruption Layer** — all IP2C parsing is isolated in `IPService.Gateway`. The raw `1;GR;GRC;Greece` format never propagates inward
- **Numeric IP key** — IPv4 addresses are stored as `long` (the bijective IPv4 ↔ uint32 mapping). This enables keyset pagination in the refresh job and pairs directly with IP2C's `?dec=` request form
- **Keyset cursor batching** — `WHERE AddressId > lastId ORDER BY AddressId TAKE N`, giving constant read cost and natural resumability
- **Resilience** — the IP2C client wraps retry (exponential backoff with jitter), circuit breaker and per-attempt timeout via `Microsoft.Extensions.Http.Resilience` / Polly v8

Full design rationale, use case flows and open trade-offs: **[docs/SRS_IP_Service.pdf](docs/SRS_IP_Service.pdf)**

---

## Tech stack

| | |
|---|---|
| Runtime | .NET 10 |
| API | ASP.NET Core Web API, Swagger |
| Mediation | MediatR |
| DI | Autofac |
| Persistence | EF Core, SQL Server |
| Caching | `IMemoryCache` behind an `ICache` abstraction |
| Scheduling | Quartz.NET |
| Resilience | `Microsoft.Extensions.Http.Resilience` (Polly v8) |
| Logging | NLog (coloured console + rolling file) |
| Testing | xUnit, Moq |

---

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB or full instance)
- `dotnet-ef` tools: `dotnet tool install --global dotnet-ef`

### Setup

```bash
git clone <repository-url>
cd ip-information-service
```

Adjust the connection string in `IPService/appsettings.json` if needed:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=IpFinderDb;Integrated Security=true;TrustServerCertificate=true;"
}
```

Apply migrations and run:

```bash
dotnet ef database update --project IPService.Infrastructure --startup-project IPService
dotnet run --project IPService
```

Swagger UI is available at the root URL in development.

> The refresh job fires immediately on startup and then hourly.

---

## API

### Get country for an IP

```http
GET /ip/192.168.1.1
```

```json
{
  "twoLetterCode": "GR",
  "threeLetterCode": "GRC",
  "countryName": "Greece"
}
```

### Get report

```http
GET /report
GET /report?codes=US,GR
```

```json
[
  { "countryName": "Greece", "addressesCount": 4, "lastAddressUpdated": "2026-07-26T08:41:37.31" },
  { "countryName": "Italy",  "addressesCount": 3, "lastAddressUpdated": "2026-07-26T07:04:51.32" }
]
```

Omitting `codes` reports all countries.

### Error responses

Errors follow [RFC 7807 `ProblemDetails`](https://datatracker.ietf.org/doc/html/rfc7807).

| Status | When |
|---|---|
| `400` | Malformed IPv4 address, or an invalid two-letter country code |
| `502` | IP2C unreachable or returned an unexpected response |
| `500` | Unexpected error (details are logged, never returned to the caller) |

Ready-to-run requests: [`IPService/IPService.http`](IPService/IPService.http)

---

## Testing

```bash
dotnet test
```

~44 tests across every layer. All unit tests — handlers run against mocked interfaces with no database, network or HTTP, which is itself a demonstration that the dependency inversion holds.

| Area | Approach |
|---|---|
| Domain | IPv4 parsing and validation boundaries |
| Application | Handler behaviour, cache hit/miss, change detection |
| Gateway | Fake `HttpMessageHandler`, one test per IP2C status |
| Infrastructure | EF Core InMemory provider |
| API | Controller mapping and exception propagation |

---

## Project structure

```
IPService/                  API host — controllers, Quartz job, DI wiring
IPService.Application/      Commands, queries, handlers, decorators, interfaces
IPService.Domain/           Entities, enums, exception hierarchy
IPService.Infrastructure/   EF Core persistence, configurations, migrations, cache
IPService.Gateway/          IP2C HTTP client (Anti-Corruption Layer)
IPService.Tests/            xUnit test suite
docs/                       SRS, ERD, architecture diagrams
```

---

## Known limitations

Documented deliberately — these are trade-offs, not oversights.

- **Unknown IPs return `200` with a placeholder country** rather than `404`. The result is persisted so repeated lookups for the same unknown address don't hit IP2C again; the refresh job corrects the record if IP2C later resolves it. The cost is that the placeholder appears in the report.
- **The refresh job writes per changed IP**, not one transaction per batch. The batch-save path exists but is not wired up. The dominant cost is the external calls (IP2C has no bulk endpoint), not the database round trips.
- **The cache is in-process**, so invalidation does not propagate across instances. The `ICache` abstraction is the single point where a distributed cache would be introduced.
- **IPv4 only.** The numeric key representation does not extend to IPv6.
- **No authentication** is configured; it was out of scope for the assignment.

Further open questions are recorded per use case in the SRS.

---

## Documentation

| | |
|---|---|
| [Software Requirements Specification](docs/SRS_IP_Service.pdf) | Requirements, use case flows, design decisions, open questions |
