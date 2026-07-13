---
description: The report the codec race must produce — modes, tables and columns — so PROTOCOL.md §6 can name a codec.
---

# The codec race — what the harness must output

`PROTOCOL.md` §6 names no compression codec, and `Version` pins it, so the spec is not final until one is chosen.
This file specifies **the report that makes the choice obvious**. Build to it; do not invent columns.

Layout (row-major vs columnar) is the other unmeasured choice, and it is measured in the same pass, because a
layout only pays through a codec.

## Results so far (2026-07-11) — read, not locked

The report is built and has run once. Raw numbers live in `results/vipaq/` (size) and the
`Curated{Encode,Decode}Benchmarks` artifacts (time). Those files are overwritten on every run, so the read is
recorded here:

- **Codec — deflate.** Gzip is bigger on every real pack, both layouts; its fixed trailer is a floor small packs
  never clear. Deflate never inflates a real pack. Time cannot rescue gzip — it is deflate plus a header and CRC.
- **Layout — columnar.** It wins on size once deflate runs (8-item 36 vs 48 b64, 100-item 132 vs 268, Bischoff
  ~10–15% smaller), widening with item count. This **contradicts `findings.md`'s bet** that a good codec already
  exploits the structure. It costs ~10% encode and ~3–5% decode time, and decode allocation is identical to row.
- **ViPaq still beats protobuf** under matched codecs — ratios 0.62–0.88 on deflate, down to 0.50 uncompressed.
  The old fairness bug (compressed ViPaq vs raw protobuf) is gone and the win holds.
- **Crossover ~2 items.** 0–1 item packs and a few 2-item ones are smaller raw; that is what D7's try-both is for.
- **Format cost in time:** ViPaq encodes ~1.9× (row) / ~2.2× (col) slower than protobuf, decode roughly on par,
  both allocate less. Sub-µs to a few µs, encoded once server-side — negligible.

**One cell is still empty: compression time.** The benchmarks ran the NoOp path only, so deflate's own
encode/decode cost and D7's try-both price are unmeasured. Run that before the final write.

**Not locked on purpose** — more scenarios and codecs are coming, so re-run before pinning.

## What the numbers must answer

1. **Does compression pay at all**, against what the library ships today (uncompressed)?
2. **Deflate or gzip**, on stored base64 size, then on time?
3. **Does columnar buy anything** once a codec runs?
4. **Is ViPaq still smaller than protobuf** when protobuf gets the same codec?

Question 4 is the one the current report gets wrong. It compares a compressed ViPaq token against raw protobuf,
so the reported gap is mostly the compression, not the format. **Every ViPaq codec must be mirrored on protobuf**,
with the same codec and the same settings, so the only thing that differs is the format.

## The modes

A mode is a header (`Layout` + `Compressed`) plus a codec. There are four codecs but only three sizes.

One `Mode` enum in `TestsKernel`, used by every table below. It is the only new public type the harness needs.

| Mode | Layout | `Compressed` | ViPaq codec | Protobuf pairs with | In size tables | In time table |
|---|---|---|---|---|---|---|
| `Raw` | Row | 0 | — | raw protobuf | yes | yes |
| `NoOp` | Row | 1 | `NoOpCodec` | — | **no** | yes |
| `RowDeflate` | Row | 1 | `DeflateCodec` | deflated protobuf | yes | yes |
| `RowGzip` | Row | 1 | `GzipCodec` | gzipped protobuf | yes | yes |
| `ColumnarDeflate` | Columnar | 1 | `DeflateCodec` | deflated protobuf | yes | yes |
| `ColumnarGzip` | Columnar | 1 | `GzipCodec` | gzipped protobuf | yes | yes |

**Two things do not vary size, and the tables must not pretend they do.**

- **Raw size is the same in both layouts.** The body is `3 × binWidth + n × 3 × (dimWidth + coordWidth)` bytes.
  Layout decides the *order* of those values, never the count. Base64 length follows byte count alone. So an
  uncompressed columnar token and an uncompressed row-major token are byte-for-byte the same length, always.
  There is no `Col+Raw` row because it would duplicate `Raw`.
