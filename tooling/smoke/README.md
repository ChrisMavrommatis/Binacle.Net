# Smoke

The assertions and stacks for `just smoke`. This suite tests the **image** rather than the code: what is inside
it, and what its HTTP surface does with the modules switched on and off.

The recipes live in `tooling/smoke.just`; this folder is the data they read. Everything is run from the repo
root. The tools - `container-structure-test` and `hurl` - are not part of `just install`; see
[DEVELOPMENT.md](../../DEVELOPMENT.md).

## 🎯 Why this exists

Every other suite in the repo runs **in process**. `Binacle.Net.IntegrationTests` boots the app with
`WebApplicationFactory` and replaces the presets with test-only ones, so it never loads the config files we
ship, and nothing anywhere touches the image.

That leaves a class of failure invisible until someone pulls the tag: a config file that did not get copied or
landed at the wrong path; a module env var that no longer switches anything on; a connection string that works
on the host and not in the container; the version build arg never reaching the process; a wrong entry point,
port or runtime. All packaging and wiring, none of it C# logic, which is why the existing suites cannot see it.

## 📂 What is in here

| File | Read by | What it is |
|---|---|---|
| `structure.yaml` | `container-structure-test` | The image's static content - shipped files, absent files, permissions, metadata, OCI labels. 31 assertions |
| `<profile>.hurl` | `hurl` | The HTTP surface for one profile, run against a running stack |
| `<profile>.yml` | `docker compose` | The stack for one profile - the image plus the env that defines it |

`structure.yaml` reads the image directly and never starts a container, so it is checked **once** per run rather
than once per profile - the same assertions behind five different stacks answer the same question five times.
It keeps the `.yaml` extension against the `.yml` stacks on purpose: it is the one file here docker never reads.

## 📋 The five profiles

Real configurations, from nothing switched on to everything. They are declared in one place - the `profiles`
variable at the top of `tooling/smoke.just` - which is what both the `all` loop and the unknown-name check read.
Adding a profile is that line plus a `.hurl` and a `.yml` named after it.

**Each profile name is also a sample folder name** under `samples/docker/`. Same configuration, different
files: a sample pins a published image and is written to be copied, while a profile runs the image under test
and carries tweaks a user must never inherit. The names are the contract between them.

| Profile | Modules on | Backend | The claim it makes |
|---|---|---|---|
| `minimal` | none, as shipped | none | The image boots from nothing and a core route answers. One 200 proves the entry point ran, the runtime works, the port is right and the non-optional config files were found |
| `quickstart` | swagger, scalar, ui | none | The published `docker run` in the root README works. Docs and the web UI serve; with service, health and debug off those surfaces are absent |
| `prod` | health, packing logs | **none** | A self-hosted deployment behind your own backend. **`/_health` is 200 with an empty `Features` array**, and it reads back a preset that exists only in a mounted file |
| `service` | swagger, scalar, service, health, packing logs | SQLite | Binacle.Net offered to other people: accounts, JWT auth, rate limiting. **`/_debug` is 404** while auth works and the admin route is 401 |
| `full` | all of the above plus ui and debug | SQLite | The dev/demo shape, where `/_debug` is deliberately 200 and the UI serves |

`minimal`, `quickstart` and `prod` are single containers. `service` and `full` inject a `JwtAuth.json` via
compose `configs:` - ServiceModule validates it at startup and will not boot without one - and use SQLite in a
named volume that is dropped on teardown.

Two profiles were the original design, on the argument that off-states do not interact. That argument is wrong
in two verified places: `/openapi/{documentName}.json` is mounted when **either** swagger or scalar is on, so
two profiles can never tell which flag did it; and the UI module's status-code-pages middleware changes the
*body* of a `/_health` or `/_debug` 404, so the off-state of one surface is shaped by another module.

### 🤔 Why `prod` and `service` are separate

They are two different products. **`prod`** is the API behind your own backend - you call it from your server,
so it needs no accounts, no auth and no database. **`service`** is Binacle.Net offered to other people, which is
what ServiceModule exists for. Most deployments are the first; the hosted binacle.net is the second.

Smoke tells them apart on one clean line: in `prod` the auth route is **404, never mounted**; in `service` it
returns a token. No ambiguity, and it fails in both directions.

### ⚙️ `prod` mounts its own presets, on purpose

