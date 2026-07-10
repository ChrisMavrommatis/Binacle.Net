---
description: Persistent ViPaq session prompt. Read first, work, then update it before you finish.
---

# ViPaq — session prompt

Read this after `CLAUDE.md` and `.agents/README.md`, not instead of them. Those two carry the repo-wide rules —
where guidance lives, how to write, and the one hard guardrail (never commit). This file only adds what is
specific to ViPaq. **Update it before you finish** (last section).

## Mission

Rebuild ViPaq to the new wire format: C# first, then the TypeScript mirror, then regenerate the test vectors.

## What is true

**`vipaq/PROTOCOL.md` is the format.** It is the only authority on what the bytes mean. It is language-neutral on
purpose — anyone can implement it. If the code disagrees with it, the code is wrong.

**These four files are true and binding.** They sit next to this one:

- [architecture.md](architecture.md) — the policy/mechanism split. The rebuild keeps it.
- [decisions.md](decisions.md) — D1–D14, settled with reasons. Do not re-open one without new measurements.
- [findings.md](findings.md) — real numbers. It measured the old implementation, so read it for magnitudes, not facts.
- [testskernel-restructure.md](testskernel-restructure.md) — test-kernel work still outstanding.

**`reference/` is false by default.** Five session logs written while the format was still being argued about,
against a wire that no longer exists. Open one only if you are stuck. Do not maintain it. Do not cite it.

**`.agents/docs/vipaq/*` is right about the shape, wrong about the format.** It describes the shipped library. Its
wire layout is stale. Its account of the **public surface and file layout is current, and you must keep to it** —
see the next section. Rewrite those docs when the new wire lands.

## What must not change

The format is being rebuilt. The project's shape is not. Breaking one of these is a bug, not a design choice.

- **The public surface ends up back at `ViPaqSerializer`** — `Serialize` / `Deserialize` plus the typed wrappers
  (`SerializeInt32`, `DeserializeInt32`, `SerializeUInt16`, `DeserializeUInt16`), `Bin<T>` / `Item<T>`, and
  `ViPaqLimits`. Callers in `api/` and the benchmark harness depend on it (`decisions.md` D4). It is rebuilt in
  phase 2. Phase 1 is allowed to remove it, but only with the sequencing question in `architecture.md` answered
  first — otherwise you break `v3/Contracts/PackResponse.cs`, `v4/Contracts/BinResponseBase.cs`, the UIModule
  decoder, and every unit test, exactly as the last attempt did.
- **The chooser is public; the layer that obeys a supplied header is `internal`.** That is `architecture.md`.
  `Serialize` picks widths, layout and compression, then calls the blind layer. Do not ship only the blind layer.
- **File layout is `Models/`, `Helpers/`, `Layouts/` at the project root.** A new folder needs a reason that
  survives being said out loud. `ExtensionMethods/` folds into the reader and writer — see `architecture.md`.
- **No new abstraction for a local problem.** No interface, no factory, no project. Copy instead. Share only when a
  third consumer actually appears. The layout codecs (`ILayoutCodec`, two implementations, one factory) are the
  one sanctioned exception, because `architecture.md` calls for them. Everything else needs asking first.

## How to work

**Say your plan in the response, then build.** Do not create a plan document — that is what filled `reference/`.
Two or three sentences before you touch code is not a plan document. It is thinking, and it is required.

**When the code forces a decision, make it there,** with the code in front of you. If the decision changes the
wire, write it into `PROTOCOL.md` **in the same change**. A decision that lives only in a code comment is lost.

**Nothing ships without a measured gain** — smaller base64, faster, less memory, or a concrete simplicity win.
Default to no.

**Do only what you set out to do.** If a fix pulls you into a restructure, stop and write it down instead.

**Never commit, stage, or push.** The human commits. Leave changes in the working tree.

## Done means

Not "the code is written." All four, every time:

1. `dotnet build` passes on `vipaq/src/Binacle.ViPaq`. **Until phase 2 lands, five downstream projects are
   knowingly red** (see "Where you are now") — that is the only accepted red. Do not add a sixth, and do not
   leave the library itself broken.
