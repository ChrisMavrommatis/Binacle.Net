# ViPaq

> **Stable as of Binacle.Net v3.0.0.** A future format change takes the next `Version` code rather than altering
> `Version = 0` — see "Room to grow" (§2.3) in [PROTOCOL.md](PROTOCOL.md).

ViPaq is a compact binary format for one packing result: a single bin plus a list of placed items. It is designed
to be stored and moved as a short base64 text token.

**The format is defined in [PROTOCOL.md](PROTOCOL.md).** That is the normative spec, and it **stands alone** —
everything needed to encode or decode a ViPaq blob is in that one file, with no dependency on any other document.
It is language-neutral: anyone can read it and write an implementation in the language they prefer. Read it before
changing any implementation here.

## 📦 Implementations

Both live in this repo, and both must stay byte-compatible **by hand** — there is no codegen and no shared schema.
The shared test vectors are what keep them honest.

| Path | What it is |
|---|---|
| [PROTOCOL.md](PROTOCOL.md) | Normative wire-format spec — header, body, widths, value limits, compression |
| `src/Binacle.ViPaq/` | C# library — the reference implementation, produces the golden bytes |
| [`packages/binacle-vipaq/`](packages/binacle-vipaq) | TypeScript mirror - see its `README.md` |
| `test-vectors/` | Language-neutral test data read by both suites — see its `README.md` |
| `test/` | C# unit tests, benchmarks, performance tests |
| `tools/` | Data generators |
| `data/` | Frozen real packing results used by the benchmarks |

## 📏 Key rule

Every dimension and coordinate must be in `[0, 65,535]`. Encoding a value above that is an error. See
[PROTOCOL.md](PROTOCOL.md) §5.

## 🔗 More

- The format, in full — [PROTOCOL.md](PROTOCOL.md)
- How the two implementations are held to it — [test-vectors/README.md](test-vectors/README.md)

**[PROTOCOL.md](PROTOCOL.md) is the only normative document.** Where anything else in the repo disagrees with
it, the protocol wins and the other file is wrong.
