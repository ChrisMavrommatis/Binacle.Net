# Binacle.Net.UIModule unit tests

The demo UI's own C# - the applet list, the four page models and the error page. Everything interactive on a
page is TypeScript in `packages/binacle-net-ui` and is tested there; nothing here renders a `.cshtml`.

| Folder | What it is |
|---|---|
| `Tests/` | one file per page model or service |

Razor generates internal page classes, so every type under test is `internal` and the module opens itself with
`InternalsVisibleTo`. That is the same arrangement the other modules use.

```bash
just test api-ui-unit
```

**What is not here.** Routing, the error-page middleware and whether a reserved path answers with JSON instead
of a web page all need a booted host, so they are integration work rather than unit work. The two middlewares
in `ModuleDefinition.UseUIModule` are inline lambdas and cannot be constructed on their own.
