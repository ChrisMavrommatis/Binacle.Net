# Config

Configuration files related to local setup

## Tmux
Tmux setup for Binacle.Net
`tmux.sh`

---
## Serve
`serve.just`, loaded as the `serve` module. Brings one thing up in the foreground.

```bash
just serve api [N|S|U|All]       # Normal, WithServiceModuleOnly, WithUiModuleOnly, WithAllModules
just serve docs                  # jekyll serve + webpack watch, one Ctrl-C stops both
just serve web
```

Run `just install` once first - it does the npm workspaces, both jekyll sites' gems, and the asset copy.

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
(`docker compose -f config/docker-compose.yml up -d`); with no argument the harness falls back to SQLite.

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
Script for building Binacle.Net and testing it
`build.sh`

---

## Container data
**Postgres always uses a named volume**, never a folder here. It chowns its data dir to its own user and locks
it to 0700, which leaves a directory in the repo you cannot read — and that fails the next `docker build`,
because the CLI walks the whole context before it builds. Wipe it with `docker compose ... down -v`.

App logs and Azurite state are bind-mounted into `config/` so you can open them, which means the folders have
to exist and be writable by the container before anything starts — `build.sh` creates and opens them. Docker
never chowns a bind mount.

`docker-compose.volume.yml` puts the app's data in a volume instead. To read it:

```bash
docker compose -f ./config/docker-compose.volume.yml cp binacle-net:/app/data ./out
```

---

## Running the built image on its own
Two minimal stacks for `binacle-net:local` — one container, ServiceModule on SQLite, no telemetry. They differ
only in where `/app/data` goes.

| File | Data |
|---|---|
| `docker-compose.volume.yml` | named volume — nothing lands in the repo |
| `docker-compose.bind.yml` | a folder on disk, so you can open the logs; `BINACLE_DATA_DIR` overrides it |

The bind one needs the folder to exist and be writable by the container first
(`mkdir -p ./config/data && chmod -R 777 ./config/data`). Docker never chowns a bind mount.