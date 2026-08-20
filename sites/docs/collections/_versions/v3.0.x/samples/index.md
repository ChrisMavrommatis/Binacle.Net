---
title: Samples
permalink: /version/v3.0.x/samples/
nav:
  order: 20
  icon: 📦
---

Starting points for running Binacle.Net. Each one is a folder of files to download and edit.

## 🐳 Docker
Five setups, from nothing switched on to everything.

- [Minimal]({% vlink /samples/docker/minimal/index.md %}): the smallest configuration that still answers requests.
- [Quickstart]({% vlink /samples/docker/quickstart/index.md %}): trying Binacle.Net out, with the docs and the web UI.
- [Prod]({% vlink /samples/docker/prod/index.md %}): the API behind your own backend. **Most deployments want this one.**
- [Service]({% vlink /samples/docker/service/index.md %}): Binacle.Net offered to callers you do not control.
- [Full]({% vlink /samples/docker/full/index.md %}): everything on at once, for a machine nobody else can reach.

The [Docker]({% vlink /samples/docker/index.md %}) page explains the choice between **prod** and **service**.

## ☸️ Kubernetes
- [Minimal]({% vlink /samples/kubernetes/minimal/index.md %}): a minimal deployment on an existing cluster.

---

## 📖 Two things that apply to all of them

**`Presets.json` is your bin set.** Every sample ships an example one. Replacing it with your own boxes,
lockers or pallets is the first thing to do; until then the answers describe someone else's packaging.

**These shapes are tested.** Each Docker sample has a matching smoke profile of the same name, run against the
image on every release.

## 🏷️ About the image tag
Every sample pins `binacle/binacle-net:{{ page.version_tag }}` - the minor tag, which picks up fixes within the
{{ page.version_tag }} line and never a breaking change. A copied sample should not jump to a new major on the
next pull, which is what `latest` would do.
