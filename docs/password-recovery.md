# Password recovery API

- `POST /api/auth/password-reset/request` starts a password reset and always returns the same generic response.
- `POST /api/auth/password-reset/confirm` consumes a one-hour token and changes the password.

Tokens are random, stored only as hashes, expire after one hour, and are single-use. The request endpoint intentionally does not reveal whether a username exists. Email delivery is not included yet; the returned service result is intended for a future email provider integration and is never returned by the controller.
