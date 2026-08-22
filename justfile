# Binacle.Net task runner.
# Setup, the dev loops and the test, coverage, openapi, agents and build modules live here. Benchmarks,
# performance and the tmux session are still shell scripts.
# `just` with no args lists every task. Install: sudo apt install just

# List all tasks
default:
    @just --list

# Test leaves: `just test <leaf>`, everything with `just test all`, listed by `just --list test`.
mod test 'tooling/tests.just'

# Coverage on top of those leaves: `just coverage all [cobertura|sonar]`, `just coverage report` for the HTML.
mod coverage 'tooling/coverage.just'

# OpenAPI documents: `just openapi generate [dir]`, `just openapi lint [dir]` to Spectral them too.
mod openapi 'tooling/openapi.just'

# The .agents/ manifests: `just agents all` after adding, renaming or re-describing a file there.
mod agents 'tooling/agents.just'

# The committed generated data: `just regen all`, `just regen check` to prove it is in step.
mod regen 'tooling/regen.just'

# CHANGELOG.md sections: `just changelog extract <version|Unreleased>`, `just changelog check <version>`.
mod changelog 'tooling/changelog.just'

# Run from source: `just serve api [profile]`, `just serve docs`, `just serve demo`, `just serve services-up`.
mod serve 'tooling/serve.just'

# Make the API image: `just build publish` for the app, `just build image [version]` for the container.
mod build 'tooling/build.just'

# Run that image: `just image up [full|volume|bind]`, `just image down [name]`.
mod image 'tooling/image.just'

# Smoke the built image: `just smoke all`, `just smoke test-structure`, `just smoke test <profile>`.
mod smoke 'tooling/smoke.just'

# Check what was built: `just check links` for both sites, `just check links <site>` for one.
mod check 'tooling/check.just'

# Two recipes rather than an `install` module: you want all of it on a fresh clone, and the only part worth
# running on its own is the asset copy. It becomes a module when there is a third thing to install separately.

# Everything a fresh clone needs before `just serve` works
[group('dev')]
install:
    npm install
    cd sites/docs && bundle install
    cd sites/demo && bundle install
    @just assets

# Copy assets/ into the docs and demo sites and the UI module - run it after changing anything under assets/
[group('dev')]
assets:
    npm run copy-assets-to-docs
    npm run copy-assets-to-demo
    npm run copy-assets-to-uimodule
