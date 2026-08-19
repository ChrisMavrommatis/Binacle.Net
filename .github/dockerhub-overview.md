**Will it fit in the box?** Binacle.Net answers which box an order goes in, in milliseconds.

Give it your box sizes and a list of items and it returns the smallest box that holds them, and where every
item sits. It is a free and open source 3D bin packing API that you host yourself.

Built for checkout: your customer picks a locker or a box, and Binacle.Net says whether the order fits before
they pay.

## ⚡ Quick start

```bash
docker run -d --name binacle-net -p 8080:8080 \
  -e SWAGGER_UI=True -e SCALAR_UI=True -e UI_MODULE=True \
  binacle/binacle-net:{{MINOR}}
```

Then ask it which of two lockers a two-item order goes in:

```bash
curl -X POST http://localhost:8080/api/v3/fit/by-custom \
  -H 'Content-Type: application/json' \
  -d '{
    "parameters": { "algorithm": "FFD" },
    "bins": [
      { "id": "locker-S", "length": 30, "width": 20, "height": 15 },
      { "id": "locker-M", "length": 40, "width": 30, "height": 20 }
    ],
    "items": [
      { "id": "box-a", "quantity": 2, "length": 10, "width": 10, "height": 10 },
      { "id": "box-b", "quantity": 1, "length": 25, "width": 18, "height": 12 }
    ]
  }'
```

One result per box. The small locker takes only one of the two items; the medium one takes both:

```json
{"result":"Success","data":[
  {"result":"NotAllItemsFit","bin":{"id":"locker-S", ...},
   "fittedItems":[{"id":"box-b", ...}],
   "unfittedItems":[{"id":"box-a","quantity":2}],
   "fittedBinVolumePercentage":60.0,"fittedItemsVolumePercentage":72.97},
  {"result":"AllItemsFit","bin":{"id":"locker-M", ...},
   "fittedItems":[{"id":"box-b", ...},{"id":"box-a", ...},{"id":"box-a", ...}],
   "unfittedItems":[],
   "fittedBinVolumePercentage":30.83,"fittedItemsVolumePercentage":100}]}
```

That is the fit check, which answers yes or no. The pack endpoints run the same algorithms and also return the
position of every item.

### 🌐 In the browser

The three flags above turn on three optional pages:

- <http://localhost:8080/> - the packing demo
- <http://localhost:8080/swagger/> - Swagger UI
- <http://localhost:8080/scalar/> - Scalar

The API itself is under `/api/v3` and `/api/v4` and needs none of them.

## 📐 What it answers

Binacle.Net works **one box at a time**.

| It answers | It does not answer |
|---|---|
| Does this order fit in this box? | How do I split an order across boxes? |
| Which of my boxes is the smallest that holds it? | How many boxes do I need? |
| Where does every item sit? | Which carrier or rate is cheapest? |

**The algorithms are heuristics.** A yes is reliable - if it says the items fit, they fit, and the pack
endpoints show you how. A no is not a proof: there may be an arrangement it did not find.

The browser demo is for you, not for your customers. Binacle.Net has no storefront UI.

## 🏷️ Which tag to use

| Tag | Moves | Use it for |
|---|---|---|
| `{{VERSION}}` | never | pinning an exact build |
| `{{MINOR}}` | on each patch in the {{MINOR}} line | production - fixes, no behaviour changes |
| `latest` | on every release, major ones included | trying Binacle.Net out |

`latest` will cross a major version and can break your integration. **Pin `{{MINOR}}` for anything you keep.**

Prereleases publish their exact version only - they never move `{{MINOR}}` or `latest`.

Every tag is on the Tags tab. What changed in each is in the
[changelog](https://github.com/binacle-labs/Binacle.Net/blob/main/CHANGELOG.md).

## ⚙️ Configuration

| Variable | Default | What it turns on |
|---|---|---|
| `SWAGGER_UI` | off | Swagger UI at `/swagger/` |
| `SCALAR_UI` | off | Scalar at `/scalar/` |
| `UI_MODULE` | off | The packing demo at `/` |

The API works with all three off, which is the right setup for a service nobody browses to.

Logs are written to `/app/data` - mount a volume there if you want to keep them. Full configuration is at
<https://docs.binacle.net>.

## 🔒 Verifying what you pulled

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

## 📦 Deploying

Docker Compose and Kubernetes samples, from a one-line quickstart to a production setup:
<https://github.com/binacle-labs/Binacle.Net/tree/main/samples>

## 💬 Who is running this?

If Binacle.Net is running in something you built, say hello. Almost nobody does, and it is the only way this
gets built for real use instead of guesses.

Questions and "it worked" are equally welcome:
<https://github.com/binacle-labs/Binacle.Net/discussions>

## 🔗 Quick reference

- **Source:** <https://github.com/binacle-labs/Binacle.Net>
- **Documentation:** <https://docs.binacle.net>
- **Website and demo:** <https://www.binacle.net>
- **File an issue:** <https://github.com/binacle-labs/Binacle.Net/issues>
- **Releases:** <https://github.com/binacle-labs/Binacle.Net/releases>
- **Security policy:** <https://github.com/binacle-labs/Binacle.Net/blob/main/SECURITY.md>
- **Architectures:** `linux/amd64`
- **Base image:** `mcr.microsoft.com/dotnet/aspnet:10.0`. Runs as a non-root user, listens on 8080
- **License:** GPL-3.0-only for the code, CC-BY-SA-4.0 for the content
