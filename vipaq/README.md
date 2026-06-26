# ViPaq

> **Experimental — may change.**

ViPaq is a compact binary format for one packing result: a single bin plus a list of placed items. It exists in
two implementations of the same wire format — a canonical C# library and a hand-maintained TypeScript mirror.

**The format is defined in [PROTOCOL.md](PROTOCOL.md).** That is the normative spec; read it before changing
either implementation. Both sides must stay byte-compatible by hand — there is no codegen or shared schema.

## Layout

| Path | What it is |
|---|---|
| [PROTOCOL.md](PROTOCOL.md) | Normative wire-format spec — header, body, widths, value limits, compression, decisions log |
| `src/Binacle.ViPaq/` | C# library — the canonical implementation |
| `test/Binacle.ViPaq.UnitTests/` | C# unit tests |
| `binacle-vipaq/` | TypeScript mirror (`npm test`) |
| `test-vectors/` | Shared, language-neutral test data read by both suites — see its `README.md` |

## Key rule

Every dimension and coordinate must be in `[0, 2^53 − 1]` (`9,007,199,254,740,991`, JavaScript's
`Number.MAX_SAFE_INTEGER`) — the largest integer both runtimes hold exactly. See PROTOCOL.md §5.

## More

- Agent notes (non-normative): `../.agent-docs/vipaq/README.md`, `../.agent-docs/vipaq/typescript.md`
- Plans: `../.plans/vipaq-integer-range-spec.md`, `../.plans/vipaq-cross-language-testing.md`
</content>
