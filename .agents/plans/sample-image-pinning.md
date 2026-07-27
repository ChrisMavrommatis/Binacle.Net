# Decide how the samples pin the docker image

**Status:** Not started. Decide before the v3.0.0 tag - the day `latest` moves is the day this bites.

**The deadline is not fixed yet.** It depends on whether a prerelease tag moves `latest`, which has never been
tested. If it does, sample users get v3.0.0 the moment the **beta** publishes, and this becomes a beta blocker
rather than a pre-tag one.

## Why

Every sample in the repo pulls `binacle/binacle-net:latest`:

- `samples/docker/minimal-setup/docker-compose.yml:3`
- `samples/docker/ui-setup/docker-compose.yml:3`
- `samples/docker/service-npgsql/docker-compose.yml:3`
- `samples/docker/service-azure/docker-compose.yml:3`
- `samples/kubernetes/minimal-setup/binacle-deployment.yaml:17`

v3.0.0 removes the V2 endpoints, rejects every ViPaq token an earlier version produced, changes the packing-logs
config shape so the old one fails startup validation, and narrows CIDR entries in the health check allow-list.
A user who copied a sample and left it on `latest` gets all of that on their next pull, with no version in their
config that says what happened.

## The call

Options:

1. **Pin to the exact released version** (`binacle/binacle-net:3.0.0`). Honest and reproducible; the sample goes
   stale between releases and someone has to bump it each time.
2. **Pin to the major line** (`binacle/binacle-net:3`) - if the release workflow publishes a major tag, which it
   does not today (`type=semver,pattern={{version}}` only). Would need a second tag rule.
3. **Leave `latest`** and say in each sample that it tracks the newest release across breaking changes.

Recommendation: pin the repo samples to the released version, and add a line to each sample saying which version
it is written for. A sample is copied once and lives for years; `latest` turns that into a time bomb.

Option 1 has an ordering to respect: pinning to `3.0.0` commits a reference to an image that does not exist until
the release publishes. Pin, then tag, then publish - and do not pin in a commit that sits on `main` for days.

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