- **`NoOpCodec` does not change size either.** It passes the body through, so a no-op token is a raw token with
  one header bit flipped. It exists to price the *compressed path* — the extra allocation and copying — with the
  squeezing removed. `Deflate` minus `NoOp` is what deflate's actual compression work costs. That is a time
  question, so `NoOp` appears only in the time table.

## Table 1 — One size table per mode: ViPaq against protobuf, like for like

`SizeComparisonTest` takes a **mode** alongside the scenario set it already takes, and `Program` registers one
per `(scenario set × mode)`. They all write into the one `SizeComparison` result file, as they do today.

That is **5 size modes** (`Raw`, `Row+Deflate`, `Row+Gzip`, `Col+Deflate`, `Col+Gzip`) across the two real
scenario sets — ten tables in one file. `NoOp` is not among them; it does not change size.

Each table is one row per scenario, and **protobuf is encoded with the same codec as that table's mode**:

| Scenario | Items | Widths b/i/c | ViPaq bytes | ViPaq b64 | Proto bytes | Proto b64 | ViPaq/Proto | Round-trip |
|---|---|---|---|---|---|---|---|---|

- `Widths b/i/c` — the three widths the encoder chose, read from the token's own header. Never the scenario tag.
- `ViPaq/Proto` — base64 against base64, as a percentage. Under 100% means ViPaq is smaller.
- **The `Raw` table pairs with raw protobuf. `*+Deflate` pairs with deflated protobuf. `*+Gzip` with gzipped
  protobuf.** One codec per table, both sides. That is the fairness rule, and it is what the old report broke.
- A row that does not round-trip is not a size win. Flag it `FAIL` and log an error, as today.
- No layout column: it is fixed per table, and it does not move raw size anyway.

## Table 2 — The summary: every mode on one line

Table 1 proves ViPaq beats protobuf *at each codec*. It cannot tell you **which codec to pin**, because that
means reading five tables side by side and matching scenario names by eye. This table does that job, and it is
the one the decision is actually made from. Its own test, its own file.

One row per scenario. Base64 lengths only — base64 is the stored form and the headline number, and the raw byte
counts are already in Table 1.

| Scenario | Items | Raw b64 | Row+Defl | Row+Gzip | Col+Defl | Col+Gzip | Proto+Defl | Proto+Gzip | Best |
|---|---|---|---|---|---|---|---|---|---|

- `Raw b64` is the column every compressed column is read against. A compressed column **larger** than it means
  compression inflated that scenario — expected on small packs, and exactly what D7's try-both exists to avoid.
- `Best` names the winning mode for that scenario, or `Raw` when nothing beat it. The modal value of this column,
  across real packs, is the answer to "which codec".
- Compare `Row+Defl` against `Col+Defl` for the layout question, holding the codec still.

## Table 3 — Encode and decode time

BenchmarkDotNet, in `Binacle.ViPaq.Benchmarks`. One row per mode, all six, plus `NoOp`.

| Mode | Encode mean | Encode alloc | Decode mean | Decode alloc |
|---|---|---|---|---|

`NoOp` is the reason this table has seven rows and Table 2 has five columns. Read it as:

- `NoOp − Raw` — what the compressed path costs before any squeezing.
- `Row+Deflate − NoOp` — what deflate's compression work actually costs.

**D7's real cost is one compression per call, not two.** Try-both means: build the body once, compress it once,
compare the two lengths, keep the shorter. So try-both costs exactly one `Compress` more than never compressing,
and nothing at all more than always compressing. The time table prices that single `Compress`. It does **not**
need a "try-both" mode of its own.

## What the harness needs, and what it must not take

`Binacle.ViPaq.TestsKernel` already has the `InternalsVisibleTo` grant (D4, amended 2026-07-10), and both
`PerformanceTests` and `Benchmarks` reference only `TestsKernel`. **So no new grant is needed.** All the
internal driving — `ProtocolEncoder`, `Header`, the three codecs — lives inside `TestsKernel`, and the two
harness projects keep calling public types.

