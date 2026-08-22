# Binacle.Net.UIModule integration tests

Boots the API with the demo UI switched on, and again with it off, and asks what each route answers with.
This is where the split between a web page and an API response is proved - a page route gets HTML, everything
under a reserved prefix must not.

| Folder | What it is |
|---|---|
| `Tests/` | one file per question: error routing, page content, and the module switched off |

```bash
just test api-ui-integration
```

**What will bite you.** `Feature.Manager` is process-global static state, set while a host builds, so two
hosts that disagree about `UI_MODULE` cannot be alive at the same time - the second to boot answers for both.
Every class here is in one collection with parallelisation off for that reason. Without it the
module-switched-off tests see the module switched on and fail about one run in two.

Nothing here requests a javascript bundle or a stylesheet. `wwwroot/` is generated and gitignored, so on a
clone that has not run the javascript build there is nothing to serve. These tests assert that a page **asks**
for the right file; `tooling/smoke` asserts the file is **there**, against a built image.
