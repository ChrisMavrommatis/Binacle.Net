---
description: Repo-root docs/ and web/ publish to the internet and are written in their own session. One carve-out for security fixes to sample files.
load: always
when: before editing anything under repo-root docs/ or web/
paths:
  - "docs/**"
  - "web/**"
---

# Never edit repo-root `docs/` or `web/`

Both publish to the internet. `docs/` is the versioned documentation site, `web/` is the marketing site.
(`.agents/docs/` is a different thing entirely - the agent reference layer, and editing it is fine.)

They are written in their own dedicated session, by an agent whose whole job is that content. Read them
freely. If work needs a page written or corrected, **write down what the page must say** in the plan or
release file that owns the work, and leave the writing to that session.

**One carve-out: a security fix to a downloadable sample file.** The `docs/collections/_versions/**` folders
hold files readers download and run - compose files, Kubernetes manifests, config json. When an analyser flags
one as vulnerable, a coding session may fix the **file**. Narrow on purpose: it must touch no prose, no front
matter and no `.md`, and it must match what repo-root `samples/` already does. Record every use of it in the
plan that owns the work.

**Why:** a change made in passing gets published without anyone reviewing it as public writing. The carve-out
exists because these files are the only public attack surface in the repo, and a fix to `samples/` does not
reach the frozen copies.
