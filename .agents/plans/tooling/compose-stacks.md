---
description: Reorganise the compose stacks - the backing services in one file, the image in two
paths:
  - "tooling/**"
---

# Reorganise the compose stacks

**Status:** **Nothing is implemented** - the four compose files are exactly as they have always been. The
maintainer settled the shape on 2026-08-15 and the compose behaviour it depends on was tested the same day
against Docker Compose v5.4.0. This file is ready to execute.

**One question is open and it is the maintainer's**, in "The bare `up` default" below. It does not block
anything: the fallback is to change nothing.

## What is wrong today

`tooling/` holds four compose files:

| File | Run by | What it starts |
|---|---|---|
| `docker-compose.yml` | `just serve services` | postgres, azurite, aspire-dashboard. No app |
| `docker-compose.build.yml` | `just image up full` | the app, **plus its own** postgres, azurite, aspire-dashboard |
| `docker-compose.volume.yml` | `just image up volume` | the app alone, SQLite, data in a named volume |
| `docker-compose.bind.yml` | `just image up bind` | the app alone, SQLite, data in a folder you can open |

**The backing services are written down twice.** `docker-compose.yml` and `docker-compose.build.yml` each
declare their own postgres, azurite and aspire-dashboard, with the same images, the same ports and the same
credentials. Change the postgres password in one and the other keeps the old one - nothing fails, they just
quietly stop matching.

Today they share azurite state by accident rather than by design: both bind `./azurite`, both sit in
`tooling/`, so both resolve to `tooling/azurite`. Move either file and that silently stops being true.

**`volume` and `bind` differ by one line** - where `/app/data` goes. That is one file with a variable, not two
files.

**Two of the names lie.** `docker-compose.build.yml` builds nothing; `just build image` does the building.
`docker-compose.yml` reads like the repo's main stack and is not - it is one module's backing services.

## The end state

Three files, each named after the `just` module that runs it, flat in `tooling/` beside the `.just` file that
owns them:

| File | Owned by | What it holds |
|---|---|---|
| `tooling/serve.services.yml` | `serve` | postgres, azurite, aspire-dashboard. **The only place these are declared** |
| `tooling/image.local.yml` | `image` | the app alone, SQLite. Serves **both** the `volume` and the `bind` stack - the recipe picks which |
| `tooling/image.full.yml` | `image` | `include:` the other two, then override one connection string. About ten lines |

`image.full.yml` in full:

```yaml
name: binacle-net-full

include:
  - serve.services.yml
  - image.local.yml

services:
  binacle-net:
    environment:
      - POSTGRES_CONNECTION_STRING=Host=postgres;Port=5432;Database=binacle_net;Username=appuser;Password=...
      - OTEL_EXPORTER_OTLP_HEADERS=x-otlp-api-key=local-dev-not-a-real-api-key
```

The recipes:

```
just serve services-up [-d]           renamed from `services`
just serve services-down [-v]         unchanged
just image up [full|volume|bind]      all three names unchanged
just image down [full|volume|bind]
```

**`services` becomes `services-up`** so the pair reads as a pair. The two recipes do opposite things and only
one of them said so. `serve.just`'s header comment currently explains why they are *not* an up/down pair -
because `up` in the foreground is stopped with Ctrl-C and `down` is only needed after `-d`. **That reasoning is
about when you need the second recipe, not about what the first one is called**, so it stays in the file and
the names stop contradicting it.

**`volume` and `bind` stay as stack names. Only the second file goes.** They are the same container twice,
differing in one line, so they share one compose file - but they are two things a maintainer wants to run and
compare, and **`bind` is the one in daily use**. The recipe holds the difference:

| `just image up ...` | project | `BINACLE_DATA_DIR` | `/app/data` lands in |
|---|---|---|---|
| `volume` | `binacle-net-volume` | unset | the named volume `binacle-net-volume_app_data` |
| `bind` | `binacle-net-bind` | `./data`, or whatever you export | a folder you can open |
| `full` | `binacle-net-full` | unset | the named volume, alongside postgres and azurite |

Two separate project names, so the two do not recreate each other's container and `just image down volume`
leaves a running `bind` alone - exactly as today, where they are separate files with separate `name:` keys.

**The names are the interface; the file is the implementation.** Nothing a maintainer types changes here. What
goes away is a second copy of the same twenty lines.

### Every named volume gets a fixed name

