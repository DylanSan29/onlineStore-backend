# Reviews API

- `GET /api/products/{productId}/reviews` returns approved reviews publicly.
- `POST /api/products/{productId}/reviews` lets an authenticated customer review a product they purchased.

Each customer can review a product once. New reviews remain unapproved until an administration workflow is added, preventing unmoderated content from appearing publicly.
