# Notifications API

- `GET /api/notifications` lists the authenticated user's notifications.
- `GET /api/notifications/unread-count` returns the unread count.
- `PATCH /api/notifications/{id}/read` marks one notification as read.
- `PATCH /api/notifications/read-all` marks all notifications as read.

Every query is scoped by the JWT user ID, so a customer cannot read or update another customer's notifications.

Administrators can create a notification for a user with `POST /api/admin/notifications/users/{userId}`. Order creation, shipment updates, and payment webhooks create notifications through the service layer.
