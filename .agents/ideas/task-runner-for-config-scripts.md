# Idea: a task runner for the config scripts (with shell completion)

**Status:** Unvetted idea. Nothing adopted. Prompted by wanting tab-completion for `config/tests.api.sh`.

## The problem

`config/` holds ~10 dev scripts (`tests.api.sh`, `tests.lib.sh`, `tests.vipaq.sh`, `coverage.sh`,
`benchmarks.*.sh`, …). Two rough edges:

- **Discovery.** You have to remember the script names and their args. There is no single "what can I run" list.
- **Completion drifts.** The only way to tab-complete today is a hand-written bash file
  (`config/completions/tests.api.bash`) that **duplicates the arg list** (`core|service`, `Sqlite|Postgres|
  AzureStorage`). Change a script's args and the completion silently lies. It is also per-script — one file each.

The general rule others follow: **completion should be a byproduct of the command definition, not a second
file kept in sync by hand.**

## The idea

One entry point that wraps the existing scripts, so `<TAB>` completes every task from one place and the arg
list lives in exactly one spot. The scripts keep doing the work — the runner is a thin front door, not a
rewrite.

Two candidates:

| Tool | Completion | Install | Feel |
|---|---|---|---|
| **make** | `bash-completion` completes `make <TAB>` targets out of the box | none — already on every dev box | terse targets, crufty syntax |
| **just** | `just --completions bash\|zsh\|fish`, plus recipe-name completion | one small binary | clean, built for exactly this |

Both give non-drifting completion and a nicer front door (`just test-service Sqlite` or `make test-service`
vs `config/tests.api.sh service Sqlite`).

## Shape (sketch, not decided)

Recipes wrap the scripts one-to-one, so no logic moves:

- `test-api` → `config/tests.api.sh`
- `test-service <infra>` → `config/tests.api.sh service <infra>`
- `test-lib`, `test-vipaq`, `test-shared`, `coverage`, `bench-lib`, `bench-vipaq` → their scripts

CI could later call the same recipes, so local and CI share one list of commands.

## Open questions

- **make or just** — zero-install vs cleaner UX. `make` wins if "no new tooling" matters most.
- **Wrap or absorb** — keep `config/*.sh` as the implementation and have recipes call them (lean, no rewrite),
  or fold simple scripts into the runner over time. Start by wrapping.
- **Retire the hand-written completion** — `config/completions/tests.api.bash` becomes redundant once the
  runner completes its own tasks; delete it then.
- **Naming** — recipe names (`test-service` vs `test:service`), and whether the infra is a positional arg or a
  named one (`just test-service Sqlite` vs `just test-service infra=Sqlite`).

## Don't

- Don't rewrite script logic into the runner. Keep the runner thin; the scripts stay the source of truth for
  how each task runs.
