# Audit logs API

- `GET /api/admin/audit-logs` lists audit records for administrators.

Supported filters include `action`, `entityName`, `userId`, `page`, and `pageSize`. The service also exposes a recording method for future controller and middleware instrumentation. Audit details must never contain passwords, raw tokens, payment credentials, or full card data.
