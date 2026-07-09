# Pending Actions

Things that can't be done from the repo — require external systems or manual steps. **Lives in `.agents/` root
alongside [`release-notes.md`](release-notes.md); maintain both as work lands** (release/CI actions here, changelog
entries there).

---

## GitHub

- [ ] Update the `API_PROJECT_PATH` Actions variable (repo Settings → Secrets and variables → Actions → Variables)
  from `src/Binacle.Net/Binacle.Net.csproj`
  to   `api/src/Binacle.Net/Binacle.Net.csproj`
  Affects: `release-docker-image.yml` workflow (publish step)
  Also tracked in [`release-notes.md`](release-notes.md) so it isn't missed at release time.

---

## Verification gaps

- [ ] **Run a docker image build.** The `Binacle.Geometry` extraction was verified against every C# suite
  (including ServiceModule) and the TS suites, all green — but the docker image build was skipped by choice.
  Run it once for a fully green sweep.
