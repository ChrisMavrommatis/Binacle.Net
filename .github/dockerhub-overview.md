**Will it fit in the box?** Binacle.Net answers in milliseconds - which box, which items, packed which way.

Free, open source, and yours to run. One `docker run` and it is up.

Built for checkout: your customer picks a locker or a box, and Binacle.Net says whether the order fits before
they pay. It solves the 3D bin packing problem over HTTP, in real time.

What it is not: the browser demo is for you, not for your customers. Binacle.Net has no storefront UI and
knows nothing about your carrier's rates.

## Quick start

```bash
docker run -d --name binacle-net -p 8080:8080 \
  -e SWAGGER_UI=True -e SCALAR_UI=True -e UI_MODULE=True \
  binacle/binacle-net:{{MINOR}}
```

Then ask it something:

```bash
curl -X POST http://localhost:8080/api/v3/fit/by-custom \
  -H 'Content-Type: application/json' \
  -d '{
    "parameters": { "algorithm": "FFD" },
    "bins":  [ { "id": "locker-M", "length": 40, "width": 30, "height": 20 } ],
    "items": [ { "id": "box-a", "quantity": 2, "length": 10, "width": 10, "height": 10 } ]
  }'
```

```json
{"result":"Success","data":[{"result":"AllItemsFit","bin":{...},
 "fittedItems":[...],"unfittedItems":[],
 "fittedBinVolumePercentage":8.33,"fittedItemsVolumePercentage":100}]}
```

With the three flags above you also get:

- <http://localhost:8080/> - the packing demo, in a browser
- <http://localhost:8080/swagger/> - Swagger UI
- <http://localhost:8080/scalar/> - Scalar

The API itself is under `/api/v3` and `/api/v4`, and needs none of them.

## Which tag to use

| Tag | Moves | Use it for |
|---|---|---|
| `{{VERSION}}` | never | pinning an exact build |
| `{{MINOR}}` | on each patch in the {{MINOR}} line | production - fixes, no behaviour changes |
| `latest` | on every release, major ones included | trying Binacle.Net out |

`latest` will cross a major version and can break your integration. **Pin `{{MINOR}}` for anything you keep.**

Prereleases publish their exact version only - they never move `{{MINOR}}` or `latest`.

Every tag is on the Tags tab. What changed in each is in the
[changelog](https://github.com/binacle-labs/Binacle.Net/blob/main/CHANGELOG.md).

## Configuration

| Variable | Default | What it turns on |
|---|---|---|
| `SWAGGER_UI` | off | Swagger UI at `/swagger/` |
| `SCALAR_UI` | off | Scalar at `/scalar/` |
| `UI_MODULE` | off | The packing demo at `/` |

The API works with all three off, which is the right setup for a service nobody browses to.

Logs are written to `/app/data` - mount a volume there if you want to keep them. Full configuration is at
<https://docs.binacle.net>.

## Verifying what you pulled

Every published image is signed with cosign - keyless, against the digest, so one signature covers
`{{VERSION}}`, `{{MINOR}}` and `latest` alike - and carries an SPDX software bill of materials and SLSA build
provenance.

```bash
cosign verify binacle/binacle-net:{{MINOR}} \
  --certificate-identity-regexp '^https://github\.com/binacle-labs/Binacle\.Net/\.github/workflows/release-docker-image\.yml@' \
  --certificate-oidc-issuer https://token.actions.githubusercontent.com

docker buildx imagetools inspect binacle/binacle-net:{{MINOR}}
```

Both flags matter. Without the identity you are only asking whether *anyone* signed the image, and anyone can.

A pass proves the image came from this repository's release workflow. It does not mean the image is free of
vulnerabilities - for that, read the bill of materials.

## Deploying

Docker Compose and Kubernetes samples, from a one-line quickstart to a production setup:
<https://github.com/binacle-labs/Binacle.Net/tree/main/samples>

## Who is running this?

Thousands of people pull this image and almost nobody says anything. If Binacle.Net is running in something
you built, say hello - it is the only way this gets built for real use instead of guesses.

Questions and "it worked" are equally welcome:
<https://github.com/binacle-labs/Binacle.Net/discussions>

## Quick reference

- **Source:** <https://github.com/binacle-labs/Binacle.Net>
- **Documentation:** <https://docs.binacle.net>
- **Website and demo:** <https://www.binacle.net>
- **File an issue:** <https://github.com/binacle-labs/Binacle.Net/issues>
- **Releases:** <https://github.com/binacle-labs/Binacle.Net/releases>
- **Security policy:** <https://github.com/binacle-labs/Binacle.Net/blob/main/SECURITY.md>
- **Architectures:** `linux/amd64`
- **Base image:** `mcr.microsoft.com/dotnet/aspnet:10.0`. Runs as a non-root user, listens on 8080
- **License:** GPL-3.0-only for the code, CC-BY-SA-4.0 for the content
