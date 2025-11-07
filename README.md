# UserManagementAPI

Simple ASP.NET Core Web API for user management with CRUD, validation, middleware, and in-memory repository.

How to run:
1. dotnet build
2. dotnet run --project UserManagementAPI

Default demo token for requests:
Authorization: Bearer demo-token-123

Endpoints:
- GET /api/users
- GET /api/users/{id}
- POST /api/users
- PUT /api/users/{id}
- DELETE /api/users/{id}

Validation: DTOs use DataAnnotations (Required, EmailAddress, MaxLength).
Middleware: GlobalExceptionMiddleware, SimpleTokenAuthMiddleware, RequestLoggingMiddleware.

How Copilot was used:
- Scaffolding Program.cs and controllers
- Suggesting DataAnnotations
- Recommending global exception middleware and request logging
