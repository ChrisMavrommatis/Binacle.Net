# Top-level layout: `config/` becomes `tooling/`, `build/` becomes `artifacts/`

**Status:** Not started. Decided 2026-08-12. Two renames in one file because they collide on the same set of
files - the `.just` modules, `sonar-analysis.xml` and `.gitignore` are edited by both - so doing them in
separate sittings means touching those files twice and reviewing the same diff twice.

## Why

The repo has three folders whose names fight each other.

- `.config/` holds `dotnet-tools.json`. The .NET SDK looks for the tool manifest at exactly that path, so the
  name is not ours and the folder does not move. Both pinned tools are live: `dotnet-sonarscanner` is restored
  and run by `.github/workflows/sonar-analysis.yml`, `dotnet-reportgenerator-globaltool` by the report recipe.
- `config/` holds the `just` modules, the compose stacks, the smoke data and the loose scripts. Its README calls
  it "the maintainer's local-dev tooling", which stopped being true: CI calls those same recipes about thirty
  times across four workflows. It is the one definition of every task, and "config" describes neither the
  recipes nor the fact that CI depends on them. Meanwhile the docker samples mount `./config:/app/config` for
  the API's actual runtime settings, so the word already means something else in this repo.
- `build/` holds output, everything gitignored but a README. That reading is backwards from both surrounding
  ecosystems: the .NET convention (Arcade, used by dotnet/runtime and aspnetcore) puts build output in
  `artifacts/` and infrastructure in `eng/`, and the Go standard layout uses `/build` for build *scripts*. A
  reader from either one gets this folder wrong.

`tooling/` was chosen over `eng/` on purpose. `eng/` is the .NET convention and would have paired with
`artifacts/` exactly, but it is jargon, and the repo's writing rule says use the plain word when one exists.
`artifacts/` is kept because there is no plainer word for it that is not already taken by `results/`.

## What makes this cheap

Two things, both verified before the plan was written. Do not undo either.

- **CI never names the folder.** Every workflow step calls a recipe - `just test lib-unit`,
  `just smoke test prod`, `just coverage all sonar`, `just build publish`. Exactly one step hardcodes a path
  into the folder: `sonar-analysis.yml:85`, the `/s:` argument pointing at `sonar-analysis.xml`.
- **All nine modules use `set working-directory := '..'`**, which resolves relative to the module file. A
  rename at the same depth needs no edit to any of those lines. Moving the folder to a different depth would
  break every path in every recipe.

## Phase 1 - `config/` becomes `tooling/`

Move the folder whole. Contents keep their names and their layout; nothing is split out, nothing is absorbed.
The two plans living under `.agents/plans/config/` stay plans - converting the loose scripts to recipes and
deciding what the image module is for are separate work and are not touched here beyond their path.

**Move**

- `git mv config tooling`
- `git mv config/config.proj` contents to `tooling/tooling.proj` (the project file is named after the folder).
- `git mv .agents/docs/config .agents/docs/tooling`
- `git mv .agents/plans/config .agents/plans/tooling`

**Edit - paths that are load-bearing**

- `justfile` - nine `mod` lines (11, 14, 17, 20, 23, 26, 29, 32, 35), plus the header comment that still says
  "still `config/*.sh`".
- `Binacle.Net.slnx:112` - `<Project Path="config/config.proj" Type="Shared">` becomes the new path and file
  name.
- `.github/workflows/sonar-analysis.yml:85` - `$GITHUB_WORKSPACE/config/sonar-analysis.xml`.
- `.gitignore:50-52` - `config/minio_data/`, `config/postgres/`, `config/data/`. Line 49 is a bare `azurite/`
  pattern that matches at any depth, so it needs no change.

**Edit - prose, in the repo**

`tooling/README.md` (10 refs, and its opening line needs rewriting to say what the folder now is: the task
definitions CI and a laptop both run), `tooling/smoke.just` (8), `tooling/serve.just` (7), `tooling/README.md`
under `smoke/` (3), `tooling/tmux.sh` (3), `tooling/image.just` (3), and one line each in `tooling/tests.just`,
`tooling/sonar-analysis.xml`, `tooling/openapi.just`, `tooling/coverage.just`, `tooling/build.just`,
`tooling/smoke/structure.yaml`, `tooling/docker-compose.{bind,build,volume}.yml`.

Outside it: `DEVELOPMENT.md` (2), `lib/README.md` (2), `.agents/board.md` (2), `.agents/release-v3.0.0.md` (2),
`.agents/design/ci-cd/decisions.md` (5), `.agents/memory/no-sonar-issue-ignores.md` (3), the two plans now under
`.agents/plans/tooling/` (9 and 4), and comments only in `run-tests.yml` (5), `smoke-image.yml` (2),
`release-docker-image.yml` (2).

`samples/docker/**` has nine refs across five compose files and four READMEs, all of the form "config/smoke/
<profile>.yml runs this same configuration against the image". Repo-root `samples/` is editable, so these are
in scope for this phase.

**Then** run `just agents all` to regenerate the `.agents` manifests, which pick up the two renamed area
folders and this file.

