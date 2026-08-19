# Docker Compose Samples

Five configurations, from nothing switched on to everything. Pick the one that matches what you are doing, copy
the folder, and change `Presets.json` to your own bin set.

| Sample | Use it when | Modules on |
|---|---|---|
| [minimal](minimal) | You want the smallest thing that answers | none |
| [quickstart](quickstart) | You are trying Binacle.Net out for the first time | docs + web UI |
| [prod](prod) | **Your own backend calls the API** | health checks, packing logs |
| [service](service) | **Callers you do not control reach the API directly** | + docs, accounts, JWT auth, rate limiting, a database |
| [full](full) | You want to see everything it can do, on a machine nobody else can reach | everything, including `/_debug` |

## Which of prod and service?

This is the choice that matters, and it is not about how big your deployment is.

**`prod`** is the API sitting behind your own backend. Your server calls it, so it needs no accounts, no tokens
and no database - and every surface you do not enable is one you do not have to defend. Most deployments are
this.

**`service`** is Binacle.Net offered to other people. That is what ServiceModule is: accounts, JWT auth,
per-caller rate limiting, and storage to keep them in. Take it when callers you do not control reach the API
directly.

If you are unsure, start with `prod`. Adding ServiceModule later is a flag and a connection string.

## `full` is not a deployment

It switches on `DEBUG_ENDPOINT`, which mounts `/_debug`. That endpoint echoes the caller's entire request back
to them, including their `Authorization` header. It is on there so you can see what it does. Do not put that
configuration anywhere other people can reach.

## About the image tag

Every sample pins `binacle/binacle-net:3.0.0-beta.4` for now, since `3.0` (the minor tag) does not exist on
Docker Hub until v3.0.0 is published. Once it is, the samples move to `binacle/binacle-net:3.0`, which picks up
fixes within the 3.0 line and never a breaking change. `latest` follows the newest release across majors, which
is right for trying things out and wrong for anything you keep.

## These shapes are tested

Every sample here is smoke-tested against the image on every release. So these are configurations that are
checked, rather than ones nobody runs.
