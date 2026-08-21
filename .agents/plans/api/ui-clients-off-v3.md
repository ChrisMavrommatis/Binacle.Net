---
description: Migrate the shipped UI clients off the v3 API
paths:
  - "api/**"
  - "packages/binacle-net-ui/**"
---

# Migrate the shipped UI clients off the v3 API

**Status:** Not started. v3 stays and is frozen, so the demo keeps working indefinitely - this is about not
shipping our own UI on the version we tell users is the older one.

## What

The shipped packing demo calls `POST /api/v3/pack/by-custom`, a compare-shaped call. **It is one line, in one
place**: `packages/binacle-net-ui/src/core/packingDemo.ts`. The UIModule rebuild deleted the second call site -
the module now serves the same TypeScript component the demo site does, so both hosts move on that one edit.

## Before assuming it needs `pack/compare-bins`

Check what the component does with the response. If it only shows the winning bin, `pack/smallest-bin` already
covers it and exists today - that is a smaller request and a smaller response than fetching every bin's result
and throwing most of it away.

## Watch out

v4 is experimental for the whole 3.0.x line and may change. Migrating our own UI to it is fine - it is the
adoption that justifies calling v4 stable later - but expect to touch that call site again.

## Done when

The packing demo calls v4, using the endpoint shaped like the answer it actually renders. Both hosts - the demo
site and the UI module in the image - get it from the same edit, but each needs its bundle rebuilt.
