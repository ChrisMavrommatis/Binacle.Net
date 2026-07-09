---
description: Persistent ViPaq session prompt. Read first, work, then update it before you finish.
---

# ViPaq — session prompt

Read this first. It is the only thing you need to start. **Update it before you finish** (last section).

## Mission

Rebuild ViPaq to the new wire format: C# first, then the TypeScript mirror, then regenerate the test vectors.

## What is true

**`vipaq/PROTOCOL.md` is the format.** It is the only authority. It is language-neutral on purpose — anyone can
implement it. If the code disagrees with it, the code is wrong.

**`reference/` is false by default.** Nine files written while the format was still being argued about, against a
wire that no longer exists. Open one only if you are stuck or want an idea, and check anything you take from it
against the spec and the code. Do not maintain it. Do not cite it as a reason.

The same is true of `.agents/docs/vipaq/*`: those describe the **shipped library**, which has not been rebuilt.
Accurate about the code, wrong about the format.

## How to work

**Do not write a plan document before you write code.** Build. When the code forces a decision, make it there,
with the code in front of you. If the decision changes the wire, write it into `PROTOCOL.md`. If it doesn't,
write it nowhere.

**Never commit, stage, or push.** The human commits. Leave changes in the working tree.

**Nothing ships without a measured gain** — smaller base64, faster, less memory, or a concrete simplicity win.
Default to no.

**Do not add a project, shared library, or abstraction to solve a local problem.** Copy it instead. Share only
when a third consumer actually appears — this rule exists because we broke it twice.

**Do only what you set out to do.** If a fix pulls you into a restructure, stop and write it down instead.

## Where you are now

**Last session (2026-07-09):** wrote the spec. Nothing has been implemented against it yet.

**Next:** implement the new wire in C# (`vipaq/src/Binacle.ViPaq/`).

**The first decision the code will force:** `PROTOCOL.md` §6 does not name a compression codec, and `Version`
pins it. The spec is not final until you pick one, and the TypeScript mirror cannot start without it. Pick it,
write it into §6, remove it from §12.

Open work, verified and not in the spec, is listed in [README.md](README.md).

## Before you finish — update this file

This prompt is the handover. A session that leaves it stale has failed the next one.

- Rewrite **"Where you are now"**: what you did, what is next, what decision is now blocking.
- If you settled something that changes the wire, it belongs in `PROTOCOL.md`, not here.
- If a `reference/` file finally proved useless, delete it. That folder should shrink to nothing.
- Keep this file short. It is a prompt, not a log. Nothing here should need to be read twice.
