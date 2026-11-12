# Admin users API

- `GET /api/admin/users` lists users without password data.
- `GET /api/admin/users/{id}` returns one user without password data.
- `PATCH /api/admin/users/{id}/role` changes a user between `admin` and `client`.
- `PATCH /api/admin/users/{id}/status?isActive=false` disables or enables a user.

Only administrators can use these endpoints. Administrators cannot deactivate themselves, and inactive users cannot log in.
