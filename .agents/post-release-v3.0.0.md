# Post-release — right after Binacle.Net v3.0.0

**Status:** Do these once v3.0.0 is out. None gate the release; all are release-adjacent. Like the release plan,
this coordinates other files and nothing points back at it. Delete it once the list is clear.

## Docs site catches up
The image ships first; the docs site follows. The work is owned by `plans/docs-versioning.md` — the `v3.0.x`
folder is still a stub:
- Write the `v3.0.x` pages: `api/` (v3 + v4), `swagger/`, `configuration/`, `samples/`, `quick-start.md`,
  `release-notes.md`. API v2 must not reappear — it lives on in `v2.1.x` / `v2.0.x`.
- Generate `swagger/v4.json` on the **`Normal` profile (ServiceModule OFF)** and mark v4 **experimental**.

## Fix the shared ViPaq protocol page
`docs/collections/_common_pages/vipaq-protocol.md` describes the OLD format (gzip) for **every** version of the
site; v3.0.0 is raw DEFLATE (`vipaq/PROTOCOL.md` §6). Do this **before publicising** if it has not already been
done as a release action — either move the page into the version folders, or make the one shared page
version-aware.

## Correct the v4 spec's algorithm claim
The v4 endpoint descriptions promise "all algorithms (FFD, WFD, BFD)". `Best` only races that full set on the
single-bin routes (`fit/bin`, `pack/bin`); everywhere else it races FFD + BFD. Low severity — v4 is
experimental — but fix it alongside the v4 docs so the published spec is honest. The behaviour is deliberate and
settled in `$lib/decisions#D1`; the measurements are in `$lib/findings`.

## Migrate the UI clients off v3
Both shipped clients call `POST /api/v3/pack/by-custom`, a compare-shaped call:
- `packages/binacle-net-ui/src/core/packingDemo.ts:127` — the web site packing demo
- `api/src/Binacle.Net.UIModule/Components/Pages/PackingDemo.razor.cs:135` — the Blazor UI module
They keep working because v3 stays and is frozen, so this is not urgent. Before assuming a client needs
`pack/compare-bins`, check what it does with the response: if it only shows the winning bin, `pack/smallest-bin`
already covers it and exists today.
