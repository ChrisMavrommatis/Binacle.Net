# Binacle.Net.Kernel unit tests

One folder per Kernel feature, each holding its own `Tests/` and `Providers/`. The Kernel is shared by every
module, so a feature's tests stay next to that feature and nothing reaches across.

| Folder | Covers |
|---|---|
| `Network/` | `IPEntry` - how a configured IP entry is read, and what each spelling admits |
