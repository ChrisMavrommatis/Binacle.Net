# Sample Configurations for Binacle.Net

Starting points for running Binacle.Net, with Docker Compose or Kubernetes. Copy the folder that matches what
you are doing and edit it - these are meant to be taken, not referenced.

## Docker Compose

| Sample | Use it when |
|---|---|
| [minimal](docker/minimal) | You want the smallest thing that answers |
| [quickstart](docker/quickstart) | You are trying Binacle.Net out for the first time |
| [prod](docker/prod) | Your own backend calls the API |
| [service](docker/service) | Callers you do not control reach the API directly |
| [full](docker/full) | You want to see everything, on a machine nobody else can reach |

[docker/README.md](docker/README.md) explains the choice between `prod` and `service`, which is the one that
matters.

## Kubernetes

- [minimal](kubernetes/minimal) - a minimal deployment on an existing cluster.

## Two things that apply to all of them

**`Presets.json` is your bin set.** Every sample ships an example one. Replacing it with your own boxes,
lockers or pallets is the first thing to do; until then the answers describe someone else's packaging.

**The image tag is pinned to `binacle/binacle-net:3.0.0-beta.1`** for now, since `3.0` (the minor tag) does not
exist on Docker Hub until v3.0.0 is published. Once it is, the samples move to `binacle/binacle-net:3.0`, which
picks up fixes within the 3.0 line and never a breaking change.
