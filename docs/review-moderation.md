# Review moderation API

- `GET /api/admin/reviews/pending` lists reviews awaiting moderation.
- `PATCH /api/admin/reviews/{id}/approve` publishes a review.
- `DELETE /api/admin/reviews/{id}` removes a review.

All moderation endpoints require the administrator role. Public product review queries continue to return approved reviews only.
