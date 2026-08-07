---
title: Docker
permalink: /version/v3.0.x/samples/docker/
nav:
  order: 1
  parent: Samples
  icon: 🐳
---

Five Docker Compose setups. Pick the one that matches what you are doing, download the folder, and change
`Presets.json` to your own bin set.

| Sample | Use it when | Modules on |
|---|---|---|
| [Minimal]({% vlink /samples/docker/minimal/index.md %}) | You want the smallest thing that answers | none |
| [Quickstart]({% vlink /samples/docker/quickstart/index.md %}) | You are trying Binacle.Net out for the first time | docs + web UI |
| [Prod]({% vlink /samples/docker/prod/index.md %}) | **Your own backend calls the API** | health checks, packing logs |
| [Service]({% vlink /samples/docker/service/index.md %}) | **Callers you do not control reach the API directly** | + docs, accounts, JWT auth, rate limiting, a database |
| [Full]({% vlink /samples/docker/full/index.md %}) | You want to see everything it can do, on a machine nobody else can reach | everything, including `/_debug` |

## 🤔 Which of prod and service?

This is the choice that matters, and it is not about how big your deployment is.

**Prod** is the API sitting behind your own backend. Your server calls it, so it needs no accounts, no tokens
and no database - and every surface you do not enable is one you do not have to defend. Most deployments are
this.

**Service** is Binacle.Net offered to other people. That is what the Service Module is: accounts, JWT auth,
per-caller rate limiting, and storage to keep them in. Take it when callers you do not control reach the API
directly.

If you are unsure, start with **prod**. Adding the Service Module later is a flag and a connection string.

## ⚠️ Full is not a deployment

It switches on `DEBUG_ENDPOINT`, which mounts `/_debug`. That endpoint echoes the caller's entire request back
to them, including their `Authorization` header. It is on there so you can see what it does. Do not put that
configuration anywhere other people can reach.

## ✅ These shapes are tested

Each sample has a matching smoke profile of the same name, run against the image on every release. So these are
configurations that get checked, rather than ones nobody runs.
