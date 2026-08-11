---
description: Manifest of every file under .agents/design, grouped by area. Regenerate with just agents all.
---

# Agent Design Index

Every design record in `.agents/design/`, grouped by area. The settled design behind the docs — the
decisions (why) and the findings (measured evidence). Permanent and citable; read the one you need.

## CI/CD

| File | Description |
|---|---|
| [ci-cd/decisions.md](ci-cd/decisions.md) | CI/CD decisions ledger — why the release pipeline is tag-triggered and promotes by digest, why workflows call just recipes, the pinning rules, and the open questions about the PR gate and supply-chain attestation. |

## Lib

| File | Description |
|---|---|
| [lib/decisions.md](lib/decisions.md) | Lib decisions ledger — why Algorithm.Best races a different set per path, and the open parallelization question. |
| [lib/findings.md](lib/findings.md) | Lib findings — the measured evidence (algorithm racing cost, parallel racing gain) behind the decisions. |

## ViPaq

| File | Description |
|---|---|
| [vipaq/decisions.md](vipaq/decisions.md) | ViPaq decisions ledger — the locked decisions and their reasons, plus the open questions. |
| [vipaq/findings.md](vipaq/findings.md) | ViPaq findings — the measured evidence (base64 size, encode/decode time) behind the decisions. |
| [vipaq/history.md](vipaq/history.md) | ViPaq design history — superseded throwaway-prototype measurements (2026-07-05) that informed the locked decisions. Reference only, not current truth. |
