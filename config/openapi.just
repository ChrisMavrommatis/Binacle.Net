# The OpenAPI documents, loaded by the root justfile as the `openapi` module.
#
#   just openapi generate          write build/openapi/Binacle.Net_v3.json and _v4.json
#   just openapi generate <dir>    write them somewhere else
#   just openapi lint [<dir>]      generate, then lint them against .spectral.yaml
#
# The documents come out of the build, not out of a running server: Microsoft.Extensions.ApiDescription.Server
# starts the app host itself and dumps every registered document. So nothing has to be brought up first, and the
# host it starts has no launch profile - ServiceModule is off, which is the shape the committed specs assume
# (no /api/auth/token path). Generation is off by default; the two knobs and why they look like that are in
# api/src/Binacle.Net/Binacle.Net.csproj.

# Module recipes run where the module file lives (config/), which would break every path below.
set working-directory := '..'

# dotnet and spectral both print why they failed, so just's own "Recipe failed" line adds nothing. The exit
# code still propagates, so CI still fails.
set no-exit-message := true

# Absolute on purpose: MSBuild resolves a relative OpenApiDir against the *project* directory, so a plain
# `build/openapi` would land in api/src/Binacle.Net/. justfile_directory() is the root justfile's directory,
# not this file's, so it is the repo root.
openapi_dir := justfile_directory() / "build" / "openapi"

# List the recipes
default:
    @just --list openapi

# Write the v3 and v4 documents [dir]
generate dir=openapi_dir:
    dotnet build api/src/Binacle.Net -p:GenerateOpenApi=true -p:OpenApiDir='{{ dir }}'

# Generate the documents, then lint them against .spectral.yaml [dir]
lint dir=openapi_dir: (generate dir)
    npx --yes @stoplight/spectral-cli lint '{{ dir }}'/*
