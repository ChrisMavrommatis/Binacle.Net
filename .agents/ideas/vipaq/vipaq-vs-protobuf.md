# Idea: Compare ViPaq to Protobuf

**Status:** Unvetted idea.

## What

Put ViPaq (our compact binary packing format, `vipaq/`) side by side with Protocol Buffers for the same
packing-result payloads. See where ViPaq wins and where it doesn't.

## Why

We hand-built ViPaq. Before we lean on it more, we should know how it stacks up against a standard everyone
already uses. If Protobuf is close on size and much easier to maintain, that's worth knowing. If ViPaq is a lot
smaller on our shapes, that justifies keeping it.

## What to measure

- Encoded size — uncompressed and gzipped — over real packing results (small, medium, big item counts).
- Encode and decode speed on both C# and TypeScript.
- Maintenance cost — schema evolution, cross-language mirrors, tooling.

## Open questions

- Which Protobuf? Plain proto3, or a variant.
- Do we compare against a naive `.proto` for our types, or a hand-tuned one.
- Include other options too (MessagePack, CBOR, flatbuffers), or keep it just Protobuf.

## Related

- `.agents/docs/vipaq/README.md`, `vipaq/PROTOCOL.md`