Decided 2026-08-15. Compose prefixes a volume with the project name unless the file gives it one, so the same
declaration under two projects makes two volumes. Both named volumes here get an explicit name instead:

```yaml
volumes:
  postgres_data:
    name: binacle-net-postgres
  app_data:
    name: binacle-net-data
```

**For postgres this is the point of the exercise.** After the `include:` refactor postgres is declared in one
file, but `just serve services-up` runs it under `binacle-net-services` and `just image up full` runs the same
declaration under `binacle-net-full`. Without a fixed name those are two databases that look like one. With it
they are one database, and the app you started from source and the app running in the image see the same rows.

**For app data it buys a predictable handle.** `docker volume inspect binacle-net-data` and the `cp` command in
`tooling/README.md` stop depending on which project happened to create it.

**This is the one part of this work that is not invisible.** The existing volumes are
`binacle-net-volume_app_data` and two project-prefixed postgres volumes. Renaming orphans them, so the next
`just image up volume` starts on an empty database and re-seeds the admin user. **That is acceptable - it is
local dev data - but it must be said out loud rather than discovered.** Anything worth keeping gets copied out
first; everything else is wiped with `docker volume rm` once the new names are in.

This is the execution of the maintainer's 2026-08-14 call that `image` is for the image and the supporting
services belong to `serve`, which is the local dev toolkit. That call was recorded and never carried out.

## What compose actually does - verified, do not re-derive

Three behaviours were tested on 2026-08-15 with compose v5.4.0. Each decides part of the shape above, and two
of them contradict what this repo previously believed.

### 1. `include:` resolves the included file's relative paths against that file's own directory

`-f a.yml -f b.yml` resolves every relative path against the **first** file. `include:` resolves each file's
paths against **itself**. Same two files, same bind source `./azurite`, two different answers:

```
include:        .../inctest/sub/azurite      <- the included file's own folder
-f outer -f in  .../inctest/azurite          <- the first file's folder
```

**This retires the folder trap.** The 2026-08-07 attempt to move these files into a subfolder was reverted
because the shared azurite bind silently stopped being shared. That failure was real, and it was an `-f`
failure. `include:` does not have it. A subfolder is now safe - it is just not being used, because flat beside
the `.just` files is easier to read.

### 2. An including file can override an included service, key by key

The outer file redeclares only the keys it changes. Everything else carries through:

```
base.yml   app: environment: DB=sqlite, KEEP=me
full.yml   app: environment: DB=postgres
result     app: environment: DB=postgres, KEEP=me
```

This is what makes `image.full.yml` ten lines instead of a second copy of the app.

### 3. One variable switches between a named volume and a bind, and a missing `./` fails loudly

```yaml
volumes:
  - ${BINACLE_DATA_DIR:-app_data}:/app/data/
```

Unset gives `type: volume, source: app_data`. Set to `./data` gives `type: bind` at the resolved absolute path,
and the top-level `volumes: app_data:` declaration is dropped from the resolved project - **no orphan volume is
created.**

Set to `data`, with no `./`, compose refuses to start:

```
service "app" refers to undefined volume data: invalid compose project
```

That is the good outcome. A path without `./` is read as a volume name, and it is caught before anything runs
rather than writing data somewhere nobody looks.

### 4. `-p` overrides the file's own `name:`

This is what lets one file serve two stacks without either inheriting the other's containers:

```
-p binacle-net-volume   ->  project binacle-net-volume, /app/data on the named volume
-p binacle-net-bind     ->  project binacle-net-bind, /app/data on the resolved path
```

**By default the volume name follows the project too** - `-p binacle-net-volume` alone produces
`binacle-net-volume_app_data`. That is the behaviour the fixed names above exist to switch off, and it is the
reason they are needed at all: containers should be per-stack, data should not be.

An included file's `name:` is ignored in favour of the including file's, which was confirmed in test 2 above -
so `image.full.yml` naming itself is enough and `image.local.yml` keeping a `name:` costs nothing.

### 5. A volume with a fixed `name:` is shared across projects, and `-v` still wipes it

Two compose files, two different project names, the same `name: binacle-shared-test`. Project A wrote a marker
file; **project B read it back.** One volume, one dataset, no `external: true` needed and no pre-creation step.

`docker compose down -v` still removes it - the fixed name does not make it permanent:

