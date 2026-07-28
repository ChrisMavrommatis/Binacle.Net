# Smoke test the built docker image over HTTP

**Status:** Designed, nothing built. Promoted from an idea on 2026-07-29, once the build/image split landed.
Local only for now - **no CI** until it has proved itself by hand.

## Read this first: this file is a seed, not a spec

It was written in a session that did the reasoning but not the building. It is deliberately opinionated so
there is something to argue with, and several of its calls are judgements made without writing a line of the
suite.

**Your first job is to disagree with it.** Investigate, then come back with what you think should be built. The
sections marked **Challenge this** are where a wrong call is most likely and most expensive. Saying "the two
profiles are wrong, here is why" is a better outcome than building two profiles because a file said so. What is
*not* up for grabs is the line in the next section - that one is load-bearing.

## The problem

Everything we test today runs **in process**. `Binacle.Net.IntegrationTests` boots the app with
`WebApplicationFactory<IApiMarker>` and replaces the presets with three test-only ones, so it never loads the
config files we ship. Nothing anywhere touches the image.

That leaves a class of failure invisible until someone pulls the tag: a config file that did not get copied or
landed at the wrong path; a module env var that no longer switches anything on, so routes are simply absent; a
connection string that works on the host and not inside the container network; the `VERSION` build arg never
reaching the process; the entry point, port or published runtime being wrong.

All of it is packaging and wiring. None of it is C# logic, which is why the current suites cannot see it.

**The mainstream .NET answer does not cover this.** `WebApplicationFactory`, Testcontainers and Aspire all host
the API in-process and containerize only its dependencies. There is no framework rung for "test the artifact";
the common shape is build, run the image, poll a health endpoint, assert over HTTP.

## The line - do not move this

> **Assert what the image contains and wires. Never assert what the algorithm computed.**

Re-running the integration assertions over HTTP buys the same coverage with worse diagnostics, ten times the
runtime, and a suite that goes red on every legitimate packing change. The integration tests own "is the answer
right", in-process, where a failure points at a line of C#.

The one place a known-good value **is** right: data that comes from the image's config files rather than from
the algorithm. The shipped presets are image content - they stay true when an algorithm changes and break when
a file does not get copied. Assert those hard. Assert a coordinate and you have built a second integration
suite.

The test to apply to any check you are tempted to add:

> Every check must be able to fail for a reason that has nothing to do with the C# logic.

## Two profiles

**Challenge this.** The reasoning: off-states do not interact, so one bare container catches every flag's
off-state at once and one all-on container catches every on-state - two runs instead of 2^7. If you find a flag
whose off-state depends on another flag, that argument is broken and the shape has to change.

| Surface | `bare` (no env) | `full` (all flags on) |
|---|---|---|
| `/api/v3/presets`, `/api/v4/presets` | 200 - core is not optional | 200 |
| `/_health` | 404 | 200 `Healthy`, carries `Version` |
| `/_debug` | 404 | 200, echoes the request |
| `/swagger/`, `/scalar/` | 404 | 200 |
| `/` (UI) | 404 | 200 HTML, its API connection string resolves |
| `/api/auth/token` | 404 | 200 for the seeded admin |
| `/api/admin/account/{id}` unauthenticated | 404 | **401** |

**The off column is the security test, and it is what justifies the second container.** `/_debug` echoes the
caller's headers and connection address, `/_health` exposes internals. If either ships on by accident, no
in-process test would notice, because the integration harness sets its own flags. The admin route is the same
shape in one request: 404 means never mounted, 200 means an admin route is open to the world, only 401 means
mounted **and** protected.

The bare run proves one more thing on its own: **the image starts with no configuration at all.**

## The boundary with the integration work

A second plan covers adding integration tests for what the in-process harnesses miss - the modules are off
there, so rate limiting and CORS are exercised nowhere. Keep this line and neither session duplicates the
other:

> **Behaviour goes to the integration suite, in process. Packaging stays here, in a container.**

"Does rate limiting return 429 when the module is on" is behaviour - fast in process, and a failure points at a
line of C#. "Is `RateLimiter.json` in the image at all" is packaging. "Does CORS echo a configured origin" is
behaviour. **"The shipped image has no `Cors.json`, so it allows no browser origin"** is packaging, and it is
worth a check here: only `Cors.Development.json` exists in the repo and `.dockerignore` strips
`**/*.Development.json`, so the image answers a cross-origin request with no `Access-Control-Allow-Origin`
header while a developer machine allows `http://localhost:7195` and `:7196`.

