# Requests

`.http` files for hitting the API by hand from an IDE. Not tests — nothing runs them in CI.

| Folder | What it covers |
|---|---|
| `v3/` | v3 endpoints |
| `v4/` | v4 endpoints — fit and pack, by custom bin, by preset bin, and smallest bin |
| `Service/` | ServiceModule — `Token.http`, plus admin endpoints under `Admin/Accounts` and `Admin/Subscriptions` |
| `Health Check.http` | The `_health` endpoint |

Preset routes name the preset inline (`.../rectangular-cuboids/Small`). Change it to any key from
`Presets.json`.

## Variables

`http-client.env.json` defines the `local` environment. Pick it in your IDE before sending a request.

- `HOST` — defaults to `localhost:7194`
- `BEARERTOKEN` — empty. Fill it from the response of `Service/Token.http` to call anything behind auth.

Start the API with `./config/api.sh` first. Use `S` if you need the ServiceModule endpoints.
