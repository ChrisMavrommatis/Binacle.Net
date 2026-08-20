# Workflows

Every GitHub Actions workflow, one file each. They call the same `just` recipes a maintainer runs, so a green
check and a local run mean the same thing.

Each file opens with a comment block explaining why it is shaped the way it is. **Read that before changing
one** - most of what looks odd here was deliberate.

## 🚦 What runs when

| Workflow | Fires on | What it does |
|---|---|---|
| `pull-request.yml` | Every pull request | Works out what changed, then runs the test suite, an image build and the workflow lint. Its `gate` job is the only name branch protection holds |
| `release-docker-image.yml` | Pushing a `v<digit>…` tag | The release: notes gate, tests, build and push to GHCR, smoke, copy to Docker Hub by digest, GitHub release, Docker Hub page |
| `deploy-docs-site.yml` | By hand | Builds the docs site, checks its links offline, deploys to Cloudflare, tags the commit it published |
| `deploy-web-site.yml` | By hand | The same for the web site |
| `sonar-analysis.yml` | By hand | Coverage to SonarCloud. Keep Automatic Analysis off in the Sonar UI - the two fight |
| `codeql-analysis.yml` | Merge to `main`, weekly, by hand | Code scanning. Findings land in the Security tab, not on a check |

## 🔄 The `shared-` files

A `shared-` prefix means **another workflow calls this one**. They are not private - each keeps its own
manual trigger, because running one by hand is the point.

| Workflow | Called by | Also runnable by hand for |
|---|---|---|
| `shared-test-suite.yml` | The pull request gate, the release | Running every test leaf plus the OpenAPI lint against a branch |
| `shared-smoke-image.yml` | The release | Smoking any published tag - it must test a **published** image, not a local build |
| `shared-dockerhub-overview.yml` | The release, as its last job | Fixing the wording on the Docker Hub page without cutting a tag |

## ⚠️ Three that are easy to get wrong

- **The image build stays out of the test suite.** The release calls that file whole, so anything added there
  is paid for twice - once per pull request and again on every release.
- **The release tag pattern is `v` then a digit**, not `v*`. The deploy workflows push their own marker tags
  (`docs-<n>`, `web-release-<run>`), and a release build must never fire on one.
- **Never rebuild the image between build and publish.** The copy to Docker Hub is by digest, which is what
  makes the published image the exact one the smoke suite passed.

The composite actions these call live next door in [`../actions`](../actions).
