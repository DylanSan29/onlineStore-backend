# Security hardening

- Authentication endpoints are limited to 10 requests per minute per client IP.
- Logout revokes only refresh tokens owned by the authenticated user.
- Unexpected exceptions are logged server-side and return a generic `ProblemDetails` response.
- Build output and local configuration files are excluded from Git.

Production deployment should still provide secrets through environment variables or a secret manager, use HTTPS, and configure a distributed rate-limit store when running multiple instances.
