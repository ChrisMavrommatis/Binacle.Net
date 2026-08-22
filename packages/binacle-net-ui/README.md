# binacle-net-ui

The front end behind the two interactive apps on the website - the packing demo and the ViPaq protocol
decoder. Alpine.js components plus a Three.js visualizer, written in TypeScript.

It exports two Alpine plugins and nothing else:

```ts
import {packingDemoPlugin} from 'binacle-net-ui';
import {protocolDecoderPlugin} from 'binacle-net-ui';
```

Each one registers the set of Alpine components its page needs, so a page turns on with a single plugin call.

## 📂 What is in it

| Folder | What it is |
|---|---|
| `src/*Plugin.ts` | The two entry points. Each is a short file that registers components - the logic is in `core/` |
| `src/core/` | The components themselves - the demo app, the visualizer, the decoder, plus the field, logger and error dialog they share |
| `src/apiModels/` | The wire shapes for the API call - request, response, bin, item, packed and unpacked item |
| `src/viewModels/` | What the page binds to - bin, box, item, control, decoded result, errors |
| `src/models/` | Internal types - dimensions, coordinates, scene data, visualizer state |
| `src/utils/` | The Three.js scene work: build the bin, add and clear items, camera position and field of view, theme colours |
| `src/types/` | Ambient declarations for Alpine and the globals the pages set |

## 🚀 Where it runs

[`sites/demo`](../../sites/demo) imports it from `_js/packing_demo.js` and `_js/protocol_decoder.js`, and webpack
bundles it. The import resolves through the npm workspace - nothing is copied, so a change here shows up on the
next webpack pass. `just serve demo` from the repo root watches both.

The demo calls a live API, at the `api_url` in that site's `_config.yml`. The decoder does not - it decodes in
the browser, through `binacle-vipaq`.

## ⚠️ It is not the UI module

`api/src/Binacle.Net.UIModule` ships its own packing demo, with its own hand-written
`wwwroot/js/PackingVisualizer.js`. It does **not** import this package, and nothing keeps the two in step. A
fix here is not a fix there.

There are no tests. The apps are checked by using them.