```
down -v, nothing else using it   ->  Volume binacle-shared-test Removed
down -v, other project still up  ->  Volume binacle-shared-test Resource is still in use
```

**Both outcomes are legible.** The second is a printed refusal, not a silent skip, so a `-v` that did not wipe
tells you why. What compose does *not* do is warn you that the volume you are about to drop is shared - that is
what the wipe lines in the docs are for.

## The bare `up` default - open, and not an agent's call

**`just image up` with no stack name runs `full` today. Should it run `bind` instead?**

Raised 2026-08-15 and deliberately left open - the maintainer said they had not decided. **Do not decide it in
their place**, and do not read the argument below as a recommendation; it is both sides written down so the
decision is cheap when it is taken.

- **For `bind`:** it is the stack in daily use. A default that is not the common case is a default nobody
  benefits from, and `full` is the expensive one - four containers instead of one.
- **For leaving it:** `full` has been the default long enough to be muscle memory, and it is the stack that
  exercises the whole image, which is what the module exists for. A bare `up` that quietly starts something
  smaller than it used to is the kind of change that is noticed once, at the worst moment.

**If it is still open when this work is done, leave the default at `full`.** Changing it is one word in
`image.just` plus its header comment and the tooling README, so it costs nothing to do later and something to
get wrong now.

## Step by step

1. **Rename `docker-compose.yml` to `serve.services.yml`.** Content unchanged. Update the two `docker compose
   -f` lines in `tooling/serve.just` and the header comment inside the file itself.

2. **Rename the `services` recipe to `services-up`.** One line in `tooling/serve.just`, then every place that
   types it - listed at the end of this file. It is the only rename here that a maintainer's muscle memory
   notices, and `just --list serve` is the thing that corrects them.

3. **Write `image.local.yml`** from `docker-compose.volume.yml`, changing one line to
   `- ${BINACLE_DATA_DIR:-app_data}:/app/data/`. Keep `name: binacle-net-volume` as the fallback for a bare
   `docker compose -f` run. Fold in the sentence `docker-compose.bind.yml` carries that the volume file does
   not: the SQLite file lands in the bound folder, so it can be opened from the host. Delete both old files.

4. **Write `image.full.yml`** as the include-and-override file above. Delete `docker-compose.build.yml`.
   Everything in it except the app's postgres connection string and the telemetry wiring already exists in the
   other two files - copy nothing across that does.

5. **Rework `_compose` in `tooling/image.just`.** Its `case` keeps all three names and now maps each to a file,
   a project name and an environment, instead of a file alone:

   ```
   full    image.full.yml    -p binacle-net-full
   volume  image.local.yml   -p binacle-net-volume
   bind    image.local.yml   -p binacle-net-bind    BINACLE_DATA_DIR=${BINACLE_DATA_DIR:-./data}
   ```

   The unknown-name reject stays exactly where it is; it is the only place a typo is caught.

   **Give both named volumes a fixed `name:` while you are in these files** - `binacle-net-postgres` in
   `serve.services.yml`, `binacle-net-data` in `image.local.yml`. Then tell the maintainer their existing
   volumes are orphaned, once, before they find an empty database themselves.

6. **`_prepare` keeps all three arms** and changes only in `bind`, which must resolve `BINACLE_DATA_DIR` the
   same way `_compose` does. Two places defaulting it differently is how the folder gets prepared and the
   container writes somewhere else.

7. **Update the module header comments** in `tooling/image.just` and `tooling/serve.just` - both name the old
   stacks and the old recipe, and `image.just`'s header states a charter sentence that this change edits.

8. **Update the docs.** Listed below.

## Traps

**`bind` is the maintainer's primary stack** - stated 2026-08-15. It is not a legacy arm to be quietly dropped
or hidden behind an environment variable. Its niche is reading packing-log files written *by the image* in an
editor, which `just serve api` does not cover because that runs from source. **`just image up bind` must keep
working exactly as it does today**, and comparing it against `volume` must stay a matter of changing one word
on the command line.

**`include:` puts the backing services in the including project.** `just image up full` runs postgres under the
`binacle-net-full` project, not under `binacle-net-services`. **The fixed volume names above are what stop that
mattering** - without them the same declared file would produce two databases under two project-prefixed
volumes.

One consequence is left, and it was reasoned rather than run: if `just serve services-up` is already running,
`image up full` should collide on port 5432 and fail. **Expected, not tested** - confirm it fails loudly rather
than starting something half-connected, and if it does not, say so in the recipe's error path.

