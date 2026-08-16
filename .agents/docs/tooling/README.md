---
id: "tooling"
description: "tooling/ — every task the repo can run, called by CI and by hand alike: the test, coverage, openapi, agents, regen, changelog, serve, build, image and smoke modules for just, the benchmark/performance scripts, the tmux script, the local compose stacks, and emulator state"
verified: "2026-08-17"
check: "Script list, tests.just leaves, coverage.just recipes, openapi.just, agents.just, regen.just, changelog.just, serve.just, build.just, image.just (stacks and the four verify checks, whose certificate identity must match SECURITY.md) and smoke.just recipes, and the compose stack/file/service table match tooling/"
also_update:
  - commands
  - samples
paths:
  - "tooling/**"
---

# Tooling

`tooling/` holds **every task the repo can run** — the just modules for tests, coverage, OpenAPI, the
agent indexes, the build, the image stacks and the smoke suite, plus the benchmark scripts, local Docker
Compose and emulator state. CI calls these same recipes rather than keeping its own copy, so a workflow step
and a maintainer typing the command do the same thing. It is **not** a deployment template; user-facing
deployment starting points live in samples (`$samples`). For the quick "how do I run X" reference see
`$commands`; this doc describes what's in the directory.

## Scripts and `just` modules (run from the repo root)

