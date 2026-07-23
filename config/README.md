# Config

Configuration files related to local setup

## Tmux
Tmux setup for Binacle.Net
`tmux.sh`

---
## Api
Script for running Binacle.Net
`api.sh`

Arguments:
- `Normal` (`N`)
- `WithServiceModuleOnly` (`S`)
- `WithUiModuleOnly` (`U`)
- `WithAllModules` (`A`)

---

## Benchmarks
Script  for all benchmarks

Arguments:
- `AlgorithmVersion`
- `MultipleBins`
- `MultipleItems`

---

## Tests
Script for running the tests for Binacle.Net

Arguments
- `lib` (`Binacle.Lib.UnitTests`)
- `api` (`Binacle.Net.IntegrationTests`)
- `api_service` (`Binacle.Net.ServiceModule.IntegrationTests`)
- `vipaq` (`Binacle.ViPaq.UnitTests`)

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