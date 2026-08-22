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

**Two hosts, one implementation.** Both compile it from source with their own webpack and ts-loader; the
import resolves through the npm workspace, so nothing is copied and a change here shows up on the next
webpack pass of each.

| Host | Entries |
|---|---|
| [`sites/demo`](../../sites/demo) | `_js/packing_demo.js`, `_js/protocol_decoder.js` |
| [the API's UI module](../../api/src/Binacle.Net.UIModule) | `_js/packing_demo.js`, `_js/protocol_decoder.js` |

`just serve demo` watches the site; `just serve api U` watches the module. **A change here lands on both, and
neither can be updated without the other** - so widen a signature first, move each host, then narrow.

The packing demo calls a live API. The site takes the address from `api_url` in its `_config.yml`; the module
passes an empty one, which means fetch relative from whatever host served the page. The decoder calls nothing
- it decodes in the browser, through `binacle-vipaq`.

## 🧪 Tests

```bash
just test packages-net-ui-unit
```

jsdom, because the components read `document` and `window` even where the logic under test does not.
`tests/model/` is the pure half - the randomizer and the view models. `tests/components/` calls each Alpine
component factory directly with a stub `$dispatch`, rather than starting Alpine.

**The Three.js half is not covered and is not meant to be.** The visualizer and the scene helpers need a WebGL
context, so a test there could only assert that a call happened.
