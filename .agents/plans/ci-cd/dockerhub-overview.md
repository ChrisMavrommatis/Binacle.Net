---
description: The Docker Hub repository page
paths:
  - ".github/workflows/**"
---

# The Docker Hub repository page

**The wiring is done.** `.github/dockerhub-overview.md` is the page, `just image dockerhub-overview <version>`
renders it, `shared-dockerhub-overview.yml` publishes it, and the release pipeline's `page` job calls that
last and skips prereleases. `$ci-cd`, `$ci-cd/release-pipeline`, `$tooling` and `$commands` carry all of it.
The short description on the repository was already correct.

**The credential is answered.** `DOCKERHUB_TOKEN` writes the description - HTTP 200 on the existing registry
push credential, proved 2026-08-19. No second secret, no password fallback. `$ci-cd` records it.

Two things left, then this file goes.

## 1. The `curl` example was run against a tag that no longer exists

The quick start in `.github/dockerhub-overview.md` shows a real request and a real response. The response came
from a run against a tag that has since been deleted from Docker Hub. **Re-run it against a tag that exists and
paste the real response back.** A broken first command is the whole first impression.

**Read the rendered page before publishing it:** `just image dockerhub-overview <version>` prints exactly what
the pipeline writes. The recipe refuses a version with a suffix, so it takes a released version only.

## 2. The logo and the categories

Both are web-form settings on the repository, both are one sitting, and neither touches anything above.

**Logo.** A sponsored-org perk and the slot is empty. `assets/media/logo/binacle-logo-512x512.png` is the
right source - Docker Hub wants a square image and 512x512 clears its minimum with room. Check the transparent
variant (`-512x512a.png`) against both themes first; the page is white in light mode and dark in dark mode, so
a logo that assumes one will look broken in the other.

**Categories.** The repo has one, "Integration & delivery", and three are allowed. That category is about
CI/CD tooling, which this is not - it is the leftover of a single hurried pick. **"Developer Tools" is the
closer fit and should lead**, with "Integration & delivery" kept only if a third genuinely better one cannot
be found in the list Docker offers. Categories feed Hub search, so this is the only item here that changes who
arrives at the page.

Then delete this file.

## Do not

Each of these was decided and is easy to undo by accident.

- **Do not put the tag list back.** Fifteen hand-maintained entries duplicating a Tags tab that is always
  right, and it is what rotted the page the first time. The three-row policy table replaces it and answers the
  one thing neither tab explains: which tag belongs in a compose file.
- **Do not name GHCR on the page.** It is staging. The reason to name it would have been handing rate-limited
  users an escape hatch, and the sponsored badge means there are none - a staging registry on a landing page
  becomes a support surface nobody meant to own.
- **Do not commit a concrete version into the page.** Placeholders and substitution, or the file is wrong the
  day the next minor ships. That is the same reason the tag list went.
- **Do not add the service module, ViPaq, or the health endpoint.** The module is not advertised here, at the
  maintainer's call. ViPaq belongs on the docs site - on a page where someone is deciding whether to run one
  command, a second format name is a reason to hesitate. The health endpoint is off by default and its path is
  configurable, so a line about it is wrong for most readers.
- **Do not let the `cosign` block drift from `SECURITY.md`.** It is copied verbatim, only the tag differs, and
  that file is the source. A published verify command that fails reads as a bug in the project.
- **Do not publish a page naming a tag that is absent or unsigned.** The org move re-keyed the certificate
  identity, so anything signed under the old owner fails the published command.
- **Do not widen `DOCKERHUB_TOKEN`.** It already does both jobs at the scope it has.
- **Do not dispatch the workflow with an empty version input to check a wording change.** Empty takes the
  latest non-prerelease release, which is not always the version you just edited the page for. Render it
  locally instead, or type the version.
