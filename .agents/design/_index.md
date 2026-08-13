---
description: Manifest of every file under .agents/design, grouped by area. Regenerate with just agents all.
---

# Agent Design Index

Every design record in `.agents/design/`, grouped by area. The settled design behind the docs — the
decisions (why) and the findings (measured evidence). Permanent and citable; read the one you need.

## CI/CD

```yaml
- file: ci-cd/decisions.md
  description: "CI/CD decisions ledger — why the release pipeline is tag-triggered, stages on GHCR and copies to Docker Hub by digest, why the prerelease guard is metadata-action's rather than a job-level skip, why the notes come from CHANGELOG.md, the pinning rules, and the open questions about the PR gate and supply-chain attestation."
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
  paths: ["vipaq/**"]
```
