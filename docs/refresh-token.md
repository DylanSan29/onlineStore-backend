# Refresh tokens

## Endpoints

- `POST /api/auth/login` validates credentials and returns a short-lived access token plus a refresh token.
- `POST /api/auth/refresh` accepts the refresh token and returns a new access/refresh token pair.
- `POST /api/auth/logout` revokes the supplied refresh token for the authenticated user.

## Security decisions

Refresh tokens are opaque random values. The API stores only a SHA-256 hash, never the usable token. Refreshing rotates the token and revokes the previous one, so a reused token is rejected. Access tokens expire after 15 minutes by default; refresh tokens expire after 7 days. These values are configurable under `Jwt`.

For production, the frontend should keep the refresh token in a secure, `HttpOnly`, `Secure`, `SameSite` cookie rather than local storage.
