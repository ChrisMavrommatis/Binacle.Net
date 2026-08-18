---
description: CI - what is left after the workflow restructure landed, and the gap the next workflows session inherits
paths:
  - ".github/**"
---

# CI - what is left after the workflow restructure

**Status: the build is done. Landed 2026-08-17 and 2026-08-18**, over two sittings - one review at a time, at
the maintainer's call. The workflows were split over a set of shared composite actions in `.github/actions/`;
the pull request gate got its `changes`/`gate` pair; concurrency on the entry points; `actionlint`; two run
summaries; the site link check.

**How it all works now lives in the CI/CD doc**, and why it is shaped that way in the CI/CD decisions ledger.
This file is down to what is *not* done, so it restates neither.

## The one thing needing hands, not code

**Point branch protection at `Pull Request / Gate`, and nothing else.** Until then every pull request waits on
a required check that no longer reports: the test suite lost its `pull_request` trigger when the gate started
calling it. **This is the last protection edit that should ever be needed** - every job under `gate` can be
renamed freely afterwards.

## The one real gap, for the workflows session

**`actionlint` cannot lint composite actions**, and no flag makes it: hand it an `action.yml` and it reports
`"jobs" section is missing`, because it treats every input as a workflow. Their **inputs** are still checked,
from the caller's side - a missing required input or a misspelled name is reported against the `uses:` line,
naming the action and listing what it accepts.

**What is unchecked is their shell: 38 lines**, against 132 in the workflows that get actionlint and
shellcheck. Four of the five blocks are the near-identical `install-*` download-and-checksum scripts.

**One check already sits in `just check actions` by hand**: a grep for a `vars` or `secrets` expression in a
manifest. That is not hypothetical - it failed the first CI run on 2026-08-18, from an expression written
inside an input `description`, because the runner evaluates the whole manifest before any step runs.

**Closing the rest means extracting `runs.steps[].run` and piping it to shellcheck** - a tool to build rather
than one to install, which is why it did not land with the rest. It belongs in `just check actions`, beside
that grep.

## Two smaller ones, same session

- **The deploy workflows write no run summary.** The gate and the release `publish` job do. A deploy's would be
  the commit, the marker tag and the URL - three greps through a log today.
- **Neither site deploys on a push.** Both are `workflow_dispatch`, and whether `docs/**` or `web/**` should
  trigger one is **not a CI question** - those two folders are written in their own session, and the deploy
  being a button is part of how that works. It is worth an answer and it was never this file's to give.
