# Shipment API

- `GET /api/orders/{orderId}/shipment` lets the customer view shipment information for their own order.
- `PUT /api/admin/orders/{orderId}/shipment` lets administrators create or update carrier, tracking, and delivery status.

Customers cannot access another customer's shipment. Shipment timestamps are assigned by the server when status changes to `Shipped` or `Delivered`. External carrier synchronization can be added later without changing the controller contract.