2. The ViPaq tests pass — `./config/tests.sh vipaq`. They cannot until phase 2. When they can, they must.
3. Anything you settled that changes the wire is in `PROTOCOL.md`, not only in a comment.
4. Nothing you added is unreachable. If no code calls it, delete it.

## Where you are now

**Last session (2026-07-10): phase 1 landed, and the old format is gone.** In `vipaq/src/Binacle.ViPaq/`:
`Header` / `Version` / `Width` / `Layout`, the `Layouts/` codecs, `Compression/` with `ICompressionCodec` +
`DeflateCodec` + `GzipCodec`, the header/width/validation helpers, and two layers:

- **`ProtocolEncoder`** — `Encode` and `Decode` on one class, because they are one agreement read in two
  directions. Handed a header, it obeys it: widths, layout, and whether to compress. **The codec is a constructor
  argument**, so a `NoOpCodec` makes the compressed path testable with the body still readable.
- **`ViPaqSerializer`** — the chooser. **A stub: `Serialize` and `Deserialize` throw.** It must work the header
  out (narrowest widths per section, a layout, and compress-both-keep-shorter per D7). `Deserialize` splits off
  only the two header bytes and hands the rest down — the item count lives *inside* the compressed body, so only
  the encoder can read it. A working version was written and pulled back out; the shape is not settled.
- **`HeaderNotation`** — also a stub. The header's text form for the test vectors; the grammar is undecided.

`ProtocolWriter<T>` / `ProtocolReader<T>` are down to `WriteValue` / `ReadValue` (which dispatch to
`Write8Bits` / `Write16Bits`) and the uint16 item count. They move one value at one width; the callers group
them into triples, because the order of an item's three dimensions *is* the layout.

Verified against `PROTOCOL.md` §10: all three worked examples byte-for-byte, the forced
`(codec × compressed × layout × width)` matrix round-tripping to input, empty-items, and every §8 rejection.

**`ViPaqSerializer` and the whole old format are deleted** — no shim. `Binacle.ViPaq` builds clean. Five
projects are knowingly red until phase 2: `Binacle.ViPaq.UnitTests`, `.TestsKernel`, `.VectorGenerators`,
`.PackedDataGenerator`, and `Binacle.Net.UIModule`. This is deliberate (D11: breaking rebuild, no migration).

**Next: race the two codecs.** `PROTOCOL.md` §6 still names no codec, which is why both exist. Measure raw
DEFLATE against gzip on real packs — stored base64 size first, then encode time. Then name the winner in §6,
strike it from §12, record the numbers in `findings.md`, lock it in `decisions.md`, and collapse
`ICompressionCodec` to the one implementation.

**Then phase 2 — the two stubs, then the five red projects.** Write `ViPaqSerializer` (the chooser) and decide
the `HeaderNotation` grammar. Pin the codec and make the serializer public. Then swap the `api/` and UIModule
callers over, and rewrite the tests, generators and test kernel against the new wire.

Phase 1 has **no tests of its own** — it was verified with a scratch console app, since the unit-test project no
longer compiles. Write them properly once phase 2 makes that project build again.

**The first decision the code will force:** `PROTOCOL.md` §6 does not name a compression codec, and `Version` pins
it. The spec is not final until you pick one, and the TypeScript mirror cannot start without it. Pick it, write it
into §6, remove it from §12 — in the same change. `decisions.md` O2 holds the constraints: the codec must exist in
C# and in the browser; gzip-Optimal and brotli-Optimal tie on size at usable speed; never brotli q11 as a default.

Open work, verified and not in the spec, is listed in [README.md](README.md).

## Before you finish — update this file

This prompt is the handover. A session that leaves it stale has failed the next one.

- Rewrite **"Where you are now"**: what you did, what is next, what decision is now blocking.
- If you settled something that changes the wire, it belongs in `PROTOCOL.md`, not here.
- If a `reference/` file finally proved useless, delete it. That folder should shrink to nothing.
- Keep this file short. It is a prompt, not a log. Nothing here should need to be read twice.
