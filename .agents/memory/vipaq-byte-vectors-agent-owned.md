---
name: vipaq-byte-vectors-agent-owned
description: ViPaq byte-exact golden vectors carry a byte-by-byte comment — a wall of hex nobody can check is not a test
type: convention
---

ViPaq's hand-derived wire bytes — `ExactBytesProvider` in the C# unit tests, `tests/providers/ExactBytes.ts` and
`tests/providers/LittleEndianCases.ts` on the TypeScript side — are written and maintained by the agent. They are
not hand-verified by the maintainer, so they have to justify themselves.

**Why:** byte-level tests are opaque to read. Without a derivation nobody can check them at all, and a wrong
vector locks in wrong behaviour for as long as it stays green.

**How to apply:** derive each vector from the spec, then comment it byte by byte, so any single row can be
spot-checked without recomputing the rest. Never add a row you cannot explain. The wire is normative in
`vipaq/PROTOCOL.md`; which vectors are generated and which stay hand-authored is settled in `$vipaq#D15`.
