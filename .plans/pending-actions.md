# Pending Actions

Things that can't be done from the repo — require external systems or manual steps.

---

## GitHub

- [ ] Update the `API_PROJECT_PATH` Actions variable (repo Settings → Secrets and variables → Actions → Variables)
  from `src/Binacle.Net/Binacle.Net.csproj`
  to   `api/src/Binacle.Net/Binacle.Net.csproj`
  Affects: `release-docker-image.yml` workflow (publish step)
