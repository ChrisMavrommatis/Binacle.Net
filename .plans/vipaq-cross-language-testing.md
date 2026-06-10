# ViPaq — Cross-Language Wire Testing

**Status:** Not started — to build (you said you'll do this; this note captures the need + findings).
**Goal:** Guarantee the C# `Binacle.ViPaq` library and its hand-maintained TypeScript mirror
(`vipaq/binacle-vipaq`) stay **wire-compatible** — bytes written by one are readable by the other.

Reference docs: `/.agent-docs/vipaq/README.md` (canonical format), `/.agent-docs/vipaq/typescript.md` (TS mirror).

## The need

ViPaq has two independent implementations of the same binary format:
- **C#** (`vipaq/src/Binacle.ViPaq`) — the canonical implementation.
- **TypeScript** (`vipaq/binacle-vipaq`) — a by-hand reimplementation, no codegen, no shared schema.

Today **nothing automatically checks they agree**. The TS doc literally says wire compatibility is
"verified by hand when changing either side." That's fragile — a one-line change on either side can silently
break interop and no test catches it.

The fix is **shared reference test data** (same idea as `Binacle.TestsKernel`'s shared scenarios): a set of
inputs paired with the canonical C# output, loaded by *both* test suites so each is graded against the same
answer key. Because the file is plain JSON at a shared path (e.g. `vipaq/test-vectors/`), C# copies it to test
output and TS imports it — same data, two consumers.

A small **generator tool** (e.g. `vipaq/tools/…`) is the clean way to produce it: define the cases in C#,
run it, and it serializes each with the canonical C# and writes the JSON. Editing a case = editing C#, not
hand-computing bytes.

## ⚠️ The gzip requirement — read this first

The real requirement: **a payload serialized by one side must deserialize on the other** (C#→TS and TS→C#),
*including* compressed payloads.

**Why interop is structurally easy here:** the first byte is the `EncodingInfo` header, and the `Version` field
(bits 7-6, `Uncompressed`/`CompressedGzip`) lives in it. It is prepended **after** compression on both sides and
is **never inside the gzip stream**; on decode both sides read byte 0 first, then decompress the rest only if
`Version == CompressedGzip`. So **every blob is self-describing** — interop reduces to "emit a valid gzip member /
accept a valid gzip member," which is standard gzip on both sides.

Key nuance that shapes the test design:

- **gzip output bytes are NOT identical** across C#'s `GZipStream` and the JS/Node `CompressionStream('gzip')`.
  Same input, same algorithm, **different bytes** (different headers/OS flag/deflate choices). Both are valid gzip.
- So for compressed payloads you **cannot assert byte-equality** between the two implementations. That only
  works for **uncompressed** payloads (those *are* byte-identical).
- But byte-difference does **not** mean interop fails: gzip is a standard, so each side's `Decompress` reads the
  other side's `Compress` output fine. **Interop is achievable; byte-equality is not.**

Therefore the test strategy must be split:
- **Uncompressed vectors** → assert exact bytes (`serialize == golden`, both directions). Strongest check.
- **Compressed payloads** → assert **cross-decode**, not bytes:
  - bytes produced by **C# serialize** → **TS deserialize** → equals the original input, and
  - bytes produced by **TS serialize** → **C# deserialize** → equals the original input.
  - This needs either a harness that runs both runtimes, or pre-captured fixtures: a C#-compressed blob checked
    into the repo that the TS test decodes (and a TS-compressed blob the C# test decodes).

This cross-decode test for compressed data is exactly the "one serializes, the other deserializes" capability
you asked for — make it a first-class case, not an afterthought.

### Compressed cross-decode matrix (the concrete test)

The idea: **compress the same input twice — once in each language — and prove two things at once:**
1. the two compressed outputs are **different bytes** (different deflate engines — assert this, don't hide it), and
2. **either output decodes on either side back to the same original input** (the artifact you started from).

So you produce two artifacts that encode the **same input**, one compressed by each language — full ViPaq blobs
(`[header][compressed body]`, not raw gzip) — and run the 2×2 decode matrix:

|                                | decode in **C#**       | decode in **TS**       |
|--------------------------------|------------------------|------------------------|
| artifact compressed **by C#**  | own round-trip         | C# → TS interop        |
| artifact compressed **by TS**  | TS → C# interop        | own round-trip         |

- All four cells must recover the **identical original input**.
- Add the divergence assertion: `artifact-cs != artifact-ts` (documents that the encoders differ on the wire).
- Never use byte-equality *between* the artifacts as a pass condition — only the decode-to-input is the contract.
- Pick an input **comfortably over** the compression threshold so both sides definitely compress (e.g. ~60 small
  items), avoiding the body-255 borderline.

**Generate once, commit, consume read-only** (the TestsKernel pattern):

- The artifacts are generated **one time** and **checked in**. The tests are pure consumers — they read the
  committed artifacts and run the matrix; they do **not** serialize/compress at run time. So each suite runs with
  only its own runtime; no cross-runtime orchestration is needed in CI.
- A **C# data-generation step** serializes the shared input → writes `artifact-cs`. A **TS data-generation step**
  does the same → `artifact-ts`. Both committed.
- The cross-decode tests on each side read **both** artifacts (plus the shared input definition) and run the
  matrix above.
- Suggested home: alongside the golden vectors, e.g. `vipaq/test-vectors/compressed/` holding the shared input
  definition + `artifact-cs` + `artifact-ts`. The input definition is the single source of truth both generators
  serialize from.

### Regeneration workflow (solved)

**Key constraint that drives the design:** compressed artifacts must **never** be byte-compared across
regenerations. Deflate output depends on the zlib/engine version (C# bundles its own zlib, which changes between
.NET versions; Node/browser differ again), so regenerating on a different machine yields *different valid bytes*.
The only stable contract is **decode-to-input** — which is exactly what the matrix asserts. So regeneration is
"produce a fresh valid blob per scenario when you choose to," not "keep bytes in sync."

1. **Shared input = `scenarios.json`** (single source of truth) at `vipaq/test-vectors/compressed/`. One or two
   large inputs (over the compression threshold). Add/change a scenario by editing this one file.
2. **One generator per runtime, both reading `scenarios.json`:**
   - C#: the vector generator also reads it → writes `artifact-cs/<name>.b64`.
   - TS: a small `node`/`tsx` script (or an env-gated jest generation test) → writes `artifact-ts/<name>.b64`.
3. **One orchestration command** (e.g. npm `regen:vipaq`) runs the C# generator then the TS one, off the same
   input. Regenerating = edit `scenarios.json` → run one command → commit. A single command running both producers
   means `artifact-cs` and `artifact-ts` can't drift apart.
4. **Tests consume read-only** — read both artifacts + the input, run the 2×2 matrix, assert
   `artifact-cs != artifact-ts`. No generation at test time, so each suite needs only its own runtime in CI.
5. **CI guards — the right kind:**
   - Do **not** regenerate-and-byte-diff compressed artifacts (zlib-version nondeterminism → false failures).
   - **Do** add a cheap **coverage check**: every scenario in `scenarios.json` has a matching `artifact-cs` *and*
     `artifact-ts` file (existence, not bytes). Catches "added a scenario but forgot to run regen."
   - A byte-level drift guard is fine **only** for the deterministic uncompressed `vectors.json` (optional).

This is the same shape as regenerating the uncompressed golden vectors, just with a producer in each language —
and the zlib-nondeterminism point is why the compressed side relies on decode + presence checks, not byte diffs.

## Bugs found while prototyping (fix these as part of the build)

All confirmed by hand against the source. None are fixed in the tree right now (the prototype was reverted).

1. **TS `getByteSize` under-allocates for ≥32-bit** — `src/utils/getByteSize.ts` returns `ThirtyTwo → 3` and
   `SixtyFour → 4`; must be **4** and **8**. Feeds `getBufferSize`, so ≥32-bit serialize output is corrupt.
   (≤16-bit data is unaffected, which is why it's gone unnoticed.)

2. **TS `getCoordinatesBitSize` validates the wrong field + rejects 0** — `src/utils/getCoordinatesBitSize.ts`
   checks `item.width` instead of `item.z` (line 11), and uses `<= 0` so it **rejects coordinate `0`**. Canonical
   C# (`BitSizeHelper.GetCoordinatesBitSize`) throws only for `< 0` — coordinate `0` is valid and common (any
   item flush against the bin origin). Fix: check `x`/`y`/`z` with `< 0`. Without this, the TS mirror can't
   serialize most real packing results.

3. **C# `ProtocolReader<T>.ReadAsByte()` only works for `T == int`** — `vipaq/src/Binacle.ViPaq/ProtocolReader.cs`
   line 29 does `return (T)(object)this.InternalReadByte();`. `InternalReadByte()` returns `int`, so the boxing
   cast unboxes only when `T` is `int`; for `long`/`ushort`/`ulong` it throws `InvalidCastException`. Its siblings
   `ReadAsUInt16/32/64` correctly use `T.CreateChecked(...)`. Fix: `return T.CreateChecked(this.ReadByte());`.
   Effect: any 8-bit-width section fails to deserialize for non-`int` `T`.

4. **C# `BitSizeHelper` overflows for narrow `T` → `SerializeInt32` is unusable above 65535** —
   `GetDimensionsBitSize`/`GetCoordinatesBitSize` build the comparison constants with
   `T.CreateChecked(uint.MaxValue)` and `T.CreateChecked(ulong.MaxValue)`. For `T = int` the first overflows, so
   `SerializeInt32` throws `OverflowException` for **any value > 65535**. The `ThirtyTwo` regime needs `T = long`;
   `SixtyFour` needs `T = ulong`. The agent doc currently calls `Int32` "the safe default" — it isn't.

   **Combined effect of #3 + #4:** a payload that mixes an 8-bit section with a ≥32-bit section is currently
   **un-round-trippable in C#** — serialize needs a wide `T` (#4), but a wide `T` breaks the 8-bit read (#3).

5. **C# vs TS compression threshold off-by-one** — C# compresses when the **body** is `> 255`; TS compresses
   when the body **plus the 3 header/count bytes** is `> 255` (`ViPaqSerializer.serialize.ts` uses `bufferSize`,
   which includes the header). They disagree at a body length of exactly 255. **NOT an interop bug** — the header
   is self-describing, so at the borderline one side may emit `[Uncompressed][raw]` and the other
   `[CompressedGzip][gzip]`, and each still decodes correctly on the opposite side. The only effect is the two
   produce *different blobs* for that one borderline input — a byte-equality difference, which we don't assert.
   Worth aligning for tidiness, but not a blocker.

6. **Pre-existing TS test casing bug** — `vipaq/binacle-vipaq/tests/utils/encodingUtils.test.ts` imports
   `../../src/models/EncodingInfo` (capital E) while the file is `encodingInfo.ts`; trips
   `forceConsistentCasingInFileNames` and fails `npm test`. One-char fix.

## Suggested build order

1. Fix the bugs above (1–4 are needed before any ≥16-bit or mixed-width vector can pass; 5 affects the
   compression boundary; 6 unblocks `npm test`).
2. Add the generator tool + an **uncompressed** golden-vector set (exact-byte parity, both directions).
   Cover every BitSize, mixed widths (large bin / small items, small dims / large coords), the 255↔256 and
   65535↔65536 boundaries, and coordinate `0`.
3. Add the **compressed cross-decode** vectors/fixtures (the gzip-interop requirement above).
4. Add per-language gap tests the shared file can't cover: `ProtocolReader`/`ProtocolWriter` little-endian,
   empty item list, the documented throws.
