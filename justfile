# Binacle.Net task runner.
# The docs/web dev loops and the test, coverage, openapi and agents modules live here. Running the API,
# benchmarks, performance and build are still config/*.sh.
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

# Docs dev: jekyll serve + webpack watch, one terminal (Ctrl-C stops both)
[group('dev')]
docs:
    @just _serve-n-watch docs

# Web dev: jekyll serve + webpack watch, one terminal (Ctrl-C stops both)
[group('dev')]
web:
    @just _serve-n-watch web

# Serve and watch together, each line prefixed with its source.
# --kill-others makes one Ctrl-C stop both.
[private]
_serve-n-watch dir:
    cd "{{justfile_directory()}}/{{dir}}" && npx concurrently \
        --kill-others \
        --names 'serve,watch' \
        --prefix-colors 'magenta,cyan' \
        'npm run serve' 'npm run watch'
