---
description: CI - what is left after the workflow restructure landed, and the gap the next workflows session inherits
paths:
  - ".github/**"
---

# CI - what is left after the workflow restructure

**Status: the build is done. Landed 2026-08-17 and 2026-08-18**, over two sittings - one review at a time, at
the maintainer's call. Seven workflows over nine shared actions; the pull request gate with its
`changes`/`gate` pair; concurrency on the entry points; `actionlint`; two run summaries; the site link check.

**How it all works now lives in `$ci-cd`**, and why it is shaped that way in `$ci-cd/decisions`. This file is
down to what is *not* done, so it restates neither.

## Two things needing hands, not code

- **Point branch protection at `Pull Request / Gate`, and nothing else.** Until then every pull request waits
  on a required check that no longer reports: the test suite lost its `pull_request` trigger when the gate
  started calling it. **This is the last protection edit that should ever be needed** - every job under `gate`
  can be renamed freely afterwards.
- **Watch one weekly Dependabot run.** Four pinned action SHAs now live only in `.github/actions/`
  (`extractions/setup-just`, `actions/setup-dotnet`, `actions/setup-node`, `ruby/setup-ruby`), and GitHub
  documents the `github-actions` ecosystem as covering `.github/workflows` without saying whether it scans a
  composite `action.yml` in a subfolder. **If it does not, those four silently stop being updated**, which is
  worse than an unpinned action because nothing reports it. If the answer is no, they come back out into a
  workflow file.

## The one real gap, for the workflows session

**`actionlint` cannot lint composite actions**, and no flag makes it: hand it an `action.yml` and it reports
`"jobs" section is missing`, because it treats every input as a workflow. Their **inputs** are still checked,
from the caller's side - a missing required input or a misspelled name is reported against the `uses:` line,
naming the action and listing what it accepts.

**What is unchecked is their shell: 38 lines**, against 132 in the workflows that get actionlint and
shellcheck. Four of the five blocks are the near-identical `install-*` download-and-checksum scripts.

**Closing it means extracting `runs.steps[].run` and piping it to shellcheck** - a tool to build rather than
one to install, which is why it did not land with the rest. It belongs as another recipe in the `check` module
beside `just check workflows`.

## Two smaller ones, same session

- **The deploy workflows write no run summary.** The gate and the release `publish` job do. A deploy's would be
  the commit, the marker tag and the URL - three greps through a log today.
- **Neither site deploys on a push.** Both are `workflow_dispatch`, and whether `docs/**` or `web/**` should
  trigger one is **not a CI question** - those two folders are written in their own session, and the deploy
  being a button is part of how that works. It is worth an answer and it was never this file's to give.
