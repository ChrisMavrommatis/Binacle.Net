# Assets

Static files the two sites share - the icons at the root, the third-party libraries they load, and the
logos. Edit the originals here; the copies in the sites are overwritten on every build.

## 📂 What is in it

| Path | What it is |
|---|---|
| `*.png`, `*.ico` | Favicons, the apple touch icon and the android chrome icons. Served from the site root, so the names matter |
| `lib/` | Vendored third-party front-end libraries, checked in rather than installed |
| `media/` | Logos and marks - `logo/` (ours, every size), `github/` and `docker/` (theirs) |
| `assets.proj` | A no-build project so these files show up in the solution. It compiles nothing |

## 📦 The vendored libraries

Downloaded, not installed - nothing in `package.json` pulls them, and nothing rebuilds them. Each folder is
the vendor's own output, dropped in as-is.

| Folder | What the sites use it for | Version |
|---|---|---|
| `lib/beercss/` | The Material-style CSS framework, plus the Material Symbols fonts | `version` file, `3.11.11` |
| `lib/swagger-ui/` | The Swagger UI bundle the docs site embeds | `version` file, `5.11.0` |
| `lib/alpine/` | Alpine.js, for the small interactive bits | not recorded |
| `lib/material-dynamic-colors/` | Theme colour generation for beercss | not recorded |

**To upgrade one, replace the files and update its `version` file.** Two of them do not have one - if you
upgrade those, add it, because nothing else in the repo records what shipped.

## 🚀 Copying them into the sites

```bash
just assets                      # after changing anything here
```

That runs the two gulp tasks in the root `gulpfile.js`, which copy every `.js`, `.css`, `.woff2`, image and
icon into [`sites/docs/`](../sites/docs) and [`sites/web/`](../sites/web). It is also part of `just install`,
so a fresh clone gets them without asking.

The copy is one-way and does not delete. Renaming a file here leaves the old name behind in both sites until
someone removes it by hand.

## ⚠️ The UI module does not read this folder

`api/src/Binacle.Net.UIModule/wwwroot/vendor/` holds its **own** copy of beercss and
material-dynamic-colors, checked in beside the module and never touched by `just assets`. The two are not in
step - the module is on beercss `3.10.8`, this folder on `3.11.11`. Upgrading here does not upgrade the
module, and it never has.
