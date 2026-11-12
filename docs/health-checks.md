# Health endpoints

- `GET /health` confirms that the API process is responding.
- `GET /health/database` checks whether the configured database connection is available.

These endpoints are intended for local troubleshooting, deployment probes, and monitoring. They do not expose connection strings or exception details.
