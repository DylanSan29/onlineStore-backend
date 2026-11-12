# Email verification API

- `POST /api/auth/email-verification/request` starts verification without revealing whether a username exists.
- `POST /api/auth/email-verification/confirm` consumes a 24-hour, single-use token.

The current `User` model has no email or verified flag, so this branch prepares the secure token workflow. A future email-provider integration should deliver the token and add an explicit `Email`/`EmailVerifiedAt` field before enforcing verification during login or checkout.
