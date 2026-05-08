# Cartographer Follow-up Fix List

> Generated from the codebase mapping pass on 2026-05-07.
> **All items implemented on 2026-05-07.**

## Priority 1 — likely correctness bugs

### 1. ~~`WebAPI/Endpoints.cs`: v2 `GET /books/{id}` returns wrong result~~ ✅ Fixed
- `bookDto != null` condition was inverted — corrected to `bookDto == null`.
- File: `WebAPI/Endpoints.cs`

### 2. ~~`BookRepository.AddAsync(BookDto)` returns null after commit~~ ✅ Fixed
- The method was committing the entity but always returning `null`; now returns the created `book`.
- File: `Infrastructure/Repositories/BookRepository.cs`

## Priority 2 — auth and API consistency

### 3. ~~`NotificationAPI`: authorization declared but auth middleware commented out~~ ✅ Fixed
- Enabled `UseAuthentication()` / `UseAuthorization()` in Program.cs pipeline.
- Enabled `AddJwtData(builder)` in the DI registration chain.
- Added `using Common.JwtHelperService` import.
- Files: `NotificationAPI/NotificationAPI.Api/Program.cs`

### 4. ~~`AuthAPI`: login credentials passed through query parameters~~ ✅ Fixed
- Created `AuthAPI.Application.DTOs.LoginRequest` record.
- Updated login endpoint to accept `[FromBody] LoginRequest` instead of `[FromQuery]` params.
- Files: `AuthAPI/AuthAPI.Application/DTOs/LoginRequest.cs`, `AuthAPI/AuthAPI.Api/Endpoints.cs`

## Priority 3 — async, configuration, and test robustness

### 5. ~~`NotificationDispatcher`: sync-over-async and unreachable code~~ ✅ Fixed
- Replaced `.Result` with `await GetUserFromApiAsync(...)`.
- Removed unreachable `throw` after `return null`; added meaningful log statement.
- File: `NotificationAPI/NotificationAPI.Infrastructure/Services/NotificationDispatcher.cs`

### 6. ~~Hard-coded localhost service URLs~~ ✅ Fixed
- `Infrastructure`: notification client URL now reads `NotificationApiSettings:BaseUrl` (fallback: `https://localhost:7019`).
- `NotificationAPI.Infrastructure`: auth client URL now reads `AuthApiSettings:BaseUrl` (fallback: `https://localhost:7219`).
- Config keys added to `WebAPI/appsettings.Development.json` and `NotificationAPI/NotificationAPI.Api/appsettings.json`.
- Files: `Infrastructure/InfrastructureDependencyInjection.cs`, `NotificationAPI/NotificationAPI.Infrastructure/InfrastructureDependencyInjection.cs`, appsettings files.

### 7. ~~Integration tests depend on a fixed local MySQL instance~~ ✅ Fixed
- Restored Testcontainers MySQL lifecycle: starts a fresh container on `InitializeAsync`, stops it on `DisposeAsync`.
- File: `IntegrationTests/Configuration/CustomWebApplicationFactory.cs`

## Priority 4 — maintenance and repo hygiene

### 8. ~~Runtime logs checked into the repository tree~~ ✅ Fixed
- Added ignore patterns for `WebAPI/Minimal_API_*.log`, `AuthAPI/AuthAPI.Api/AuthAPI_*.log`, `AuthAPI/AuthAPI.Api/Users_API_*.log`, and `NotificationAPI/NotificationAPI.Api/NotificationAPI_*.log`.
- File: `.gitignore`
- **Note**: existing tracked log files still in history; run `git rm --cached WebAPI/Minimal_API_*.log` etc. to remove them from the index without deleting from disk.

### 9. Generated migration files — no code change needed
- Files remain in place for schema history.
- Scanner skip rules already exclude them from future mapping passes.

## Related docs
- Main architecture map: [`CODEBASE_MAP.md`](CODEBASE_MAP.md)
- Condensed reader report: [`../subagent_report_1.md`](../subagent_report_1.md)
