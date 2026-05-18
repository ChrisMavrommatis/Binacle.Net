# Agent Docs Improvement Plan

Fixes identified after reviewing all files in `.agent-docs/`.
Ordered by impact — do high priority items first.

---

## High Priority ✓

### ~~1. Fix stale link in `v3-vs-v4.md`~~

`api/v3-vs-v4.md` line 38 links to `.plans/v4-endpoints.md`.
That's a planning artifact, not stable documentation.

**Fix:** Inline the endpoint list directly into `v3-vs-v4.md` and remove the link.
Use the table from `.plans/v4-endpoints.md` — it's the right content, just in the wrong place.

---

### ~~2. Fill `contracts.md`~~

`api/contracts.md` has a real frontmatter description but zero content — just a gap marker.
Agents select it based on the description, then get nothing.

**Fill with:**
- `IWith*` interfaces (`IWithBin`, `IWithBins`, `IWithItems`, `IWithOperationParameters`) — what each one is and where it lives
- Request base classes and how to compose them
- Validator pattern — where validators live, how they hook into `BindingResult<T>`
- Response types for v4 — shape of fit and pack responses
- OpenAPI examples — where they live and how they're wired up

---

### ~~3. Add task-based index to root `README.md`~~

The README is organized by slice (what things are).
Agents have tasks (what they need to do).
Without a task index, agents have to infer which docs to read.

**Add a "Common Tasks" section** with read-lists, e.g.:

| Task | Read these |
|---|---|
| Add a v4 endpoint | `api/endpoints.md`, `api/add-endpoint.md`, `api/contracts.md`, `api/service.md` |
| Understand how results are selected | `lib/processors.md`, `concepts/fit-vs-pack.md` |
| Add or modify a test | `tests/README.md`, `tests/scenarios.md` |
| Work with presets | `api/presets.md` |
| Understand v3 vs v4 differences | `api/v3-vs-v4.md` |

---

## Medium Priority ✓

### ~~4. Create `api/service.md`~~

`processors.md` currently describes `IBinacleService` inline at the top.
That service is the main thing endpoint handlers call — it deserves its own file.

**Extract to `api/service.md`:**
- The `IBinacleService` method table (already in processors.md, just move it)
- When to use each method (single bin vs multi-bin vs smallest)
- Where the implementation lives (`BinacleService.cs`)

**Then update `processors.md`** to remove the service section and link to `api/service.md` instead.

---

### ~~5. Fix vague "latest version" in `algorithms.md`~~

`lib/algorithms.md` says "the API uses one version at a time (usually the latest)."
An agent writing a new endpoint doesn't know which version to use.

**Fix:** Replace the vague statement with the actual current version (e.g., "currently v2 for all three heuristics").
Add a note: "always use the latest version when writing new code."

---

### ~~6. Add ServiceModule context to `add-endpoint.md`~~

The endpoint template in `add-endpoint.md` includes `.RequireRateLimiting("ApiUsage")` and `.RequireCors(CorsPolicy.CoreApi)`.
There's no note that these only apply when ServiceModule is active.

**Fix:** Add a one-line note below the template explaining that those two lines
are ServiceModule-dependent and should be omitted if ServiceModule is not in use.

---

### ~~7. Create `api/presets.md`~~

Both v3 and v4 have preset endpoints but there's no doc explaining what presets are.
An agent working on any preset-related feature has no reference.

**Cover:**
- What a preset is (named collection of bin definitions)
- Where presets are configured (appsettings, test fixtures, etc.)
- How preset names and bin names map to the route params `{preset}/{bin}`
- How to add a new preset for testing

---

## Low Priority ✓

### ~~8. Move commands out of root `README.md`~~

The commands block (run API, tests, benchmarks, Docker) takes up most of the root README.
It pushes the slice index and navigation down.

**Fix:** Move commands to a new `commands.md` at the root of `.agent-docs/`.
Replace the block in `README.md` with a single line: `See [Commands](commands.md)`.

---

### ~~9. Clarify stub files~~

`vipaq/README.md`, `packages/README.md`, `docs/README.md`, `gems/README.md`, `web/README.md`
all contain only a gap marker. They add noise for agents doing core API work.

Updated frontmatter descriptions on all 5 gap files to warn of incomplete content.
`processors.md` merged into `algorithm-processor.md` and `result-selection.md`, then deleted.

---

## Done When

- No file has a real frontmatter description but empty content
- No doc links to a `.plans/` file
- The root README has a task-based index
- `IBinacleService` has its own doc
- Presets are documented
- Algorithm version ambiguity is resolved
