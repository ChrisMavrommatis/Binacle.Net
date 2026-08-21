---
description: The packing demo shows the HTTP call it just made, against this host, ready to copy
paths:
  - "api/src/Binacle.Net.UIModule/**"
  - "packages/binacle-net-ui/**"
---

# Idea: show the request the demo just made

**Status:** Unvetted idea. Nothing decided, nothing scheduled.

## What

The packing demo already holds real numbers - the visitor's own boxes and items - and it already builds a
request body from them. Show it. A panel beside the results with the exact call that was sent and the response
that came back, ready to copy.

```
POST http://localhost:8080/api/v3/pack/by-custom
Content-Type: application/json

{ "bins": [ ... ], "items": [ ... ], "parameters": { ... } }
```

Today nothing on any page in the image shows a request. Someone who has watched the visualizer work still has
to open the API documentation and rebuild the same call by hand.

## Why the UI module is the right host for it

**The URL is only useful when it is theirs.** The module is served from the instance the visitor is running, so
the host in that snippet is a host they can paste into their own code. A copy of the same panel on a public
demo site can only ever show a public host nobody will call.

That is the one thing this feature has that most do not: it is worth more inside the image than outside it.

## Open questions

- **What form.** Raw HTTP, a `curl` line, or a language snippet. `curl` is the one that pastes into a terminal;
  raw HTTP is the one that matches the documentation. Probably not both.
- **Where the URL comes from.** The demo's `baseUrl` is empty by default and the browser resolves it relative,
  so the panel has to read the page's own origin rather than the value handed to the component.
- **Which API version.** The component posts to `/api/v3/pack/by-custom` today. A panel that teaches people the
  call teaches whichever version it prints, so this interacts with the plan to move the shipped clients off v3 -
  printing v3 while the documentation recommends v4 is worse than printing nothing.
- **Whether it is inside the tool or around it.** If the panel lives in the shared component it lands on both
  hosts, where it is worth much less. If it lives in the Razor page it has to read the component's state, which
  is a seam that does not exist yet. This is the question to answer first; it decides the cost.
- **The response half.** Showing the response too doubles the panel and the visualizer already shows that
  result. It may be the request alone.
