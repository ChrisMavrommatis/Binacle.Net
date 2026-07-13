---
description: What the API and UIModule migration left behind — stale OpenAPI examples, saved browser tokens, and the v3 payload break.
---

# API migration — what is left

`Binacle.Net` and `Binacle.Net.UIModule` are **green** (2026-07-10). Their surveys are deleted. Three things were
found while migrating them and deliberately not fixed. None of them stops a build.

## 1. The OpenAPI `ViPaqData` examples are wrong — and were already wrong

**14 hardcoded base64 strings across 7 files.** Every one predates the rebuild and cannot be decoded by the new
wire, so every one is now a lie in the published OpenAPI document.

```
v3/Contracts/PackByCustomRequest.cs              2
v3/Contracts/PackByPresetRequest.cs              2
v4/Contracts/Fit/FitCustomBinRequest.cs          2
v4/Contracts/Fit/FitPresetBinRequest.cs          2
v4/Contracts/Pack/PackCustomBinRequest.cs        2
v4/Contracts/Pack/PackCustomSmallestBinRequest.cs 2
v4/Contracts/Pack/PackPresetBinRequest.cs        2
```

**They were never right.** Only **two distinct tokens** exist among the 14 — one string is copy-pasted into 12
example bodies, across v3 and v4, across fit and pack, across preset and custom. `PackCustomBinRequest.cs` uses
the same token for its 4-item "fully packed" example and its 2-item "partially packed" example. A token encodes
the bin *and* the items, so at most one of those two could ever have matched. This is independent of the rebuild.

Regenerating them means, for each example, building the `Bin` and item list the surrounding example already
declares, and running it through `ViPaqSerializer.Serialize(...).ToBase64()`. The examples become derived from
their own data instead of guessed.

**Do not hand-write them.** A scratch generator that reads each example's declared geometry is the only way this
stays true the next time the wire changes. Better still, make the examples call the serializer at startup so they
cannot drift again — but that puts encode cost in the OpenAPI path, so measure before choosing.

## 2. Saved tokens in the browser are undecodable

`ProtocolDecoder.razor.cs` keeps user tokens in `localStorage` under `ProtocolDecoderSavedResults` (read at line
33, written at 60 and 96). Every saved token is from the old wire. Nothing was changed about this.

The first byte cannot distinguish "old format" from "corrupt" — the old header was one byte, the new one is two,
and the new `Version` field reads `0` on plenty of old bytes. So there is no detection to write. The honest fix is
to clear the key once, on a version marker, and tell the user why their saved results are gone.

This is a maintainer decision, not a code question.

## 3. The v3 payload break

v3's JSON contract is intact — `ViPaqData` is still a base64 string under the same conditions. But every token it
emits is unreadable by any old client, and every stored token is unreadable by the new API.

`decisions.md` D11 already settled this (breaking rebuild, no compatibility, no migration) and
`.agents/pending-actions.md` already tracks announcing it. What is still wanted is the maintainer confirming that
a hard payload break inside a frozen contract is acceptable.

## What the migration actually did

For the record, since both surveys are gone:

- New file `vipaq/src/Binacle.ViPaq/ViPaqBase64Extensions.cs` — `byte[].ToBase64()` and `string.FromBase64()`,
  in the `Binacle.ViPaq` namespace. Purely additive.
- Three call sites now chain: `ViPaqSerializer.Serialize<Bin, PackedBox, int>(bin, items).ToBase64()`, and
  `ViPaqSerializer.Deserialize<Bin, PackedItem, int>(token.FromBase64())` in the decoder page.
- **The typed wrappers were not needed.** `SerializeInt32` / `DeserializeInt32` never came back; the call sites
  name the generic arguments directly. Nothing else asks for them, so they stay deleted.
- The surveys predicted `CS0122` (inaccessible). The real errors were `CS0117` — the methods had simply been
  renamed. The serializer was already public.
