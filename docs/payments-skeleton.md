# Payment integration skeleton

Endpoints:

- `POST /api/payments/orders/{orderId}/checkout` validates order ownership and prepares a provider checkout response.
- `GET /api/payments/orders/{orderId}/status` reserves the status contract for the provider integration.
- `POST /api/payments/webhooks/mercadopago` validates the configured webhook signature and updates payment/order status.

Mercado Pago credentials are configuration placeholders only. `Enabled` is false by default and no external API call is made. Payment records are persisted with idempotency keys. Before production use, replace the placeholder signature contract with Mercado Pago's official signature verification and SDK/API adapter. Store `AccessToken` and `WebhookSecret` in environment variables or a secret manager, never in source control.
