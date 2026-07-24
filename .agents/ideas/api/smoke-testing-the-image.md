# Idea: smoke test the built docker image over HTTP

**Status:** Unvetted idea. Nothing built. Prompted by asking how we'd prove the image works before a release.

## The problem

Everything we test today runs **in process**. `Binacle.Net.IntegrationTests` boots the app with
`WebApplicationFactory<IApiMarker>` and then replaces the presets with three test-only ones, so it never loads
the config files we actually ship. Nothing anywhere touches the image.

That leaves a whole class of failure invisible until someone pulls the tag:

- a config file that didn't get copied into the image, or landed at the wrong path
- a module env var in the compose file that no longer switches anything on — renamed flags fail silently, the
  app starts and the routes are just absent
- a connection string that works on the host and not inside the container network
- the `VERSION` build arg never reaching the process
- the entry point, the port, or the published runtime being wrong

All of it is packaging and wiring. None of it is C# logic, which is why the current suites can't see it.

## The idea

A separate test project that talks to a **running container over HTTP**, like any other client. It answers one
question: *did this image come up wired correctly?* Not *are the answers right* — that's the existing suites.

The rule that keeps it honest:

> **Every check must be able to fail for a reason that has nothing to do with the C# logic.**

If a check can only break when an algorithm breaks, it belongs in the integration suite instead.

## What it would check — by failure, not by endpoint

Group the tests by the failure they catch, so the name of the failing file already says what's wrong.

| Group | Catches | Roughly |
|---|---|---|
| Startup | app didn't start, or a dependency is down | `/_health` is `Healthy`, and every entry inside it is too; plain HTTP is served without a redirect |
| Configuration | `Config_Files/` missing or unparseable | the shipped presets are listed for v3 and v4; read one all the way through to its bins |
| Modules | a module flag stopped switching anything on | OpenAPI docs, Swagger, Scalar, the UI site, and the ServiceModule mounted-but-protected |
| Packing | endpoints reachable but can't run | one fit and one pack on v3 and v4, plus one through a shipped preset |
| ServiceModule | storage unreachable from inside the container | get an admin token, reject a wrong password, use the token on an admin route |
| Image | the `VERSION` build arg never reached the process | the version stamp in the response |

Two checks are worth calling out because one request proves two things:

- **An unauthenticated admin route must answer 401.** A 404 means the module never mounted; a 200 means an admin
  route is open to anyone. Only 401 means mounted *and* protected.
- **The default admin can get a token.** That covers the whole storage path in one request — the startup task
  wrote the seeded account, and the token endpoint read it back.

## What it should deliberately *not* do

Scope creep is the way this turns into a slow second integration suite.

- **No project reference to the app.** That's how a smoke suite quietly becomes an integration suite. Talk HTTP
  only, read responses as JSON documents rather than the app's contract types.
- **No coverage collection** — the code under test runs in another process.
- **No shared fixtures or scenario data.** Payloads stay inline and tiny.
- **No numeric assertions.** Never assert a coordinate or a bin count; that's the algorithm's job, and it makes
  the suite fail on every legitimate packing change.
- **No validation or error-path cases.** Wrong input handling is logic.
- **Keep it fast.** If it takes longer than the image takes to build, it's doing too much.

## Inventory — what we'd need to do

**The test project**

- [ ] `api/test/Binacle.Net.SmokeTests` — xunit.v3 on the Microsoft Testing Platform, like the other suites.
      No `ProjectReference`, no coverage package.
- [ ] An assembly fixture holding one `HttpClient`, with `AllowAutoRedirect = false` — the container serves
      plain HTTP, so a misconfigured HTTPS redirect should show up as a 307, not as a connection error.
- [ ] A readiness wait in the fixture (poll `/_health`, give up after ~90s) so a slow start reads as slow rather
      than broken, and every test can then assume the app is up.
- [ ] A failure message on that timeout that says how to start one — it's the message you'd hit most often.
- [ ] Env knobs for the base URL, the expected version, and the admin credentials, so it can be pointed at a
      staging instance instead.
- [ ] The six test files above.

**Scripts**

- [ ] Split the publish + `docker build` out of `config/build.sh` into its own script. `build.sh` starts compose
      in the foreground and can't hand the terminal back, so it can't build-then-test.
- [ ] `config/smoke.sh` — build, start detached, run the suite, stop. Flags to keep the container up afterwards
      and to skip the build and test whatever is already running. No `sleep`; the suite does its own waiting.

**Wiring**

- [ ] Add the project to `Binacle.Net.slnx`.
- [ ] Repo-layout row in `.agents/docs/README.md`; a Smoke section in `config/README.md`.
- [ ] Decide whether the release workflow runs it against the image it just built (see open questions).

## Things to watch out for

Found the hard way while sketching this; they'd each cost an hour otherwise.

- **`Presets.json` is UTF-8 with a BOM.** A parser that chokes on it returns a preset list that looks fine but
  has no bins in it. So read a preset all the way through, don't just count them.
- **v4's fit status is its own enum.** It isn't v3's — `BinFitResultStatus` is `Unknown/Fits/DoesNotFit/EarlyExit`
  against v3's `AllItemsFit/NotAllItemsFit/EarlyFail_*`.
- **`/scalar` answers 302 to `/scalar/`.** With redirects off, request the trailing slash.
- **`Guid.Empty` is not a usable "missing" id.** The admin routes reject it as invalid before they look it up, so
  a 404 check comes back 422. Use an arbitrary real GUID.
- **xUnit1051 fires on every HTTP call in a test method.** Centralising the calls on the fixture — passing
  `TestContext.Current.CancellationToken` in one place — is cleaner than annotating ~20 call sites.
- **An empty connection string falls through to the next provider**, because resolution checks
  `string.IsNullOrWhiteSpace`. Useful for running the app locally on SQLite while pointing the suite at it.

## Open questions

- **Does CI run it?** Running it in the release workflow against the freshly built image is the whole point of
  having it, but it means docker-in-CI and a compose stack per run. Local-only first is the cheap start.
- **Compose or Testcontainers?** Compose reuses `docker-compose.build.yml`, so the suite tests the same stack we
  hand to users. Testcontainers would own the lifecycle from inside the tests and drop `smoke.sh` — more control,
  another dependency, and it stops testing our compose file.
- **How much of the ServiceModule?** It's the only group that needs a storage backend, and so the only reason the
  smoke stack needs more than one container.
- **Where does the version stamp get asserted?** Today the only HTTP surface for it is the UI footer, which ties
  an image check to the UI module being on. A version field on the health response would decouple them.

## Related

- the API configuration doc — the config file layout and env-var conventions these checks are really testing
- the modules doc — the module flags
- the build-topology doc — the publish → `docker build` → compose chain
