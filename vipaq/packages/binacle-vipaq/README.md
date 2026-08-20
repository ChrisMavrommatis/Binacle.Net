# binacle-vipaq

The TypeScript implementation of the ViPaq format - encode a packing result to bytes, decode it back. It is
the **mirror** of the C# library in [`../../src/Binacle.ViPaq`](../../src/Binacle.ViPaq), not a port of it:
both are written against [`../../PROTOCOL.md`](../../PROTOCOL.md), which is the only normative document.

There is no codegen and no shared schema between the two. **The shared test vectors are what keep them
honest** - see [`../../test-vectors/README.md`](../../test-vectors/README.md).

## 📂 What is in it

| Path | What it is |
|---|---|
| `src/ViPaqSerializer*.ts` | The entry points - `serialize` and `deserialize`, split into a file each |
| `src/Protocol{Reader,Writer,Encoder}.ts` | The bit-level reader and writer the serializer sits on |
| `src/models/` | Header, bin, item, dimensions, coordinates, layout, version, width |
| `src/layouts/` | The two body layouts - row-major and columnar - behind a small factory |
| `src/compression/` | The codecs - none, deflate, gzip - and `resolveCodec` picking between them |
| `src/utils/` | Header build and byte conversion, width selection, body length, the format error type |
| `tests/` | The jest suites, with their cases in `tests/providers/` and vector reading in `tests/support/` |
| `tools/` | `generateVectors.ts`, the TS half of the interop vector generator |

Its only dependency inside the repo is `binacle-compact-notation`, the shared parser for the compact strings
the vectors are written in.

## 🧪 Tests

```bash
just test vipaq-ts-unit          # from the repo root
```

Two of the suites are the cross-language ones and matter more than the rest:

- **`interop.test.ts`** decodes what C# encoded and checks it against the **input**, never byte-for-byte
  against the C# bytes. Two encoders may make different valid blobs; both must decode to the same result.
- **`interopIntegrity.test.ts`** catches the drift that happens when only one side is regenerated.

## 🔄 Regenerating the interop vectors

```bash
just regen vipaq-interop-vectors # from the repo root
```

That runs the C# generator **and** `tools/generateVectors.ts` in one recipe on purpose: both halves write from
the same `input.json`, and regenerating one alone is exactly the drift the integrity test exists to catch.

## ⚠️ Read the protocol first

Every value is bounded - dimensions and coordinates must be in `[0, 65,535]`. Where anything here disagrees
with [`../../PROTOCOL.md`](../../PROTOCOL.md), the protocol wins and this code is wrong.
