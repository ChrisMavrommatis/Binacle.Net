# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Writing Style

Use plain, simple language in all docs, comments, and explanations. Short is better.
If you can say it in fewer words without losing meaning, do.
Avoid technical terms unless there is no simpler way to say it.
Write like you are explaining to a developer at 3am.

Keep each line to around 120 characters.
When nearing that, break at a full stop and continue on the next line.
If a sentence can't be broken by 120 chars, you can run to 150-166 before you must break.
At that point, rephrase to add a full stop, or just break the line.


## Agent Documentation

Detailed documentation for agents is in `.agents/docs/`:

@.agents/docs/README.md
@.agents/docs/_index.md

## Critical Rules

- **Do not modify v3.** All new endpoints go in v4 only.
- **Always use `BindingResult<T>`** in endpoint handlers — never bind the request body directly.
- **Add `.RequireRateLimiting("ApiUsage")`** to v4 endpoints that handle user requests (fit, pack, presets).
  Safe to include unconditionally — no-op when ServiceModule is off.
- **Add `.RequireCors(CorsPolicy.CoreApi)`** where CORS protection is needed. Check existing endpoints
  in the same group for the expected pattern before deciding.
- **Never add `.ProducesProblem(500)` per endpoint** — `ApiV4EndpointGroup` sets it for all v4 endpoints.
- **`Algorithm` is required** — the `NotNull()` validator rejects null. Never treat a missing algorithm as valid.
- **Never construct `OperationResult` directly** — only `OperationResultBuilder` can create one.
- **`Presets.json` is required** — the app fails to start without it.
- **If adding a new module**, create its own `IModuleMarker` in that module's assembly.
- **When you edit a doc in `.agents/docs/`**, update its `verified:` frontmatter date to today.
  Also check its `also_update:` field — those docs may also be stale.
- **When verifying a doc in `.agents/docs/`**, read its `check:` field to know exactly what to confirm.
