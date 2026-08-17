# The OpenAPI documents, loaded by the root justfile as the `openapi` module.
#
#   just openapi generate          write artifacts/openapi/Binacle.Net_v3.json and _v4.json
#   just openapi generate <dir>    write them somewhere else
#   just openapi lint [<dir>]      generate, then lint them against tooling/openapi.spectral.yaml
#
# The documents come out of the build, not out of a running server: Microsoft.Extensions.ApiDescription.Server
# starts the app host itself and dumps every registered document. So nothing has to be brought up first, and the
# host it starts has no launch profile - ServiceModule is off, which is the shape the committed specs assume
# (no /api/auth/token path). Generation is off by default; the two knobs and why they look like that are in
# api/src/Binacle.Net/Binacle.Net.csproj.

# Module recipes run where the module file lives (tooling/), which would break every path below.
set working-directory := '..'

# dotnet and spectral both print why they failed, so just's own "Recipe failed" line adds nothing. The exit
# code still propagates, so CI still fails.
set no-exit-message := true

# Absolute on purpose: MSBuild resolves a relative OpenApiDir against the *project* directory, so a plain
# `artifacts/openapi` would land in api/src/Binacle.Net/. justfile_directory() is the root justfile's directory,
# not this file's, so it is the repo root.
openapi_dir := justfile_directory() / "artifacts" / "openapi"

# Named because the ruleset no longer sits at the repo root, where spectral finds one by itself. Absolute for
# the same reason as openapi_dir: the recipe runs from the repo root, but a caller can pass any dir.
ruleset := justfile_directory() / "tooling" / "openapi.spectral.yaml"

# List the recipes
default:
    @just --list openapi

# Write the v3 and v4 documents [dir]
generate dir=openapi_dir:
    dotnet build api/src/Binacle.Net -p:GenerateOpenApi=true -p:OpenApiDir='{{ dir }}'

# --fail-severity=warn because spectral's default exits 0 on warnings. Without it the step prints
# "No results with a severity of 'error' found!" and passes whatever the warnings say - a gate that looks
# like it works.

# Generate the documents, then lint them against tooling/openapi.spectral.yaml [dir]
lint dir=openapi_dir: (generate dir)
    npx --yes @stoplight/spectral-cli lint --ruleset '{{ ruleset }}' --fail-severity=warn '{{ dir }}'/*
