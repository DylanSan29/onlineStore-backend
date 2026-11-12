# Wishlist API

Authenticated customers can manage their own wishlist:

- `GET /api/users/me/wishlist` lists saved active products.
- `POST /api/users/me/wishlist/{productId}` saves an active product.
- `DELETE /api/users/me/wishlist/{productId}` removes a product.

The API takes the user ID from the JWT, never from the request body. This prevents one customer from reading or changing another customer's wishlist. Duplicate additions are idempotent.