**One database means one wipe.** `just serve services-down -v` and `just image down full -v` now empty the same
postgres. Today they empty two different ones, so `-v` in one place could not surprise you in the other. That is
the price of sharing and it is worth paying, but the wipe lines in both compose headers and in
`tooling/README.md` have to say which database they mean.

**Postgres never gets a bind mount.** It chowns its data directory to its own user and locks it to 0700, which
leaves a folder in the repo you cannot read - and `docker build` walks the whole context, so the next build
fails on it. The named volume is deliberate. This survives every rename here.

**`tooling/README.md` gives a `docker compose -f ./tooling/docker-compose.volume.yml cp ...` command** for
reading data out of the named volume. That filename is going away and the command has to be rewritten, not just
repointed - the volume is now conditional on `BINACLE_DATA_DIR` being unset.

## What names these files, and has to change with them

**Grep for both at once** - `docker-compose` and `serve services` - or the recipe rename gets missed in files
the filename rename does not touch. `tooling/tests.just` is exactly that case: it names the recipe in a comment
and no compose file at all.

Outside `.agents/`:

- `tooling/image.just` - the `case` in `_compose`, the `_prepare` arms, the header comment
- `tooling/serve.just` - the recipe name, two `docker compose -f` lines, the header comment
- `tooling/tests.just` - a comment telling you to bring the services up before the Postgres and AzureStorage
  leaves. Recipe name only
- `tooling/README.md` - the Serve section, the Tests section, the Image stacks section and the Container data
  section, which names each stack and what it needs prepared
- the header comments inside each compose file, which name their own filename, their sibling's, and the recipe

Inside the agent docs, described in plain words because a plan cannot point into them: the tooling reference
doc carries a table of every compose file - its module, its recipe, its project name - and all four rows
change, plus its one-line summary of the `serve` module and its list of which folders each `up` prepares. The
commands doc lists both recipes verbatim, names `tooling/docker-compose.yml` in its `check:` line, and names
the recipe once more in prose. The api tests doc tells the reader to bring the services up before two suites.
The samples doc names `tooling/docker-compose.build.yml` when it explains how the smoke stacks differ from the
shipped samples. The ci-cd design record names both `tooling/docker-compose.yml` and
`tooling/docker-compose.build.yml` in a list of places a connection string appears.

The smoke stacks under `tooling/smoke/` are untouched. They are standalone single-container files by design,
they bind nothing, and nothing here reaches them.

## Done when

- Postgres, azurite and the dashboard are declared in exactly one file, and `grep -c POSTGRES_PASSWORD` over
  `tooling/*.yml` proves it.
- `just image up full` is still one command and still brings up the app against postgres with telemetry
  flowing.
- `just image up volume` and `just image up bind` behave exactly as they do today, from the same file, and
  neither disturbs the other's container or data.
- `docker volume ls` shows exactly two Binacle volumes, `binacle-net-postgres` and `binacle-net-data`, with no
  project prefix on either.
- Data written through `just serve services-up` is still there after `just image up full`, and the reverse.
  That is the check that proves the fixed name did its job.
- `just --list serve` shows `services-up` and `services-down` next to each other.
- No file in `tooling/` is named after something it does not do.
- Nothing anywhere still says `docker-compose.build.yml`, `docker-compose.volume.yml`, `docker-compose.bind.yml`
  or `just serve services`.

## Do not

- Give postgres a bind mount, in any file, for any reason.
- Use `-f a.yml -f b.yml` to compose these together. It resolves relative paths against the first file, which
  is the exact failure that got the 2026-08-07 attempt reverted.
- Have `image.just` call a recipe in `serve.just`, or the reverse. The few lines of `mkdir` and `chmod` are
  copied on purpose - a module reaching into another one puts back the coupling that splitting them removed.
- Let `image.local.yml` itself default `BINACLE_DATA_DIR` to a path. **The recipe sets it for `bind` and
  nothing else.** Inside the file, unset must resolve to the named volume - otherwise a bare
  `docker compose -f tooling/image.local.yml up`, or `up volume`, starts leaving container-owned folders in the
  repo, and one of those fails the next `docker build`.
- Drop `-p` and rely on the file's `name:`. Both stacks would land in one project, `up bind` would recreate the
  `volume` container, and the named volume would change name and read as an empty database.
