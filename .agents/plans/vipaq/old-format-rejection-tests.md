# Add old-format ViPaq token rejection tests

**Status:** Not started — ready to implement. The tokens and the mechanism are already verified (below); this
just commits them as regression vectors.

## Why
v3.0.0 tells users old ViPaq tokens "no longer decode." That was checked against the real `binacle/binacle-net:2.1.1`
image: 250 random old tokens plus targeted adversarial ones, all rejected with `ViPaqFormatException`, **zero
silent misparses**. Commit that as a guard so a future decoder change can't quietly start accepting (misparsing)
an old token. It backs the release claim and the breaking-rebuild decision (`$vipaq#D11`).

## The harness already exists — this is mostly new vectors
`vipaq/test/Binacle.ViPaq.UnitTests/Tests/Serialization/DecodeInvalidTests.cs` asserts that every row in
`vipaq/test-vectors/serialization/decode-invalid.json` throws, resolved by `Name`. Adding rows is enough — no
new test code. Each row is `{ Name, Blob, Reason }`, where **`Blob` is an array of per-byte strings** (`"0x00"`,
or binary like `"0b10_00_00_00"`), *not* base64. Convert the base64 tokens below to per-byte hex arrays.

## Curated set — one real v2.1.1 token per rejection stage
Small and representative (not the 250). Each is genuine v2.1.1 output; the category follows from input size.

| Name (suggest `old-v2-*`) | base64 token | first byte | rejected by |
|---|---|---|---|
| `old-v2 uncompressed 8-bit tiny` | `AAEACgoKBQUFAAAA` | `0x00` | header reserved-bit check (byte 1 = old count low byte) |
| `old-v2 8-bit, count byte looks like a valid header` | `AAQAKCgoCgoKAAAACgoKCgAACgoKAAoACgoKAAAK` | `0x00` | **body-length integrity** — passes the header check, then "declaring 10240 items…" |
| `old-v2 uncompressed 16-bit` | `EAgALAEsASwBZGRkAAAAZGRkZAAAZGRkAGQAZGRkAABkZGRkyAAAZGRkZGQAZGRkZABkZGRkAMgA` | `0x10` | reserved width-code check |
| `old-v2 gzip-compressed` | `QB+LCAAAAAAAAAM9jSGSRUEIA2PaYDBjxuxJ5v7n+kXzag1QIem8vPeAJLhm4FSoXSrfqPWsvYDWU2o+/9eaxpM2NSdBJQ0cs21WC2VWI7WB77G4RUw2R+acpGSingNcyUeyRlqydlpySTZKSa7FfYEt3OKtGXKuvXOStrfsxW8u8Gf7tV07p/ID3Nc1NG0BAAA=` | `0x40` | unsupported version (old packed the compress flag into the version field; `1f 8b` gzip magic follows) |

The second row is the one that matters most: its second byte forms a *valid* new header, so it reaches body
parsing — and the body-length check still catches it. Keep that case.

## Cross-language — do not skip this
`decode-invalid.json` is replayed by **both** suites — C# (`DecodeInvalidTests`) and TS
(`vipaq/packages/binacle-vipaq/tests/providers/DecodeInvalid.ts`), see `$vipaq/cross-language-testing`. So these
rows also assert the **TS** decoder rejects old tokens. Only C# was verified in the spike. **Run the TS suite
after adding them** — if TS misparses one instead of throwing, that is a real decoder gap to fix, not a vector
to weaken.

## Decisions
- **Extend `decode-invalid.json` vs a new `old-format.json`.** Lean extend — the existing test picks it up free;
  use an `old-v2-*` name prefix and a `Reason` that records the v2.1.1 provenance. A dedicated file only if the
  set grows.
- Optionally pin the C#-specific stage per token in `SerializationBehaviorTests` (reserved-bit / width-code /
  version / body-length), so a regression that moves the rejection to a different stage is visible. Nice-to-have.

## How to regenerate the tokens
`docker run --rm -p 8781:8080 binacle/binacle-net:2.1.1`, then
`POST /api/v3/pack/by-custom` with `includeViPaqData: true`. Category by input: small dims → uncompressed 8-bit;
coords > 255 → 16-bit; many items → gzip. Header bytes and body layout are in `vipaq/PROTOCOL.md` §2/§6/§7.
