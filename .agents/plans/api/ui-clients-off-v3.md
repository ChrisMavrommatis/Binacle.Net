---
description: Migrate the shipped UI clients off the v3 API
paths:
  - "api/**"
---

# Migrate the shipped UI clients off the v3 API

**Status:** Not started, not urgent. After v3.0.0. v3 stays and is frozen, so both clients keep working
indefinitely - this is about not shipping our own UI on the version we tell users is the older one.

## What

Both shipped clients call `POST /api/v3/pack/by-custom`, a compare-shaped call:

- `packages/binacle-net-ui/src/core/packingDemo.ts:127` - the web site packing demo
- `api/src/Binacle.Net.UIModule/Components/Pages/PackingDemo.razor.cs:135` - the Blazor UI module

## Before assuming they need `pack/compare-bins`

Check what each one does with the response. If it only shows the winning bin, `pack/smallest-bin` already covers
it and exists today - that is a smaller request and a smaller response than fetching every bin's result and
throwing most of it away.

## Watch out

v4 is experimental for the whole 3.0.x line and may change. Migrating our own UI to it is fine - it is the
adoption that justifies calling v4 stable later - but expect to touch these two call sites again.

## Done when

Both clients call v4, and each uses the endpoint shaped like the answer it actually renders.
