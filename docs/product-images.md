# Product images API

- `GET /api/products/{productId}/images` lists images for a product.
- `POST /api/products/{productId}/images` adds image metadata; admin only.
- `DELETE /api/products/{productId}/images/{imageId}` removes an image; admin only.

The API stores URLs and metadata. File storage (for example, Azure Blob Storage or S3) should be added separately; the backend should receive only trusted HTTPS URLs from that storage service. Marking an image as primary clears the previous primary image and synchronizes the product's legacy `ImageUrl` field.