**One thing from that work lands on you regardless, and it will cost an hour if you miss it: the `full` suite
can trip the anonymous rate limit on itself.** With `SERVICE_MODULE` on, `ApiUsageAnonymous` is
`SlidingWindow::60/3600-30` - 60 anonymous requests an hour, a shared bucket, and the failure looks like random
429s rather than a limit. Options to weigh: keep the full profile under the limit, authenticate the suite so it
gets the higher subscription bucket, or raise the limit for the smoke stack in its compose file.

## How to look at the image, and how to talk to it

**Inspecting the contents.** The image has a shell, so the fastest way to see what actually shipped is to
override the entry point rather than to start the app:

```bash
docker run --rm --entrypoint sh binacle-net:local -c 'ls Config_Files Config_Files/*/'
docker run --rm --entrypoint sh binacle-net:local -c 'cat Config_Files/DiagnosticsModule/HealthChecks.json'
docker run --rm --entrypoint sh binacle-net:local -c 'find . -name "*.Development.json"'   # must be empty
docker inspect binacle-net:local --format '{{range .Config.Env}}{{println .}}{{end}}'      # BINACLE_VERSION
```

Everything in the "verified" section below was read that way. Some of those checks have no HTTP surface at all
(no `*.Development.json` in the image is the clearest), so **decide whether the suite shells into the image for
those or whether they stay a separate recipe.** A C# test that shells out to `docker` is a different kind of
test from one that speaks HTTP, and mixing them is worth a deliberate decision rather than a drift.

**Talking to it.** Bring a stack up, then talk plain HTTP as any client would:

```bash
just build image                 # publish + docker build -t binacle-net:local
just image up full               # or the new bare stack; add -d to detach
# API on http://localhost:8080
just image down full
```

The container serves **plain HTTP on 8080** - no TLS, no dev certificate. Set `AllowAutoRedirect = false` on
the client so a misconfigured HTTPS redirect shows up as a 307 rather than as a connection error.

The seeded admin is `admin@binacle.net` / `B1n4cl3Adm!n`, written by a startup task on first boot; the token
endpoint is `POST /api/auth/token`. Getting a token proves the whole storage path in one request - the task
wrote the account and the endpoint read it back - which is why it is worth having even though it looks like an
auth test.

## Verified against `binacle-net:local` on 2026-07-29

Each was read out of the built image. Re-check any that matter before you build on them; the point of listing
them is that you should not have to *discover* them.

- **`Config_Files/DiagnosticsModule/HealthChecks.json` ships `"Enabled": false`.** `/_health` exists only
  because a stack sets `HealthChecks__Enabled=True`. It is not a free readiness probe for a bare `docker run`,
  and "the flag still switches it on" is itself a check.
- **No `*.Development.json` in the image** - confirmed empty. The last line of `.dockerignore` is doing real
  work. If it ever stops, development overrides silently win in production.
- **`Config_Files/Presets.json` starts `EF BB BF`** - a UTF-8 BOM. A parser that chokes returns a preset list
  that looks fine with no bins in it, so read a preset *through to its bins*. The three shipped presets are
  `rectangular-cuboids`, `perfect-cubes` and `sample`, three bins each.
- **ServiceModule ships only `RateLimiter.json`** - no `JwtAuth.json`. The module cannot boot on the image
  alone; the compose files inject it via `configs:`, which is what users must also do.
- **`Config_Files/UiModule/ConnectionStrings.json` is in the image.** "The UI module works" means the page
  renders *and* that connection string resolves from inside the container network.
- **`RestrictedIPs: []` means `RestrictsNobody`**, so the health endpoint is readable from the host without
  special configuration. Behind a proxy it would not be - that is the forwarded-headers path, not this suite's.
- **The version is already on the health response.** `SystemHealthCheck` puts `{"Version", Metadata.Version}`
  into the check data, so the image stamp can be asserted without the UI module being on.

## What it must not do

Scope creep is how this becomes a slow second integration suite.

- **No project reference to the app.** Talk HTTP only, read responses as JSON documents rather than the app's
  contract types. A `ProjectReference` is how a smoke suite quietly turns into an integration suite.
- **No coverage collection** - the code under test runs in another process.
- **No shared fixtures or scenario data.** Payloads stay inline and tiny.
- **No numeric assertions** from a packing run - never a coordinate or a bin count.
- **No validation or error-path cases.** Wrong-input handling is logic.
- **Keep it fast.** If it takes longer than the image takes to build, it is doing too much.

## Inventory - a starting point, not a work order