`prod.yml` mounts `prod-presets.json` over `/app/Config_Files/Presets.json`, and `prod.hurl` reads back a
preset named `smoke-lockers` that appears in no shipped file. That is the strongest positive in the suite: it
proves the right image started **and** that it read configuration we supplied, which a default preset could
never show. It also mirrors the first thing any integrator does - replace the bin set with their own.

### 🔐 `service` carries the tightest assertions

It is `full` minus the two things you never expose when other people can reach you - the web UI and the debug
endpoint. `/_debug` echoes the caller's own request including their `Authorization` header. The API docs stay
on because they are documentation, not a debug surface.

Its `/_health` reading is what anchors the rest: `Features` positively states that swagger, scalar and service
are on **and** that ui and debug are off, so the 404s beside it cannot pass for the wrong reason. The admin
route in one request separates three states - 404 never mounted, 200 open to the world, and only 401 meaning
mounted **and** protected.

## 📏 Two rules, if you add a check

**Assert what the image contains and wires. Never assert what the algorithm computed.** The integration suites
own "is the answer right", in process, where a failure points at a line of C#. Re-running those assertions over
HTTP buys the same coverage with worse diagnostics, ten times the runtime, and a suite that goes red on every
legitimate packing change. The dividing line is ownership, not subject: "does rate limiting return 429" is
behaviour and belongs to integration; "is `RateLimiter.json` in the image" is packaging and belongs here.

The one known-good value that **is** right here is data that comes from the image's config files rather than
from the image's own config files. `minimal` reads a preset through to its bins by name and count, never their
dimensions - that stays true when an algorithm changes and breaks when a file does not get copied. `prod` goes
one better and reads a preset from a file it mounted itself.

**Every check must be able to fail, and must not be able to pass for a reason unrelated to what it claims.** A
profile whose assertions are all 404s satisfies the first rule and is still worthless - a wrong image, a
container that read no config, and a typo'd feature flag all pass it. That is why the profiles carrying 404s
pair them with positive 200s that prove the right image read its config.

It also means a **negative** assertion has to be falsified when you touch it. `not contains "DebugEndpoint"` is
`service`'s security check, and a predicate that quietly stops matching passes while asserting nothing. Point it
at a value that must fail, confirm it goes red, then put it back.

## ⚠️ Gotchas

These are not assertions - they are what makes the setup correct. Miss one and you get a green that means
nothing, or a red that reads as a flake.

- **Feature flags are exact-match `True`/`False`, case-sensitive.** A lowercase `SWAGGER_UI=true` is silently
  ignored and 404s - the same 404 a broken image gives. Config keys (`HealthChecks__Enabled`) are
  case-insensitive; feature flags are not.
- **Redirects are off** (hurl's default), so request the real pages: `/swagger/` 301s to `index.html`, and
  `/scalar` 302s to `/scalar/`. A stray HTTPS redirect surfaces as a 307 rather than a connection error.
- **`prod` and `full` raise `RateLimiter__ApiUsageAnonymous`.** The shipped anonymous limit is 60 requests an
  hour in a bucket that decays, so two runs ten minutes apart would go red on 429s. Presence of the setting is
  packaging; the number is behaviour, and behaviour belongs to the integration suite.
- **Use a real GUID for the admin 401 check.** `Guid.Empty` is rejected as invalid (422) before the lookup ever
  happens, so it would pass for the wrong reason.
- **The token request doubles as a storage-path check.** Startup tasks run to completion before the port opens,
  so once anything answers, the seeded admin exists and its tables do too. Getting a token proves the task wrote
  the account and the endpoint read it back, in one request.
- **The container serves plain HTTP on 8080.** No TLS.

## 🚫 Not the samples

These stacks look like `samples/docker/*` and must never become them. They run the image under test rather than
a published tag, and they carry test-only tweaks - the raised rate limit, disposable storage, an inline
throwaway JWT secret - that a sample a user copies must never have. The samples pin a published image and are
the thing users start from.

**The names are the contract.** A profile and a sample folder with the same name are the same configuration:
`samples/docker/prod` is `tooling/smoke/prod.{yml,hurl}`. Change one and change the other, or the name is lying.
A sample the suite never runs is untested advice, which is the same argument that produced this suite one level
out.

Backend is the exception, and deliberately so. Smoke is SQLite-only because the backend axis already belongs to
the integration suite - `just test api-service-integration Postgres` proves that side. Matching the module set
is what these names promise; matching the database is not.