**Operational, before moving anything:** bring the compose stacks down. `git mv` does not carry the ignored
state directories - `config/minio_data/`, `config/postgres/`, `config/data/`, `config/azurite/` - and a running
container bind-mounted into a path that just moved fails in a way that looks unrelated.

## Phase 2 - `build/` becomes `artifacts/`

Runs after phase 1 lands, not beside it. Both phases edit `coverage.just`, `tests.just`, `build.just`,
`openapi.just`, `sonar-analysis.xml` and `.gitignore`.

**Move:** `git mv build artifacts`. Only `README.md` is tracked; the rest is generated and gitignored, so the
move is one tracked file plus whatever the last build left behind.

**Edit**

- `tooling/coverage.just` (12 refs), `tooling/tests.just` (8), `tooling/build.just` (5),
  `tooling/openapi.just` (2) - the output paths the recipes write to.
- `tooling/sonar-analysis.xml` (5) - the coverage and test-result paths the scanner reads.
- `.gitignore` (6): lines 28, 40, 42, 43, 65, 73.
- `Dockerfile:29` - `COPY ["build/binacle-net", "."]`.
- `.dockerignore` (2), `.spectral.yaml` (1).
- `artifacts/README.md` - rewrite the intro, and add one line saying what it is *not*: repo-root `results/` is
  tracked measured evidence, records rather than output, and does not move here. The two names sit close enough
  that a reader will ask.
- `.agents/release-v3.0.0.md` (5), `.agents/plans/ci-cd/multi-arch-images.md` (3), `.agents/plans/todos.md` (1),
  `.agents/plans/image-base-slimming.md` (1).

**Blocked on two lines that a coding session may not write.** `docs/_config.yml:27` and `web/_config.yml:27`
both set `destination : ../build/docs` and `../build/web` - Jekyll writes the generated sites there. Rename
`build/` without changing those two lines and both sites keep building into a path that is no longer in the
layout. Both files are inside the off-limits published folders and neither is a downloadable sample, so the
security carve-out does not reach them. **Phase 2 is not finished until those two lines read `../artifacts/docs`
and `../artifacts/web`.** Either the docs session makes that edit, or the human makes it by hand; a coding
session does neither.

## For the docs session

Two items, both inside `docs/`, neither doable in a coding session.

1. **Required, and phase 2 depends on it.** `docs/_config.yml:27` - change `destination : ../build/docs` to
   `destination : ../artifacts/docs`. The matching line is `web/_config.yml:27`, `../build/web` to
   `../artifacts/web`. Nothing else on either page changes. Without this the site builds into a folder the
   repo no longer has.
2. **Cosmetic, no hurry.** Five frozen sample compose files under
   `docs/collections/_versions/v3.0.x/samples/docker/{minimal,quickstart,prod,full,service}/docker-compose.yml`
   carry a line 5 comment reading `# Smoke profile: <name>. config/smoke/<name>.yml runs this same
   configuration against the image under...`. The folder is `tooling/` now, so `config/smoke/` becomes
   `tooling/smoke/` in all five. The live copies under repo-root `samples/` are fixed in phase 1; these are the
   version-frozen duplicates the docs site serves.

## Verification - each phase proves its own work

Do not hand a phase back without running these. A rename that leaves one stale path produces a failure at
release time, which is the thing this repo has already been bitten by once.

- `just --list` resolves, and every module lists: `just --list test`, `coverage`, `openapi`, `agents`,
  `changelog`, `serve`, `build`, `image`, `smoke`. A broken `mod` path makes `just` fail outright, so this
  catches the nine most important edits in one command.
- `dotnet build Binacle.Net.slnx` succeeds - this is what proves the `slnx` entry and the renamed `.proj`
  are right.
- `just test all` is green.
- After phase 2 only: `just coverage all cobertura` then `just coverage report`, and confirm the HTML report
  actually lands under `artifacts/coverage/html-report/`. This is the one path that goes through the pinned
  ReportGenerator tool and the recipe's own output directory, so it exercises both halves of the rename.
- `docker build .` succeeds after phase 2, proving `Dockerfile:29` and `.dockerignore`.
- No stale references: `grep -rn "\bconfig/" --exclude-dir=node_modules --exclude-dir=.git` returns only the
  runtime `./config:/app/config` sample mounts, `.config/dotnet-tools.json`, `webpack.config.js`,
  `jest.config.js`, `NuGet.Config`, `_config.yml` and `package-lock.json`. After phase 2, the same sweep for
  `\bbuild/` returns only `Directory.Build.props`, `dotnet build`, `docker build`, `npm run build` and the
  two off-limits `_config.yml` destination lines if they have not been fixed yet.
- `git status` shows no leftover empty directory and no untracked file that used to be tracked.

## Done when

- `config/` and `build/` do not exist, and nothing outside the exclusions above names them.
- `just --list`, the full test run and a docker build are all green from a clean clone.
- The two `_config.yml` destination lines point at `artifacts/`.
- `.agents` manifests regenerated, so the renamed area folders resolve.
