# ViPaq

> **Experimental — may change.**

ViPaq is a compact binary format for one packing result: a single bin plus a list of placed items. It is designed
to be stored and moved as a short base64 text token.

**The format is defined in [PROTOCOL.md](PROTOCOL.md).** That is the normative spec, and it is language-neutral —
anyone can read it and write an implementation in the language they prefer. Read it before changing any
implementation here.

## Implementations

Both live in this repo, and both must stay byte-compatible **by hand** — there is no codegen and no shared schema.
The shared test vectors are what keep them honest.

| Path | What it is |
|---|---|
| [PROTOCOL.md](PROTOCOL.md) | Normative wire-format spec — header, body, widths, value limits, compression |
| `src/Binacle.ViPaq/` | C# library — the reference implementation, produces the golden bytes |
| `packages/binacle-vipaq/` | TypeScript mirror (`npm test`) |
| `test-vectors/` | Language-neutral test data read by both suites — see its `README.md` |
| `test/` | C# unit tests, benchmarks, performance tests |
| `tools/` | Data generators |
| `data/` | Frozen real packing results used by the benchmarks |

## Key rule

Every dimension and coordinate must be in `[0, 65,535]`. Encoding a value above that is an error. See
[PROTOCOL.md](PROTOCOL.md) §5.

## More

- Why the format is shaped this way — `../.agents/plans/vipaq/decisions.md`
- Agent notes (non-normative) — `../.agents/docs/vipaq/README.md`, `../.agents/docs/vipaq/typescript.md`
- How the two implementations are tested against each other — `../.agents/docs/vipaq/cross-language-testing.md`
