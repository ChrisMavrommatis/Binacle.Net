---
description: Harden and slim the base image
---

# Harden and slim the base image

**Status:** The finding this file was opened for is **fixed**. The image was carrying two .NET runtimes; the
publish dropped `--self-contained` on 2026-08-10 and went from **150.2 MB to 103.2 MB**, promoted to CI, and
beta 2 is the first image built that way. All suites were green - 31 structure assertions, five smoke
profiles, eleven test leaves. The release plan tracks that part.

**What is left is what the title now says.** With the app layer down to 18 MB, the base image *is* the image -
so this file is no longer about a duplicated runtime, it is about the base itself. Every size below was
measured on 2026-08-10, not estimated. Not in v3.0.0.

## Where the 103.2 MB sits now

From `docker history` on a locally built image:

| Layer | Size |
|---|---|
| base - `mcr.microsoft.com/dotnet/aspnet:10.0` | **93.7 MB** |
| the app - `COPY artifacts/binacle-net` | 18.4 MB |
| `libgssapi-krb5-2` via apt | 2.5 MB |
| everything else (workdir, `/app/data`, user) | negligible |

**The base is roughly 90% of the image.** That is the whole reason this plan still exists.

## The options, measured

Base image sizes, pulled and inspected:

| Base | Size |
|---|---|
| `aspnet:10.0` (current) | 93.7 MB |
| `aspnet:10.0-noble-chiseled-extra` | 67.5 MB |
| `aspnet:10.0-noble-chiseled` | 52.6 MB |
| `runtime-deps:10.0` | 46.7 MB |
| `runtime-deps:10.0-noble-chiseled` | **5.5 MB** |

Combined with the two measured app-layer sizes - **18 MB framework-dependent, 123 MB self-contained**:

| Combination | Approx image | Notes |
|---|---|---|
| framework-dependent + `aspnet` **(today)** | **103.2 MB** | measured |
| framework-dependent + `aspnet` chiseled-extra | ~88 MB | keeps ICU |
| framework-dependent + `aspnet` chiseled | ~73 MB | **smallest realistic** - needs invariant globalization |
| self-contained + `runtime-deps` chiseled | ~130 MB | |
| self-contained + `runtime-deps` | ~170 MB | worse than today |

## Going back to self-contained, for DHI or anything else, costs size {#self-contained-revisited}

The question came up as "or going DHI with self-contained". The measurement answers it:

**The self-contained publish is 123 MB. The entire `aspnet` base is 93.7 MB.** The bundled runtime is bigger
than the whole base it would replace, so no base saving can win it back - even the 5.5 MB chiseled
`runtime-deps` lands around 130 MB, well above today's 103.2 MB.

So self-contained is **not** a route to a smaller image. It is only a route to a *generic* base, because a
self-contained app needs no .NET in the base at all. That matters for DHI specifically, since a generic minimal
hardened base is easier to find than a .NET-specific one - but it costs about 27 MB over today, and 57 MB over
framework-dependent chiseled, to buy that freedom.

**It would only pay off with trimming or Native AOT**, which is what makes that 5.5 MB base interesting. This
app is a poor trimming candidate: the UI module is Blazor, and Azure SDK, Npgsql and OpenTelemetry are all
reflection-heavy. Trimming here is a project with a real risk of runtime failures the tests may not catch, not
a flag. **Do not treat it as a quick follow-up.**

It also gives back the patching property that motivated dropping self-contained in the first place: with the
runtime inside the artifact, a .NET security fix means republishing the app rather than rebasing the image.

## The recommendation

**Framework-dependent on a chiseled `aspnet` tag.** Free, no new vendor, keeps rebase-to-patch, and it is the
smallest of the realistic options. Chiseled also brings the part that matters more than megabytes: no shell, no
package manager, non-root by default, and a fraction of the package surface for a scanner to find CVEs in.

Be honest about the size prize, though - the big win is already banked. This is **15 to 30 MB** depending on
the ICU decision. **Do it for the attack surface and take the size as a bonus.**

### Two blockers, and the first decides which tag

- **ICU.** Plain chiseled has no ICU, so it needs `InvariantGlobalization=true` - which changes string
  comparison and culture behaviour app-wide. `chiseled-extra` carries ICU and costs about 15 MB. **Take
  `-extra` unless someone has checked that invariant globalization is safe here**; a wrong answer shows up as
  subtly different sorting or parsing, not as a crash.
- **`libgssapi-krb5-2` cannot be apt-installed** - chiseled has no package manager. Options: copy the shared
  object in from a builder stage, or **drop it**. Worth remembering what it buys: Npgsql prints
  `Cannot load library libgssapi_krb5.so.2` at startup, which is harmless and only *looks* fatal. A library was
  shipped to silence a cosmetic log line. Weigh that against the hardening rather than assuming it is required.

### What to verify

The smoke suite is the safety net and it is already proven against this image shape - 31 structure assertions
plus five profiles. Expect some structure assertions to need updating: they will reference paths, or a shell,
that chiseled does not have. That is the suite doing its job, not a problem with the base.

Also confirm `APP_UID` still resolves and `/app/data` is still writable by it. That chown is what made mounted
volumes work at all, and it is the kind of thing a base change breaks quietly.

## Docker Hardened Images

**Check the catalog before planning around it.** DHI is a paid Docker subscription add-on - minimal,
near-zero-CVE images with signed SBOM and VEX attestations. **Whether it carries an ASP.NET runtime image is
not verified here** and should be confirmed first rather than assumed.

That confirmation matters more than it did before: framework-dependent needs a base *with* the ASP.NET runtime,
so a generic minimal DHI base is not enough on its own. The alternatives are a .NET DHI if one exists, or going
back to self-contained and paying the 27 MB described above.

**What it adds over chiseled** is mostly supply-chain paperwork - maintained near-zero-CVE bases, VEX
statements letting a scanner discount non-exploitable findings, and signed attestations. **What it costs** is a
subscription and a vendor dependency for an open-source project.

**Opinion: chiseled first, and probably chiseled only.** The gap between `aspnet` and chiseled is real; the gap
between chiseled and DHI is mostly attestation metadata, and `sbom: true` plus `provenance: mode=max` in the
release workflow supplies part of that story for free - the CI release plan owns that change.

Revisit DHI if a consumer actually asks for VEX or signed attestations, or if the subscription already exists
for another reason.

## Sequence

1. Decide the ICU question - it picks the tag.
2. Decide the krb5 question - copy the `.so` in, or drop it and accept the log line.
3. Switch the base, run the full smoke suite, update whatever structure assertions it catches.
4. Only then look at DHI, and only after confirming the catalog has something that fits.

## Done when

The base is a deliberate choice with a written reason, and the image ships no shell and no package manager -
or there is a recorded decision saying why it still does.
