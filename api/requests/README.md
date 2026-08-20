# Requests

`.http` files for hitting the API by hand from an IDE. Not tests — nothing runs them in CI.

| Folder | What it covers |
|---|---|
| `v3/` | v3 endpoints — fit, pack, and presets |
| `v4/` | v4 endpoints — fit, pack, and presets |
| `Service/` | ServiceModule — `Auth/Token.http`, plus the admin endpoints under `Admin/Account` and `Admin/Subscription` |
| `Health Check.http` | The `_health` endpoint |

## 📂 Layout — one file per endpoint

A request file mirrors its endpoint's source path and takes the endpoint class's name:

```
api/src/Binacle.Net/v4/Endpoints/Fit/CustomBin.cs          -> v4/Fit/CustomBin.http
api/src/Binacle.Net/v3/Endpoints/Pack/ByPreset.cs          -> v3/Pack/ByPreset.http
api/src/Binacle.Net.ServiceModule/v0/Endpoints/Auth/Token.cs -> Service/Auth/Token.http
```

So every endpoint has exactly one request, and a new one is easy to place — find the endpoint, match the path.
`requests.proj` globs `**\*.http`, so nothing needs registering.

Preset routes name the preset inline (`.../rectangular-cuboids/Small`). Change it to any key from
`Presets.json` — `rectangular-cuboids`, `perfect-cubes`, or `sample`.

Note that v3 **fit** takes only `algorithm` — `includeViPaqData` is a v3 **pack** parameter and a v4 parameter,
but not part of the v3 fit contract.

## 🔑 The admin requests need an account that exists

`Service/Admin/**` files carry a hard-coded `@ID` from whoever wrote them, so they answer `404` against a store
that has never seen it. Send `Admin/Account/List.http` to see what the store actually holds and copy an id out
of it into `@ID` - it needs nothing but a bearer token. `Admin/Account/Create.http` also returns a new id in its
`Location` header, and answers `409` once that username exists.

## 🌍 Variables

`http-client.env.json` defines the `local` environment. Pick it in your IDE before sending a request.

- `HOST` — defaults to `localhost:7194`
- `BEARERTOKEN` — empty. Fill it from the response of `Service/Auth/Token.http` to call anything behind auth.

Start the API with `just serve api` first. Use `S` if you need the ServiceModule endpoints.
