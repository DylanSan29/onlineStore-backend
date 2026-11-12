# Payment integration skeleton

Endpoints:

- `POST /api/payments/orders/{orderId}/checkout` validates order ownership and prepares a provider checkout response.
- `GET /api/payments/orders/{orderId}/status` reserves the status contract for the provider integration.
- `POST /api/payments/webhooks/mercadopago` reserves the webhook contract.

Mercado Pago credentials are configuration placeholders only. `Enabled` is false by default, no external API call is made, and the webhook does not process events yet. Before production use, add the official provider SDK/API adapter, server-side signature verification, idempotency keys, payment persistence, and order-status updates. Store `AccessToken` and `WebhookSecret` in environment variables or a secret manager, never in source control.
