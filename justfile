# Binacle.Net task runner.
# Setup, the dev loops and the test, coverage, openapi and agents modules live here. Benchmarks, performance
# and the docker build are still config/*.sh.
# `just` with no args lists every task. Install: sudo apt install just

# List all tasks
default:
    @just --list

# Test leaves: `just test <leaf>`, everything with `just test all`, listed by `just --list test`.
mod test 'config/tests.just'

# Coverage on top of those leaves: `just coverage all [cobertura|sonar]`, `just coverage report` for the HTML.
mod coverage 'config/coverage.just'

# OpenAPI documents: `just openapi generate [dir]`, `just openapi lint [dir]` to Spectral them too.
mod openapi 'config/openapi.just'

# The .agents/ manifests: `just agents all` after adding, renaming or re-describing a file there.
mod agents 'config/agents.just'

# Run one thing locally: `just serve api [profile]`, `just serve docs`, `just serve web`.
mod serve 'config/serve.just'

# Two recipes rather than an `install` module: you want all of it on a fresh clone, and the only part worth
# running on its own is the asset copy. It becomes a module when there is a third thing to install separately.

# Everything a fresh clone needs before `just serve` works
[group('dev')]
install:
    npm install
    cd docs && bundle install
    cd web && bundle install
    @just assets

# Copy assets/ into the docs and web sites - run it after changing anything under assets/
[group('dev')]
assets:
    npm run copy-assets-to-docs
    npm run copy-assets-to-web
