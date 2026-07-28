# Config

Configuration files related to local setup

## Tmux
Tmux setup for Binacle.Net
`tmux.sh`

---
## Serve
`serve.just`, loaded as the `serve` module. Everything you run **from source** while working on the code.

```bash
just serve api [N|S|U|All]       # Normal, WithServiceModuleOnly, WithUiModuleOnly, WithAllModules
just serve docs                  # jekyll serve + webpack watch, one Ctrl-C stops both
just serve web
just serve services [-d]         # what the API talks to: aspire dashboard, azurite, postgres. No binacle-net
just serve services-down [-v]    # only needed after -d; Ctrl-C is enough otherwise
```

Run `just install` once first - it does the npm workspaces, both jekyll sites' gems, and the asset copy.

`services` is here rather than with the image stacks because it runs no binacle-net at all: it is what
`just serve api` talks to, and what the Postgres and AzureStorage test leaves need. Running the **built
image** is the other job, and that is the `image` module below.

---

## Benchmarks
Script  for all benchmarks

Arguments:
- `AlgorithmVersion`
- `MultipleBins`
- `MultipleItems`

---

## Tests
`tests.just`, loaded by the root `justfile` as the `test` module. One recipe per suite, so tab completion
finds them and CI calls the same recipes a maintainer does.

```bash
just --list test                 # every leaf
just test all                    # everything that needs nothing brought up
just test lib-unit               # one leaf
just test api-service-integration Postgres
```

Postgres and AzureStorage need their service up first
(`just serve services -d`); with no argument the harness falls back to SQLite.

---

## Coverage
`coverage.just`, loaded as the `coverage` module. It runs the same leaves with the collector attached - coverage
is the same run with extra output, not a second one.

```bash
just coverage all                # every suite + the table (cobertura)
just coverage all sonar          # the formats Sonar imports
just coverage report             # merge the last cobertura run -> build/coverage/html-report/index.html
just coverage table              # re-print the table without re-running
```

The format names the consumer: `cobertura` is what the table and the HTML report read, `sonar` is Visual Studio
xml for C# plus lcov for TS. Output is one flat file per suite under `build/tests/` and
`build/coverage/<format>/`, named after the project or package.

---

## Build
`build.just`, loaded as the `build` module. The publish and the image, nothing else.

```bash
just build publish            # dotnet publish -> build/binacle-net
just build image [version]    # publish, then docker build -t binacle-net:<version>, default local
```

`image` re-publishes every time - `docker build` copies whatever sits in `build/binacle-net`, so skipping it is
how a stale image gets tagged. The output path is fixed because the Dockerfile hardcodes it.

Neither recipe touches the container data folders, and neither needs `sudo`, so CI can call them as they stand.

---

## Image stacks
`image.just`, loaded as the `image` module. Runs the image `just build image` produced, three ways. One name
per compose file, and `up` prepares the bind-mounted folders before it starts anything.

```bash
just image up                    # same as `up full`
just image up full               # everything on: all modules, all three backends, the dashboard
just image up volume             # the image alone, SQLite, data in a named volume
just image up bind               # the image alone, SQLite, data in a folder you can open
just image down [name] [-v]      # -v drops the named volumes, postgres included
```

Extra arguments go straight through to `docker compose`. The name is positional, so pass it whenever you pass
a flag - `just image up -d` reads `-d` as the stack name and is rejected.

All three check `binacle-net:local` exists first and point you at `just build image` if it does not. Without
that check compose falls back to pulling from Docker Hub and reports "pull access denied", which reads like a
credentials problem rather than the missing local build it is. `serve services` needs no such check - it runs
no binacle-net.

The folder setup is written out in both `serve.just` and `image.just` rather than shared. A module that
reaches into another one puts back the coupling that splitting them removed, and it is a few lines of `mkdir`
and `chmod`.

---

## Container data
**Postgres always uses a named volume**, never a folder here. It chowns its data dir to its own user and locks
it to 0700, which leaves a directory in the repo you cannot read — and that fails the next `docker build`,
because the CLI walks the whole context before it builds. Wipe it with `just image down full -v`.

App logs and Azurite state are bind-mounted into `config/` so you can open them, which means the folders have
to exist and be writable by the container before anything starts — docker never chowns a bind mount, and the
containers write as their own users. The `up` recipes do that, per stack: `just serve services` needs
`config/azurite`; `image up full` needs it plus `config/data/{logs,pack-logs}`; `image up bind` needs
`BINACLE_DATA_DIR` (default `config/data`); `image up volume` needs none.

They open the **directory** and nothing inside it. The files belong to whoever wrote them — the app as
`APP_UID`, azurite as root — and stay writable to that same writer, so a recursive `chmod` would fail on
exactly those files while making nothing more writable. `sudo` is used only for a directory docker created
itself, which the daemon makes as root.

`docker-compose.volume.yml` puts the app's data in a volume instead. To read it:

```bash
docker compose -f ./config/docker-compose.volume.yml cp binacle-net:/app/data ./out
```

---

## Running the built image on its own
Two minimal stacks for `binacle-net:local` — one container, ServiceModule on SQLite, no telemetry. They differ
only in where `/app/data` goes.

| Stack | File | Data |
|---|---|---|
| `volume` | `docker-compose.volume.yml` | named volume — nothing lands in the repo |
| `bind` | `docker-compose.bind.yml` | a folder on disk, so you can open the logs; `BINACLE_DATA_DIR` overrides it |