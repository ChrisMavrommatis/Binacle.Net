# .config

The .NET local tool manifest, and nothing else. `dotnet-tools.json` pins the two command-line tools this repo
installs into itself rather than expecting on your machine:

| Tool | Command | Used by |
|---|---|---|
| `dotnet-reportgenerator-globaltool` | `reportgenerator` | `just coverage report`, which merges the last run into an HTML report |
| `dotnet-sonarscanner` | `dotnet-sonarscanner` | The Sonar analysis workflow |

## 🛠️ Getting them

```bash
dotnet tool restore
```

`just coverage report` runs that itself, so there is normally nothing to do by hand.

Both are pinned with `rollForward: false` - a restore installs that exact version or fails, rather than
quietly moving to a newer one and changing the report.
