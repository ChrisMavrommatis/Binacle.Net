---
description: "Integration tests: cover what the harness cannot see today"
paths:
  - "api/**"
---

# Integration tests: cover what the harness cannot see today

**Status:** Designed, nothing built. Split out on 2026-07-29 from the smoke-testing work, because the two are
different jobs: this one is **behaviour, in process**; the other is **packaging, in a container**.

## This is two sessions, and phase 1 stops before it writes anything

Added 2026-08-07. The file used to say "one session each" and it was wrong about this one - it is two, and
running them as one is how it produces the mistake it warns about below.

**Phase 1 - investigate, read-only.** Answer the four questions at the bottom of this file. Write the answers
back into this plan, replacing the guesses with what you found. **Then stop.** Do not write a test, do not
touch a harness, do not delete a TODO. The maintainer decides the shape from your answers.

**Phase 2 - build.** A later session writes the tests against a shape that is already settled, and deletes the
`Challenge this` markers as it goes.

The gate is here because the two big questions depend on each other. "One run with everything on, or a matrix?"
cannot be answered until "how many more cross-module dependencies are there?" is answered, and that second one
is real research. A session doing both will settle the shape early so it can get on with writing tests, which
is exactly the failure this plan exists to avoid.

## Read this first: this file is a seed, not a spec

It was written in a session that found the gaps but wrote none of the tests. It is deliberately opinionated so
there is something to argue with.

**Your first job is to disagree with it.** Investigate, then come back with what you think should be built.
Sections marked **Challenge this** are where a wrong call is most likely and most expensive. "The matrix is
wrong, here is why" is a better outcome than a matrix built because a file said so.

## The gap

The integration harnesses boot the app with `WebApplicationFactory` and **core modules only**. Three `// TODO`
comments say so:

- `api/test/Binacle.Net.IntegrationTests/BinacleApi.cs:35`
- `api/test/Binacle.Net.IntegrationTests/BinacleApiWithoutPresets.cs:33`
- `api/test/Binacle.Net.ServiceModule.IntegrationTests/BinacleApi.cs:44`

In the core harness the pre-build configuration dictionary is literally empty, so Diagnostics, Service and UI
are all off. Every module combination the image actually ships is untested end to end.

## What that hides - verified 2026-07-29

**1. Rate limiting was exercised nowhere. That one is built** -
`api/test/Binacle.Net.ServiceModule.IntegrationTests/RateLimiting/` covers both limiters and the login
throttle's partition, with a host per test. It is the worked example of the shape this plan is about: **a core
behaviour that only exists because an optional module registered something.** Read it before deciding anything
below; it answers in code what this plan asks in prose.

**2. CORS is exercised nowhere.**

`Program.cs` always registers the `CoreApi` policy and every core endpoint carries
`.RequireCors(CorsPolicy.CoreApi)`. The origins come from an optional `Cors.json`; with none present,
`AllowedOrigins` falls back to an empty array - a closed default the validator's own comment says is intended.
Nothing asserts that a configured origin is echoed back, or that an unconfigured one is not.

**3. The shipped presets are replaced.** Both core harnesses swap in three test-only presets, so no in-process
test ever reads `Config_Files/Presets.json`. **Leave that alone** - proving the shipped presets load is the
container suite's job, and it is the one place the two plans must not both act.

## The boundary with the smoke suite

Keep this line and neither session will duplicate the other:

> **Behaviour goes here, in process. Packaging goes to the container suite.**

"Does rate limiting return 429 when the module is on" is behaviour - test it here, where it is fast and a
failure points at a line of C#. "Is `RateLimiter.json` in the image at all" is packaging. "Does CORS echo a
configured origin" is behaviour. "The shipped image has no `Cors.json`, so it allows no browser origin" is
packaging.

## What to build - phase 2, after the gate above

**Challenge this.** It is what the gap implies, written out so there is something concrete to reject. Phase 1
does not build any of it; it decides whether this list is right.

- [ ] Turn the modules on in the harnesses - Diagnostics, Service, UI - and delete the three TODOs.
- [ ] **Decide: one run with everything on, or a small matrix over the combinations that actually ship.**
      Everything-on is cheaper and catches registration conflicts; a matrix catches "module A only works
      because module B registered something", which is exactly the failure this plan exists for. The rate
      limiting case argues for at least two configurations. Runtime is the budget - the integration suite is
      already the long pole.
- [ ] CORS: assert a configured origin comes back in `Access-Control-Allow-Origin` and an unconfigured one does
      not.
- [ ] Look for more of the same shape before deciding the matrix. The search is core code that only works
      because an optional module registered something - `RateLimited`, `RequireCors`, `RequireAuthorization`
      and any middleware a module adds to the shared pipeline are the places to start.

## Things to watch out for

- **A live rate limiter will make other tests flaky.** The anonymous partition key is a constant, so it is one
  bucket for every anonymous caller in a host, and it does not refill inside a run. **Do not turn the limiter on
  in a shared harness.** The rate limiting tests build their own host per test for this reason.
- **Test-host configuration goes through an env var the harness reads, never a `.runsettings` file** - the
  Microsoft Testing Platform runner ignores VSTest runsettings. `BINACLE_TEST_INFRA` already works this way.
- **Turning UI and Service on changes the route table**, so a test asserting a 404 for an unknown path may
  start hitting a real endpoint. Expect some existing assertions to need adjusting, and treat each one as a
  question rather than a fix - a 404 that becomes a 200 might be the bug this work exists to find.
- **The ServiceModule harness disables the auth-token limiter on purpose.** If you turn it back on, the auth
  tests need to account for it.

## Questions to come back with answers to - this is phase 1's whole job

- **One run or a matrix?** And if a matrix, which combinations - the ones the samples ship, or something
  smaller?
- **How many more cross-module dependencies are there?** That list is what decides the answer above.
- **Does anything break when the modules go on?** If existing assertions have to change, say which and why -
  that is a finding, not a chore.

## Related work, not to be done here

The CI side - running these on every PR - is tracked with the other PR gates. This plan is about writing the
tests; a gate that runs them is a separate change and should not hold this up.
