# Tooling

Every task this repo can run: the `just` modules, the scripts that have not moved into one, the local compose
files, and emulator state. CI calls these same recipes rather than keeping its own copy, so a workflow step and
a maintainer running the command by hand do the same thing. Everything here is run from the repo root. `just`
with no arguments lists every task.

This is **not** a deployment template - `samples/` holds the user-facing starting points.

---

## Setup
Root `justfile`, not a module. Run once on a fresh clone.

```bash
just install                     # npm workspaces, both jekyll sites' gems, then the asset copy
just assets                      # only the asset copy - after changing anything under assets/
```

`install` assumes the tools are already there. What they are, which versions, and how to install them -
including docker and the two smoke binaries - is **[DEVELOPMENT.md](../DEVELOPMENT.md)** at the repo root. It is
the only place that lives; nothing here repeats it.

---

## Serve
`serve.just`, loaded as the `serve` module. Everything you run **from source** while working on the code.

```bash
just serve api [N|S|U|All]       # Normal, WithServiceModuleOnly, WithUiModuleOnly, WithAllModules
just serve docs                  # jekyll serve + webpack watch, one Ctrl-C stops both
just serve web
just serve services-up [-d]      # what the API talks to: aspire dashboard, azurite, postgres. No binacle-net
just serve services-down [-v]    # only needed after -d; Ctrl-C is enough otherwise
```

`services-up` is here rather than with the image stacks because it runs no binacle-net at all: it is what
`just serve api` talks to, and what the Postgres and AzureStorage test leaves need. Running the **built
image** is the other job, and that is the `image` module below.

Its file, `serve.services.yml`, is the only place those three services are declared. The `full` image stack
includes it rather than repeating it, so both run the same postgres on the same volume - one database, and
one `-v` in either place that empties it.

---

## Tests
`tests.just`, loaded as the `test` module. One recipe per suite, so tab completion finds them and CI calls the
same recipes a maintainer does.

```bash
just --list test                 # every leaf
just test all                    # everything that needs nothing brought up
just test lib-unit               # one leaf
just test api-service-integration Postgres
```

Postgres and AzureStorage need their service up first (`just serve services-up -d`); with no argument the harness
falls back to SQLite.

---

## Coverage
`coverage.just`, loaded as the `coverage` module. It runs the same leaves with the collector attached - coverage
is the same run with extra output, not a second one.

```bash
just coverage all                # every suite + the table (cobertura)
just coverage all sonar          # the formats Sonar imports
just coverage report             # merge the last cobertura run -> artifacts/coverage/html-report/index.html
just coverage table              # re-print the table without re-running
```

The format names the consumer: `cobertura` is what the table and the HTML report read, `sonar` is Visual Studio
xml for C# plus lcov for TS. Output is one flat file per suite under `artifacts/tests/` and
`artifacts/coverage/<format>/`, named after the project or package.

---

## OpenAPI and the agent indexes
Two small modules, `openapi.just` and `agents.just`.

```bash
just openapi generate [dir]      # write artifacts/openapi/Binacle.Net_v3.json and _v4.json
just openapi lint [dir]          # generate, then Spectral them against openapi.spectral.yaml
just agents all                  # rewrite every .agents/**/_index.md
```

The documents come out of the build, not out of a running server, so nothing has to be brought up first.

---

## Regenerating committed data
`regen.just`, loaded as the `regen` module. Four tools write data that is **committed to the repo**, and this
is the only place that says how to run them.

```bash
just regen all                     # every generator, in dependency order
just regen or-lib-scenarios        # OR-Library text -> shared/data/bischoff-suite
just regen vipaq-packed-data       # those + custom-problems, packed -> vipaq/data/packed
just regen vipaq-interop-vectors   # the interop pair + header bytes -> vipaq/test-vectors
just regen check                   # regenerate, then fail if any of it changed
```

Every tool takes no arguments by design: a run does every generator in its list, so it cannot half-run and
leave the data inconsistent. Nothing here is parameterised for the same reason.

