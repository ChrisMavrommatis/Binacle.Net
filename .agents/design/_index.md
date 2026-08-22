---
description: Manifest of every file under .agents/design, grouped by area. Regenerate with just agents all.
---

# Agent Design Index

Every design record in `.agents/design/`, grouped by area. The settled design behind the docs — the
decisions (why) and the findings (measured evidence). Permanent and citable; read the one you need.

## General

```yaml
- file: decisions.md
  description: "General decisions ledger — why the repository moved to the binacle-labs organization, what moved with it and what deliberately did not, the three signing identity bands, the rule that a version is named only where the version is the fact, and how the agent reference layer is kept honest against the code."
  paths: ["NOTICE", "README.md", "SECURITY.md", "CHANGELOG.md", "Dockerfile", "sites/docs/**"]
```

## API

```yaml
- file: api/decisions.md
  description: "API decisions ledger — why a module-off document carries no `429` and what guarantees it, what the generated documents are a document of, and why the API sends no HSTS header."
  paths: ["api/**"]
```

## CI/CD

```yaml
- file: ci-cd/decisions.md
  description: "CI/CD decisions ledger — why the release pipeline is tag-triggered, stages on GHCR and copies to Docker Hub by digest, why the prerelease guard is metadata-action's rather than a job-level skip, why the notes come from CHANGELOG.md, the pinning rules, why lychee is a pinned binary rather than its own action, and the open questions about the PR gate and supply-chain attestation."
  paths: [".github/workflows/**"]
```

## Lib

```yaml
- file: lib/decisions.md
  description: "Lib decisions ledger — why Algorithm.Best races a different set per path, where the packing vocabulary lives, why there are two tests kernels, and the open parallelization question."
  paths: ["lib/**"]
- file: lib/findings.md
  description: "Lib findings — the measured evidence (algorithm racing cost, parallel racing gain) behind the decisions."
  paths: ["lib/**"]
```

## ViPaq

```yaml
- file: vipaq/decisions.md
  description: "ViPaq decisions ledger — the locked decisions and their reasons, plus the open questions."
  paths: ["vipaq/**"]
- file: vipaq/findings.md
  description: "ViPaq findings — the measured evidence (base64 size, encode/decode time) behind the decisions."
  paths: ["vipaq/**"]
- file: vipaq/history.md
  description: "ViPaq design history — superseded throwaway-prototype measurements (2026-07-05) that informed the locked decisions. Reference only, not current truth."
```
