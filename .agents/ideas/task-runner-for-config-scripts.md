# Idea: a task runner for the config scripts (with shell completion)

**Status (2026-07-26):** Partly overtaken. `just` was picked and a `justfile` exists at the repo root, but it
covers only the docs and web dev loops and says so: "The rest stays in `config/*.sh` until we know what we
actually want out of `just`." So the tool question below is **settled**; what remains open is whether the test,
coverage, benchmark and build scripts get recipes too.

Read this alongside the CI plan for one shared set of scripts - that plan needs one entry point per job callable
from both a laptop and a runner, which is the same front door this idea describes. Decide them together or they
will produce two competing ways to run a build.

Originally prompted by wanting tab-completion for `config/tests.api.sh`.

## The problem

`config/` holds ~10 dev scripts (`tests.api.sh`, `tests.lib.sh`, `tests.vipaq.sh`, `coverage.sh`,
`benchmarks.*.sh`, …). Two rough edges:

- **Discovery.** You have to remember the script names and their args. There is no single "what can I run" list.
- **No completion at all.** Nothing in `config/` completes anything today. The obvious fix — a hand-written
  bash completion file per script — is the trap: it **duplicates the arg list** (`core|service`,
  `Sqlite|Postgres|AzureStorage`), so changing a script's args makes the completion silently lie, and it is one
  file per script.

The general rule others follow: **completion should be a byproduct of the command definition, not a second
file kept in sync by hand.**

## The idea

One entry point that wraps the existing scripts, so `<TAB>` completes every task from one place and the arg
list lives in exactly one spot. The scripts keep doing the work — the runner is a thin front door, not a
rewrite.

Two candidates were weighed, **`just` won** and is in use:

| Tool | Completion | Install | Feel |
|---|---|---|---|
| **make** | `bash-completion` completes `make <TAB>` targets out of the box | none — already on every dev box | terse targets, crufty syntax |
| **just** | `just --completions bash\|zsh\|fish`, plus recipe-name completion | one small binary | clean, built for exactly this |

Either gives non-drifting completion and a nicer front door (`just test-service Sqlite`
vs `config/tests.api.sh service Sqlite`).

## Shape (sketch, not decided)

Recipes wrap the scripts one-to-one, so no logic moves:

- `test-api` → `config/tests.api.sh`
- `test-service <infra>` → `config/tests.api.sh service <infra>`
- `test-lib`, `test-vipaq`, `test-shared`, `coverage`, `bench-lib`, `bench-vipaq` → their scripts

CI could later call the same recipes, so local and CI share one list of commands.

## Open questions

- ~~**make or just**~~ — settled, `just` is in use for the docs and web loops.
- **Does the rest move in at all?** The justfile deliberately stopped at the dev loops. The CI work wants one
  entry point per job anyway, so the real question is whether those entry points are recipes or stay as
  `config/*.sh` that a recipe calls.
- **Wrap or absorb** — keep `config/*.sh` as the implementation and have recipes call them (lean, no rewrite),
  or fold simple scripts into the runner over time. Start by wrapping.
- **Naming** — recipe names (`test-service` vs `test:service`), and whether the infra is a positional arg or a
  named one (`just test-service Sqlite` vs `just test-service infra=Sqlite`).

## Don't

- Don't rewrite script logic into the runner. Keep the runner thin; the scripts stay the source of truth for
  how each task runs.