To force a header the kernel must be able to build one. `ViPaqSerializer.CreateHeader` becomes `internal`
rather than `private`; the kernel calls it and flips `Compressed` and `Layout`. **Do not re-implement the
width-choosing rule in the kernel** — that is the duplicate-spec mistake `ViPaqHeader` was just rescued from.

Protobuf must be compressed through **the library's own codec types**, not a hand-rolled `GZipStream`, or the
comparison silently measures two different compressor configurations.

## Build order

1. **`ViPaqSerializer.CreateHeader` goes `private` → `internal`.** The one library change.
2. **`Mode` enum in `TestsKernel`**, exactly the six rows above.
3. **`ViPaqEncoder.Encode(scenario, mode)`** — calls `CreateHeader`, applies the mode's `Layout` and
   `Compressed`, and hands a `ProtocolEncoder` the mode's codec. `Decode` needs the same codec, so it takes the
   mode too. The existing no-mode overload becomes `Encode(scenario, Mode.Raw)`.
4. **`ProtobufEncoder.Encode(scenario, mode)`** — raw, deflated or gzipped, through the **library's** codec types.
   Delete `EncodeGzip`. A hand-rolled `GZipStream` here would silently compare two different compressor configs.
5. **`SizeComparisonTest` takes a `Mode`**, and `Program` registers one per `(set × mode)` — ten registrations.
   Put the mode in each table's title.
6. **`CodecComparisonTest`** — new, Table 2, its own `ResultFile`. Encodes each scenario in all five size modes.
7. **`Benchmarks`** — Table 3, seven rows including `NoOp`.

`CompressionCrossoverTest` and `CuratedScenarioCheck` both currently assert that **nothing compresses**, because
nothing does. They keep passing while the race runs — the race drives `ProtocolEncoder` directly and never
changes what `ViPaqSerializer` ships. Un-invert them only when the codec is pinned.

## D5 already says this

`decisions.md` D5 used to call the codec choice a one-off experiment to be kept out of the harness. **It was
reversed on 2026-07-10** and now says the opposite: the race is part of the permanent harness. Read it there.

**Nothing shrinks once the codec is pinned.** Every mode keeps its table, every table keeps its protobuf mirror,
and both codecs keep their implementations. "Does compression still pay on this pack, and is deflate still the
right pick?" is a question the harness should answer on every run, not once. Pinning the codec changes exactly
one thing: which codec `ViPaqSerializer` hands the encoder.

## Both codecs stay. Only one reaches the wire.

`ICompressionCodec`, `DeflateCodec` and `GzipCodec` are **permanent**, not scaffolding. The harness measures both
on every run, and protobuf is mirrored through both, so both must exist for as long as the tables do.

That is not the same as leaving the format ambiguous, and the difference matters:

- **The wire pins one codec.** `Version` fixes it (`PROTOCOL.md` §6) and there is no codec field in the header,
  so a shipped blob is inflatable by exactly one thing. `ViPaqSerializer` names that one, and only that one.
- **The library keeps two.** `ProtocolEncoder` takes a codec because that is what makes it testable and what
  lets the harness force a mode. It always has.

So `ICompressionCodec`'s comment ("this interface is temporary… it collapses to that single implementation") is
now wrong, and so is every plan file that says to collapse it. The interface stays. What changes after the race
is which codec `ViPaqSerializer` hands the encoder — one line.

`NoOpCodec` stays too, for the same reason: it prices the compressed path, and it lets a test force the
compressed framing with the body still readable (§6.1 forbids comparing real compressed bytes).

## After the race

In one change: name the winner in `PROTOCOL.md` §6, strike it from §12, record the numbers in `findings.md`,
lock the codec **and** the layout in `decisions.md`, split `ProtocolEncoder.Encode` into a body half and a
compress-and-frame half (D7 needs it), point `ViPaqSerializer` at the winning codec, and drop `Deserialize`'s
`NotSupportedException`.

Then un-invert the two `TestsKernel` checks that currently assert nothing compresses: `CuratedScenarioCheck`
splits back per list, and `CompressionCrossoverTest` finds a crossover again.