`or-lib-scenarios` writes what `vipaq-packed-data` reads, which is the whole reason `all` exists - that
ordering is the part that is easy to get wrong by hand. `vipaq-interop-vectors` is one recipe rather than two
because its C# and TS halves write `interop/cs` and `interop/ts` from the same `input.json`; regenerating one
alone is the drift the interop integrity tests exist to catch.

Every run is deterministic, so `check` is just "run everything, then see whether the tree moved". It diffs only
the `.json` the generators write - these folders also hold their own README, and `vipaq/test-vectors` as a
whole holds hand-authored vectors no generator touches. **Nothing in CI calls it**, and that is deliberate: it
is for the maintainer who edited a tool or a source problem and wants to know what fell out of step.

---

## Build
`build.just`, loaded as the `build` module. The publish and the image, nothing else.

```bash
just build publish               # dotnet publish -> artifacts/binacle-net
just build image [version]       # publish, then docker build -t binacle-net:<version>, default local
```

`image` re-publishes every time - `docker build` copies whatever sits in `artifacts/binacle-net`, so skipping it is
how a stale image gets tagged. The output path is fixed because the Dockerfile hardcodes it.

Neither recipe touches the container data folders, and neither needs `sudo`, so CI can call them as they stand.

---

## Image stacks
`image.just`, loaded as the `image` module. Runs the image `just build image` produced, three ways - all
`binacle-net:local`, differing in what runs beside it and where `/app/data` goes.

```bash
just image up                    # same as `up full`
just image up full               # everything on: all modules, all three backends, the dashboard
just image up volume             # the image alone, SQLite, data in a named volume - nothing lands in the repo
just image up bind               # the image alone, SQLite, data in a folder you can open
just image down [name] [-v]      # -v drops the named volumes, postgres included
```

Extra arguments go straight through to `docker compose`. The name is positional, so pass it whenever you pass
a flag - `just image up -d` reads `-d` as the stack name and is rejected.

Two files, three stacks. `volume` and `bind` are one container differing only in where `/app/data` goes, so
they share `image.local.yml`; `_compose` gives each its own project name and sets `BINACLE_DATA_DIR` for
`bind` alone. `full` is `image.full.yml`, which `include:`s that file and `serve.services.yml` and overrides
the app's storage and telemetry - about twenty lines, and nothing declared twice. It publishes the same 5432
as `serve services-up`, so those two cannot run at once.

All three check `binacle-net:local` exists first and point you at `just build image` if it does not. Without
that check compose falls back to pulling from Docker Hub and reports "pull access denied", which reads like a
credentials problem rather than the missing local build it is. `serve services-up` needs no such check - it
runs no binacle-net.

The folder setup is written out in both `serve.just` and `image.just` rather than shared. A module that
reaches into another one puts back the coupling that splitting them removed, and it is a few lines of `mkdir`
and `chmod`.

### Verifying a published image

```bash
just image verify 3.0.0            # all four checks
just image verify 3.0.0 signature  # one of them
```

The odd one out in this module: it reads a published image off Docker Hub, builds nothing, and never logs in.
Four checks, each answering something the next assumes - which Docker Hub tags share the digest, the
signature, the SBOM and provenance, and the labels plus what the container says about itself when you run it.

**Docker Hub only.** The staging registry is the release workflow's business and nothing else reads it.

**The version is required and has no default**, because a default rots into a tag nobody meant to check. All
four run even when one fails, so a failure comes with the three answers that explain it; the exit code is 1 if
any check failed. Only the signature check needs `cosign` - see `DEVELOPMENT.md` - and it says so rather than
passing quietly when it is missing.

**Only `3.0.0` and later can pass.** Signing and the SBOM start there, so `2.1.1` and
anything earlier fail on `signature` and `attestations`. That is history rather than a broken check - `2.1.1`
is the useful thing to run it against when you want to watch it fail.

---

## Smoke
`smoke.just`, loaded as the `smoke` module. Tests the **image** rather than the code: what it contains, and what
its HTTP surface does with the modules switched on and off. Needs `container-structure-test` and `hurl` -
see [DEVELOPMENT.md](../DEVELOPMENT.md).

