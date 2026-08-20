---
description: Everything under sites/ publishes to the internet and is written in its own session. Two carve-outs - the three repo-facing READMEs, and security fixes to sample files.
load: always
when: before editing anything under sites/
paths:
  - "sites/**"
---

# Never edit anything under `sites/`

Every published site lives there, one directory each, and all of them go to the internet. `sites/docs/` is the
versioned documentation site, `sites/web/` is the marketing site.
(`.agents/docs/` is a different thing entirely - the agent reference layer, and editing it is fine.)

They are written in their own dedicated session, by an agent whose whole job is that content. Read them
freely. If work needs a page written or corrected, **write down what the page must say** in the plan or
release file that owns the work, and leave the writing to that session.

**One carve-out: the three repo-facing READMEs.** `sites/README.md`, `sites/docs/README.md` and
`sites/web/README.md` are for whoever opens the folder, not for a visitor - both `_config.yml` files list
`README.md` under `exclude`, so no build publishes them. A coding session may write them, and must, because
`every-folder-has-a-readme.md` covers them like any other folder. Nothing else under `sites/` is included:
not a page, not a layout, not front matter.

**A second carve-out: a security fix to a downloadable sample file.** The
`sites/docs/collections/_versions/**` folders hold files readers download and run - compose files, Kubernetes
manifests, config json. When an analyser flags one as vulnerable, a coding session may fix the **file**.
Narrow on purpose: it must touch no prose, no front matter and no `.md`, and it must match what repo-root
`samples/` already does. Record every use of it in the plan that owns the work.

**Why:** a change made in passing gets published without anyone reviewing it as public writing. The READMEs are
excluded from both builds, so they are repo files that happen to sit under `sites/`. The sample carve-out
exists because these files are the only public attack surface in the repo, and a fix to `samples/` does not
reach the frozen copies.