**Challenge this too.** It is what the design above implies, written out so there is something concrete to
reject.

**The test project**

- [ ] `api/test/Binacle.Net.SmokeTests` - xunit.v3 on the Microsoft Testing Platform, like the other suites. No
      `ProjectReference`, no coverage package. Add it to `Binacle.Net.slnx`.
- [ ] Read the profile (`bare`/`full`), a base URL, the expected version and the admin credentials from the
      environment, so the same suite can be pointed at a staging instance.
- [ ] Expected status per surface as a **keyed table** rather than branching code - one row per surface, a
      column per profile, matching how theory data is keyed elsewhere in the repo.
- [ ] An assembly fixture holding one `HttpClient` with `AllowAutoRedirect = false`.
- [ ] A readiness wait in the fixture. `/_health` for `full`; `bare` has no health endpoint, so poll a core
      route instead. Give up after ~90s with a message that says how to start a container - it is the message
      you would hit most often.
- [ ] One file per failure group, named after the failure it catches, so a red test name says what is wrong.

**Stacks**

- [ ] `config/docker-compose.bare.yml` - one service, the image, a port, zero env. There is no bare stack
      today: `full`, `volume` and `bind` all switch the modules on. It doubles as the file a user reads to see
      the minimum.
- [ ] Add `bare` to the `image` module's name-to-file map in `config/image.just`.

**Recipes**

- [ ] `just test api-smoke <profile>` - runs the suite against something already up, the same shape as
      `api-service-integration <backend>`, with a `case` that rejects an unknown profile. **Not** in
      `just test all`: that is the set which needs nothing brought up.
- [ ] `just smoke` in the **root** justfile - build, then per profile: up, test, down. Root because it spans
      the `image` and `test` modules, the way `install` spans setup.
- [ ] **No `config/smoke.sh`.** The scripts that did this kind of thing were deleted this month and the jobs
      are already recipes.

**Docs**

- [ ] Repo-layout row in the agent docs README; a Smoke section in `config/README.md`; the leaf in the commands
      doc.

## Deliberately not doing yet

- **CI.** Local by hand first - the value is there without it, and a gate nobody trusts gets disabled. When it
  does go in, the place is the **release** workflow, not the PR gate: build with `load: true`, smoke the loaded
  image, push only if green. Note the dependency: `release-docker-image.yml` still inlines its own publish, so
  until it calls `just build publish`, the smoke path and the release path build the image two different ways
  and smoking one says little about the other.
- **A container `HEALTHCHECK` and `docker compose up --wait`.** It would move readiness out of the fixture, but
  `mcr.microsoft.com/dotnet/aspnet:10.0` ships no curl or wget, so it means adding a package to a production
  image for the tests' benefit.

## Things to watch out for

Each of these would cost an hour otherwise.

- **The `full` suite can trip the anonymous rate limit on itself** - see the boundary section above.
- **v4's fit status is its own enum** - `BinFitResultStatus` is `Unknown/Fits/DoesNotFit/EarlyExit`, not v3's
  `AllItemsFit/NotAllItemsFit/EarlyFail_*`.
- **`/scalar` answers 302 to `/scalar/`.** With redirects off, request the trailing slash.
- **`Guid.Empty` is not a usable "missing" id.** The admin routes reject it as invalid before they look it up,
  so a 404 check comes back 422. Use an arbitrary real GUID.
- **xUnit1051 fires on every HTTP call in a test method.** Centralise the calls on the fixture, passing
  `TestContext.Current.CancellationToken` in one place, rather than annotating ~20 call sites.
- **An empty connection string falls through to the next provider**, because resolution checks
  `string.IsNullOrWhiteSpace`. Useful for running the app locally on SQLite while pointing the suite at it.

## Questions to come back with answers to

Not rhetorical - these are the decisions the building session owns.

- **Are two profiles right?** Is there a flag whose off-state depends on another flag, which would break the
  argument that off-states collapse into one run?
- **Do the image-content checks live in the suite or in a recipe?** Some of them have no HTTP surface at all.
- **How much ServiceModule?** It is the only group needing a storage backend, and so the only reason the `full`
  stack needs more than one container. The seeded admin getting a token is the cheapest proof storage is
  reachable from inside the container network - is anything beyond that worth its runtime?
- **Is a third profile for the documented quickstart worth it?** The root README tells users to run with
  `SWAGGER_UI`, `UI_MODULE` and `SCALAR_UI` on and ServiceModule off. `bare` and `full` bound the space, but
  nothing proves the exact command we publish works.
