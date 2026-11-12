# Coupons API

- `GET /api/coupons` lists coupons for administrators.
- `POST /api/coupons` creates a fixed or percentage coupon for administrators.
- `POST /api/coupons/validate?subtotal=100` validates a coupon for the authenticated customer.
- `POST /api/orders` accepts an optional `couponCode` and recalculates the discount from database values.

Coupon codes are normalized to uppercase. The backend enforces date ranges, percentage limits, one use per customer, and global usage limits. The client never controls the final discount or order total.

The migration `20261112000000_AddCouponDiscount` must be applied to existing SQL Server databases before deploying this version.
