# Pin the samples to the released docker image

**Status: decided 2026-07-28, not applied.** The call is **option 1 - pin the five repo samples to the exact
released version** (`binacle/binacle-net:3.0.0`), and add a line to each sample saying which version it is written
for. What is left is applying it, which is deliberately not done yet - see the ordering below.

**Do not apply this early.** Pinning to `3.0.0` commits a reference to an image that does not exist until the
release publishes. Pin, then tag, then publish - and do not pin in a commit that sits on `main` for days. That is
why the decision is recorded here instead of being made and applied in one sitting.

**The deadline is not fixed yet.** It depends on whether a prerelease tag moves `latest`, which has never been
tested. If it does, sample users get v3.0.0 the moment the **beta** publishes, and this becomes a beta blocker
rather than a pre-tag one.

## Apply it like this

Five files, `latest` -> `3.0.0`, plus a one-line comment in each saying which version the sample targets:

- `samples/docker/minimal-setup/docker-compose.yml:3`
- `samples/docker/ui-setup/docker-compose.yml:3`
- `samples/docker/service-npgsql/docker-compose.yml:3`
- `samples/docker/service-azure/docker-compose.yml:3`
- `samples/kubernetes/minimal-setup/binacle-deployment.yaml:17`

## Why

All five pull `binacle/binacle-net:latest` today. v3.0.0 removes the V2 endpoints, rejects every ViPaq token an earlier version produced, changes the packing-logs
config shape so the old one fails startup validation, and narrows CIDR entries in the health check allow-list.
A user who copied a sample and left it on `latest` gets all of that on their next pull, with no version in their
config that says what happened.

## Why the exact version, and not the other two

A sample is copied once and lives for years; `latest` turns that into a time bomb. The cost of option 1 is real -
the samples go stale between releases and someone bumps them each time - and it was accepted.

Rejected:

- **Pin to the major line** (`binacle/binacle-net:3`). Would keep the samples current within v3, but the release
  workflow publishes no major tag today (`type=semver,pattern={{version}}` only), so it needs a second tag rule
  first. Worth revisiting if the bumping becomes a chore.
- **Leave `latest`** with a warning line. Rejected: the warning does not help the reader who already copied it.

## Also affected - the docs site, which is off limits to edit

Record what the pages must say and leave the writing to the docs session:

- `docs/collections/_common_pages/quick-start.md:26` runs `binacle/binacle-net:latest`. It is a **common** page
  shared by every version folder, so whatever it says applies to a v1.3.x reader too.
- `docs/collections/_versions/v2.0.x/samples/docker/*/docker-compose.yml` and the same under `v2.1.x` pin
  `latest`. A reader on the v2.1.x docs following a v2.1.x sample gets a 3.0.0 image. Those version folders
  should pin their own line (`2.1.1`, `2.0.x`), because that is what the page is documenting.

## Done when

The repo samples carry a deliberate tag, and the docs requirement above is written into the docs plan that owns
those pages.