```bash
just smoke all                       # build binacle-net:local, check its structure, then every profile
just smoke test-structure [image]    # the static content only - reads the image, no container
just smoke test <profile> [image]    # one profile end to end: up -> hurl -> down
just smoke up <profile> [image]      # bring one up and leave it   [minimal|quickstart|prod|service|full]
just smoke down <profile> [-v]       # stop it
```

**Every recipe takes the image last and defaults to `binacle-net:local`**, so the same suite runs against a
local build or a published tag:

```bash
just smoke all binacle/binacle-net:3.0.0           # smoke what is actually on Docker Hub
just smoke test prod binacle/binacle-net:3.0        # one profile against the released minor tag
```

Given anything but the local tag, `all` pulls instead of building - there is nothing to build, and building
would tag `binacle-net:local` while the stacks went on using the image you asked for. The stacks read it as
`$BINACLE_IMAGE` with the same default, so a bare `docker compose -f tooling/smoke/<profile>.yml` still works.

Two halves. `tooling/smoke/structure.yaml` is read straight from the image - the shipped config files,
`/app/data` ownership, the OCI labels. It has nothing to do with which stack is up, so `all` runs it once rather
than once per profile. The other half is one `.hurl` per profile, run against a running stack. The four profiles
- `minimal`, `quickstart`, `prod`, `service`, `full` - are declared in the `profiles` variable at the top of
`smoke.just`, and each one is also a folder name under `samples/docker/`.

Editing a `.hurl` is the one case for the private recipe: `just smoke up prod`, then
`just smoke::_test_profile prod` as many times as you need, then `just smoke down prod -v`.

**[`smoke/README.md`](smoke/README.md) has the rest** - what each profile claims, the two rules that decide
whether a check belongs in this suite at all, and the gotchas that make a green run mean something. Read it
before adding or changing an assertion.

---

## Container data
**Postgres always uses a named volume**, never a folder here. It chowns its data dir to its own user and locks
it to 0700, which leaves a directory in the repo you cannot read - and that fails the next `docker build`,
because the CLI walks the whole context before it builds. It is `binacle-net-postgres`, one volume shared by
`serve services-up` and `image up full`, so `-v` in either place wipes the database for both.

App logs and Azurite state are bind-mounted into `tooling/` so you can open them, which means the folders have
to exist and be writable by the container before anything starts - docker never chowns a bind mount, and the
containers write as their own users. The `up` recipes do that, per stack: `just serve services-up` needs
`tooling/azurite`; `image up full` needs the same one; `image up bind` needs `BINACLE_DATA_DIR` (default
`tooling/data`); `image up volume` needs none.

They open the **directory** and nothing inside it. The files belong to whoever wrote them - the app as
`APP_UID`, azurite as root - and stay writable to that same writer, so a recursive `chmod` would fail on
exactly those files while making nothing more writable. `sudo` is used only for a directory docker created
itself, which the daemon makes as root.

The `volume` and `full` stacks keep their data in `binacle-net-data`, where you cannot open it directly. Copy
it out by name, so it works whether or not anything is running:

```bash
docker run --rm -v binacle-net-data:/data -v "$PWD/out:/out" alpine cp -a /data/. /out/
```

---

## Benchmarks and performance
Still scripts, one per slice. Both take `-c Release` and write into gitignored folders.

```bash
./tooling/benchmarks.lib.sh [FastValidation|AlgorithmRacing|BischoffSuite|Parallelization|ResultSelection]
./tooling/benchmarks.vipaq.sh [Encode|Decode]      # no argument = every benchmark
./tooling/performance.lib.sh                       # console runner, writes markdown reports
./tooling/performance.vipaq.sh
```

The alias tables live at the top of each `benchmarks.*` script - that is the list to change when a benchmark
class is added or renamed.

---

## Tmux
`tmux.sh` builds (or re-attaches to) a session named `binacle` with windows `api`, `docs`, `web`, `tests`,
`misc` and `bench_1..3`. Panes are pre-`cd`'d but nothing runs automatically - it is a staging layout, not a
launcher.
