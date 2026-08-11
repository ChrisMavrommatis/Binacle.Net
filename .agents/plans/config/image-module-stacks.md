# Decide what the `image` module is still for

**Status:** Not started, and **nothing has been changed**. `config/image.just` and its three stacks -
`docker-compose.build.yml`, `docker-compose.volume.yml`, `docker-compose.bind.yml` - are exactly as they have
always been. This file records what a 2026-08-07 session found when it examined them, so the next session does
not rediscover it.

Not urgent and not part of v3.0.0. It is maintainer tooling: no user copies it, nothing ships it, and nothing
in CI calls it.

## What changed around it

The smoke suite now runs five profiles and `just smoke up <profile>` **leaves the stack running** until you
take it down. That covers "bring the image up in a known configuration and poke at it", five ways, with no
folder setup because smoke throws its storage away.

That overlaps two of the three stacks:

| Stack | Still unique? |
|---|---|
| `volume` | **No.** The image alone with every module on and SQLite in a named volume is `just smoke up full`, near-identically. Smoke's version also has `/_debug` on and a raised rate limit, which for manual poking is better, not worse |
| `bind` | **Barely.** Its one claim is data you can open in an editor. It is also the stack carrying the whole mkdir/chmod/sudo-to-wipe tax, and the one that can leave a container-owned directory in the repo and break the next `docker build` |
| `full` (`docker-compose.build.yml`) | **Yes.** The only stack that runs the image against Postgres and Azurite rather than SQLite, and the only one wired to a telemetry collector. Smoke is SQLite-only by design and carries no dashboard |

`volume` and `bind` differ from each other only in where `/app/data` goes. That is one file with a variable, not
two files.

## The decision - no longer blocked

This waited on the samples landing. **They landed 2026-08-07, and the answer went the other way from what the
wait expected.** `service` does carry commented Postgres and Azure Storage connection strings, but they point at
*your own* database - the sample says in as many words that a production deployment should not run its database
in the same compose file, so uncommenting one brings up no backend. Nothing in `samples/` starts a Postgres or an
Azurite.

`docker-compose.build.yml` does: `docker compose config --services` lists `postgres`, `azurite`,
`aspire-dashboard` and `binacle-net` - it is the services stack plus the image. That is a different question
from the one the samples answer, so **`full` earns its keep and outcome 3 is off the table.** Only `volume` and
`bind` need resolving.

The two live outcomes, ranked:

1. **Keep `full` only.** Delete `volume` and `bind`; `up`/`down` lose their positional stack name entirely.
   Simplest, and the one the overlap argues for.
2. **Keep `full`, merge `volume` and `bind` into one file** with `BINACLE_DATA_DIR` choosing volume-vs-bind.
   Keeps the on-disk-data option at the cost of one file rather than two.

~~Delete the module.~~ Ruled out above - the samples do not cover the multi-backend case and were never going
to, because pointing a reader at their own database is the right advice for a sample.

Anything that deletes `bind` should first check the maintainer has not been using it - reading packing-log
files written *by the image* is its real niche, and `just serve api` covers that only for a from-source run.

## If the files are ever reorganised - do not use a subfolder

This was tried on 2026-08-07 and reverted. Compose resolves relative bind sources against **the compose file's
own directory**. `docker-compose.build.yml` binds `./data` and `./azurite`, and `docker-compose.yml` binds
`./azurite` too - today both mean `config/azurite`, so the services stack and the image stack share emulator
state. Moved into `config/image/`, they silently stop sharing and you get two Azurites, with nothing failing to
tell you.

Putting the bind targets inside the subfolder instead does not help. `.gitignore` covers `config/data/` but not
a nested copy, and a container-owned unreadable directory inside the build context fails the next
`docker build` while the CLI walks it - which is the exact failure the comments in those files already warn
about.

A **flat rename** - `image-full.yml`, `image-volume.yml`, `image-bind.yml` - gets the same tidiness with none of
that, and collapses the name-to-file `case` in `_compose` to one interpolated path. That is the shape to take
if the goal is only tidiness.

## Done when

The module is either justified in one sentence or gone, and no stack in it duplicates a smoke profile. Delete
this file then.
