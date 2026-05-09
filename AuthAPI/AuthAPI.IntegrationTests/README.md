# AuthAPI Integration Tests

This project contains integration tests for `AuthAPI.Api` authentication and authorization flows.

## Covered scenarios

- `POST /users/login` success and failure paths
- `GET /users` unauthorized (no token)
- `GET /users` authorized admin access
- `GET /users` forbidden access for non-admin user
- `POST /users` authorized user creation
- `POST /users` unauthorized creation attempt

## Run tests

```powershell
cd AuthAPI/AuthAPI.IntegrationTests
dotnet test
```

## Notes

- Tests use a MySQL Testcontainer and run DB migrations/seed through app startup.
- Credentials used for admin login come from seed data:
  - `admin@email.com`
  - `Test1234!`