| Script | What it does |
|---|---|
| `serve.just` | **Not a script** — the `serve` module for the root `justfile`, everything run **from source**. `just serve api [profile]` runs the API via `dotnet run -lp <profile>` (`Normal`/`WithServiceModuleOnly`/`WithUiModuleOnly`/`WithAllModules`, aliases `N/S/U/All`, default `Normal`); `just serve docs` and `just serve web` run jekyll + webpack watch together; `just serve services-up [-d]` / `just serve services-down` bring up what the API talks to |
| `tests.just` | **Not a script** — the `test` module for the root `justfile`. One recipe per suite, run with `just test <leaf>`; see `$commands` for the list |
| `performance.<slice>.sh` | `dotnet run -c Release` for the slice's `PerformanceTests`. Slices `lib`, `vipaq`. Writes to gitignored `PerformanceTests.Artifacts` |
| `benchmarks.<slice>.sh [alias]` | `dotnet run -c Release --filter <pattern>` from the slice's `Benchmarks` project. Slices `lib`, `vipaq`. No arg = all |
| `build.just` | **Not a script** — the `build` module for the root `justfile`. `just build publish` publishes the API (`-c Release -o artifacts/binacle-net --no-self-contained --runtime linux-x64`); `just build image [version]` publishes then `docker build -t binacle-net:<version>` (default `local`), applying the three per-build OCI labels. Neither starts compose, and neither needs `sudo`, so CI calls both as they stand — see `$ci-cd` |
| `coverage.just` | **Not a script** — the `coverage` module for the root `justfile`. Runs the test leaves with the collector attached and writes to gitignored `artifacts/tests/` + `artifacts/coverage/`; see `$commands` |
| `openapi.just` | **Not a script** — the `openapi` module for the root `justfile`. `just openapi generate [dir]` builds the v3/v4 documents into gitignored `artifacts/openapi/`, `just openapi lint [dir]` generates then Spectral-lints them against `.spectral.yaml` |
| `agents.just` | **Not a script** — the `agents` module for the root `justfile`. `just agents all` regenerates the `_index.md` manifest for `.agents/docs`, `.agents/design`, `.agents/plans`, `.agents/memory` and `.agents/ideas` (grouped by area); `just agents generate-index <name>` does one |
| `regen.just` | **Not a script** — the `regen` module for the root `justfile`. The four generators whose output is **committed**: `just regen or-lib-scenarios` (OR-Library text → `shared/data/bischoff-suite`), `just regen vipaq-packed-data` (that plus `custom-problems`, packed → `vipaq/data/packed`), `just regen vipaq-interop-vectors` (the C# and TS interop halves plus the header bytes → `vipaq/test-vectors`), `just regen all` in dependency order, and `just regen check` which runs `all` then fails if any generated `.json` moved. None takes an argument — each tool runs every generator in its list so it cannot half-run. **No workflow calls `check`** |
| `changelog.just` | **Not a script** — the `changelog` module for the root `justfile`. Reads `CHANGELOG.md` at the repo root. `just changelog extract <version\|Unreleased>` prints one release's section, with its headings promoted from `###` back to `##` for a release body; `just changelog check <version\|Unreleased>` exits 1 if that section is missing or empty. The release workflow calls both, so CI and a laptop parse the file the same way and the exact body can be previewed before a tag is pushed — see `$ci-cd/release-pipeline` |
| `image.just` | **Not a script** — the `image` module for the root `justfile`. Runs what `build.just` produced: `just image up [full\|volume\|bind]` (default `full`) and `just image down [name]`; extra arguments pass through to `docker compose`. `up` creates and opens the bind-mounted folders first, and every stack stops with a pointer to `just build image` if `binacle-net:local` is missing. **`just image verify <version> [check]` is the odd one out** — it reads a *published* image off Docker Hub, builds nothing and never logs in; see below |
| `smoke.just` | **Not a script** — the `smoke` module for the root `justfile`. Tests the image rather than the code. `just smoke test-structure [image]` runs `container-structure-test` against `tooling/smoke/structure.yaml`; `just smoke test <profile> [image]` does up → hurl → down for one profile; `just smoke up`/`down` are the manual halves; `just smoke all [image]` builds, checks the structure once, then runs every profile. Every recipe takes the image last, default `binacle-net:local`, so a published tag can be smoked too |
| `tmux.sh` | Builds/re-attaches the `binacle` tmux session (windows `api`/`docs`/`web`/`tests`/`misc`/`bench_1..3`); panes are pre-`cd`'d, nothing auto-runs |

The launch profiles live in `serve.just`; the benchmark filters live inside the per-slice `benchmarks.*`
scripts. `tmux.sh` is standalone — it has no aliases.

The TS leaves (`just test shared-ts-unit`, `just test vipaq-ts-unit`) run jest from the repo root. Run
`just install` first — it does the root `npm install` (the packages are npm workspaces, so one install covers
them all), `bundle install` for both jekyll sites, and copies `assets/` into `docs/` and `web/`.

## Local Docker Compose

**Three files, each named after the module that runs it.** `serve.services.yml` brings up what the app talks
to and no binacle-net at all, so it belongs to `serve`, alongside `just serve api` (and it is what the
Postgres/AzureStorage test leaves need). The two `image.*.yml` files follow `just build image` and answer a
different question — does the shipped image work — so they are the `image` module's stacks, and that is why
only they check for `binacle-net:local`. The five under `smoke/` answer a narrower question again — does it
work *as configured* — and are driven entirely by `just smoke`, never by hand.

**Three stacks come out of two image files.** `volume` and `bind` are one container differing only in where
`/app/data` goes, so `image.local.yml` serves both and `_compose` picks with `-p` and `BINACLE_DATA_DIR`.
`image.full.yml` `include:`s that file and `serve.services.yml`, then overrides the app's storage and
telemetry — so postgres, azurite and the dashboard are declared once, in `serve`.

| File | Module | Command | Project name | Runs |
|---|---|---|---|---|
| `serve.services.yml` | `serve` | `just serve services-up` | `binacle-net-services` | **Backing services only** — `aspire-dashboard`, `azurite`, `postgres`. No API. The only place those three are declared |
| `image.full.yml` | `image` | `just image up full` | `binacle-net-full` | **Full** — `include:`s the other two files and overrides the app's storage and telemetry, so it is about twenty lines. Local image + `azurite` + `postgres` + `aspire-dashboard`, all modules on; injects `OpenTelemetry.Production.json` on top of the `JwtAuth.json` it inherits. All three storage backends run; Postgres wins on provider order, swap by moving the comment. The `image` module's default |
| `image.local.yml` | `image` | `just image up volume` | `binacle-net-volume` | **Simple** — the local image alone, ServiceModule on SQLite, data in the named volume `binacle-net-data` |
| `image.local.yml` | `image` | `just image up bind` | `binacle-net-bind` | **Simple** — the same file, with `BINACLE_DATA_DIR` set by the recipe so `/app/data` is a bind at `tooling/data`. Compose then drops the volume declaration, so this stack leaves none behind |
| `smoke/<profile>.yml` | `smoke` | `just smoke up <profile>` | `binacle-smoke-<profile>` | **Five throwaway stacks** — `minimal`, `quickstart`, `prod`, `service`, `full`, one per smoke profile, and each name is also a `samples/docker/` folder. Storage is a named volume dropped on teardown, so they need no `_prepare`. They take the image from `$BINACLE_IMAGE` (default `binacle-net:local`); `service`/`full` inline `JwtAuth.json` and raise `RateLimiter__ApiUsageAnonymous` so a second run inside the hour does not go red on 429s; `prod` mounts its own `Presets.json` so reading it back proves the config-mount path |

Each file carries its own `name:` as a fallback, but `image.just` passes `-p` — two stacks share one file, so
without it `up bind` would recreate the `volume` container. Inside `image.just` the stack name maps to a file,
a project and an environment in one place, so `up` and `down` cannot disagree about which one it means;
`smoke.just` gets the same guarantee for free, since the profile name **is** the filename.

**Both named volumes carry a fixed `name:`** — `binacle-net-postgres` and `binacle-net-data` — so compose does
not prefix them with the project. That is what makes `serve services-up` and `image up full` one database
rather than two that look alike, and it means `-v` in either place wipes it for both. The two also publish the
same 5432, so they cannot run at once; the second one fails on the port, loudly, and leaves the first alone
(checked 2026-08-15).

### What compose does here — tested 2026-08-15 against compose v5.4.0

Four behaviours the shape above rests on. They were **run, not reasoned**. Do not re-derive them.

- **`include:` resolves an included file's relative paths against that file's own directory.** `-f a.yml -f
  b.yml` resolves every path against the **first** file instead. Same two files, same `./azurite` source, two
  different answers. **This retires the subfolder trap**: the 2026-08-07 attempt to move these files into a
  subfolder was reverted because the shared azurite bind silently stopped being shared — that was an `-f`
  failure, and `include:` does not have it. A subfolder is safe now; it is simply not used, because flat
  beside the `.just` files reads better.
- **An including file overrides an included service key by key.** Everything it does not name carries through,
  which is why `image.full.yml` is twenty lines rather than a second copy of the app. An included `name:` is
  ignored in favour of the including file's.
- **`${BINACLE_DATA_DIR:-app_data}:/app/data/` is what switches a named volume for a bind.** Unset gives
  `type: volume`; `./data` gives `type: bind` at the resolved absolute path **and drops the top-level
  `volumes:` declaration**, so the bind stack leaves no orphan volume. A value with no leading `./` is read as
  a volume name and refuses to start — `refers to undefined volume data: invalid compose project` — which is
  the good outcome: it fails before anything writes data where nobody looks.
- **A fixed volume `name:` is shared across projects** with no `external: true` and no pre-creation step.
  `down -v` still removes it when nothing else references it, and prints `Resource is still in use` when
  something does. Both are legible; what compose does **not** do is warn that the volume being dropped is
  shared by another stack.

**A bare `just image up` means `full`.** Raised and settled on 2026-08-15: it stays the expensive stack,
because that is the one that exercises the whole image, which is what the module is for.

### Do not

- **Give postgres a bind mount**, in any file, for any reason. It chowns its data directory to its own user
  and locks it to 0700, leaving a folder in the repo nobody can read — and `docker build` walks the whole
  context, so the next build fails on it. The named volume is deliberate and survives every rename.
- **Compose these together with `-f a.yml -f b.yml`.** Path resolution, above. That is the exact failure that
  got the 2026-08-07 attempt reverted.
- **Have `image.just` call a recipe in `serve.just`, or the reverse.** The `mkdir` and `chmod` lines are
  copied on purpose.
- **Let `image.local.yml` default `BINACLE_DATA_DIR` to a path.** The recipe sets it for `bind` and unsets it
  for the other two. Inside the file, unset must stay the named volume — otherwise a bare `docker compose -f`
  run starts leaving container-owned folders in the repo, and one of those fails the next `docker build`.
- **Drop `-p`** and rely on the file's own `name:`. Two paragraphs up for why.

## Verifying a published image

`just image verify <version> [check]` in `image.just`, with the four checks as private `_verify-*` recipes.
Each is one question and the order matters — every one answers something the next assumes.

**Docker Hub only.** GHCR is the release workflow's staging registry and nothing outside that workflow reads
it, so this recipe knows one repository. It carried a fifth check until 2026-08-15, `digest`, which compared
the tag across both registries.

| Check | What it proves |
|---|---|
| `tags` | The Docker Hub tag map, from the v2 API. Rows sharing a digest are one image under several names — how you see what `latest` resolves to. The **date** is the trap: it moves for reasons that are not a retag, so it is printed and never compared |
| `signature` | `cosign verify` against the Docker Hub tag. A signature is a referrer stored beside the image, not inside the index, so it does not survive `imagetools create` and the pipeline signs after the copy as well as before it. Needs `cosign`; fails with a pointer when it is missing |
| `attestations` | The SPDX SBOM package count and the SLSA provenance builder id. Both are manifests **inside** the index, so the index digest hashes them and the one signature already covers them — nothing extra to verify, this only reports what is attached |
| `metadata` | The three OCI labels, then a throwaway run: `BINACLE_VERSION`, the uid, `/app/data`'s owner, and the `System.*.dll` count in `/app`. That count is the **framework-dependent proof** — 4 on a framework-dependent build, ~170 on a self-contained one |

**The version argument is required and never defaults**; a default rots into a tag nobody meant to check.
**No `docker login` anywhere** — these are the commands a user runs, and a check that only passes with a
credential is not checking a public artifact. The aggregate does **not** use `set -e`: it runs all four, ORs
the exit codes and fails at the end, because the first failure otherwise hides the three answers explaining it.

**Two just traps live in this recipe** and both cost real time:

- **A Go template needs four braces open, two closed** — `{{{{ json .SBOM }}`. Two opening braces are just's
  own interpolation. Four closing braces emit a literal `}}` on the end of every value, which still looks
  right: piping into `jq` gives `parse error: Unmatched '}'` under a correct-looking answer.
- **A backtick in a recipe-body comment is executed by just**, before the shell ever sees the line. Found
  2026-08-15 by writing the brace explanation with backticks around the braces; the recipe died on
  `Backtick failed with exit code 2`. Explain punctuation in words down there, not in code spans.

**Only `3.0.0-beta.3` and later can pass.** The recipe matches the signature against the `binacle-labs`
certificate identity, so it accepts only images signed after the repository moved. `3.0.0-beta.2` **is**
signed, but under the old identity, and fails `signature` for that reason alone. `3.0.0-beta.1`, `2.1.1` and
everything earlier were never signed and fail both `signature` (`no signatures found`) and `attestations`
(no SBOM). Neither case is a broken check. It also binds every user-facing surface: an example naming a tag
must name one that passes **today**, and `latest` stays unsigned until v3.0.0 publishes.

**`3.0.0-beta.3` is the reference tag** — the one to re-run against and the one to name in an example until
v3.0.0 is out. Green on all four checks, 2026-08-17: one tag row, signed by the release workflow under the
new identity, a 167-package SBOM, and provenance naming the build run. `2.1.1` is still the tag to watch it
fail against — no signature, no SBOM, and 172 System dlls where a framework-dependent build carries 4.

The cosign invocation was proven against **cosign 3.1.3**, the version `DEVELOPMENT.md` pins. Both flags were
kept; dropping the identity would make the check ask only whether anyone signed the image.

The smoke stacks are separate files from `samples/` on purpose. They run the image under test and carry
test-only tweaks — a raised rate limit, disposable storage — that a sample a user copies must never have.

**`tooling/smoke/README.md` is the authority on that suite** — what each profile claims, the two rules that
decide whether a check belongs in it (`assert what the image contains and wires, never what the algorithm
computed`; `every check must be able to fail`), and the setup gotchas. It is written for a human, and it is
where the design rationale went when the smoke plan was deleted on 2026-08-07. Read it before changing an
assertion; do not re-derive any of it here.

Which folders `up` prepares: `serve services-up` needs `tooling/azurite`; `image up full` needs that same one
and nothing more, since its `/app/data` is the named volume; `image up bind` needs `BINACLE_DATA_DIR` (default
`tooling/data`); `image up volume` needs none. It opens the **directory** only, never `-R` — the files inside
belong to whoever wrote them (the app as `APP_UID`, azurite as root) and stay writable to that writer, so a
recursive `chmod` would fail on them while making nothing more writable. `sudo` is used only for a directory
docker created itself, which the daemon makes as root. The few lines that do this are **copied** into both
modules rather than shared: a module reaching into another one restores the coupling the split removed.

## Emulator state
- `tooling/azurite/` holds Azurite emulator state (`__azurite_db_*__.json`).
- `tooling/tooling.proj` is a `Microsoft.Build.NoTargets` content project (no compile) that includes the config
  files in the solution — see `$build-topology`.

## The folder itself

Renamed from `config/` on 2026-08-12, and `build/` became `artifacts/` in the same change. The old names were
each wrong in a different way: `config/` held recipes rather than configuration, while the API's own runtime
settings live in `Config_Files`; `build/` held output, which the .NET convention calls `artifacts/` and the Go
convention reads as the opposite — build *scripts*. `eng/` was considered and rejected as jargon, `tooling/`
being the plain word for what it holds. `artifacts/` is not `results/`: that one is committed measured
evidence, records that outlive a build.

**Every module sets `set working-directory := '..'`, which resolves relative to the module file.** The folder
therefore has to stay one level below the repo root. Moving it deeper or shallower breaks every path in every
recipe at once, and does it silently — the recipes still parse.

**References point one way: out of `tooling/`, never into it.** This folder may name anything; almost nothing
may name it. The exceptions are (a) whatever operates on it — the `justfile` `mod` lines, `Binacle.Net.slnx`,
the `.gitignore` state-dir patterns, the `/s:` argument in `sonar-analysis.yml`; (b) this `.agents/` layer,
whose job is describing the repo; (c) the repo's own top-level docs, `README.md`, `DEVELOPMENT.md` and
`CLAUDE.md`, because something has to say the folder exists. **Never a comment**, and never a file a user
copies — a sample that names a maintainer's path is handing a reader something they cannot use. A comment that
needs to talk about this folder is a briefing, not a trap, and belongs here or in design instead.
