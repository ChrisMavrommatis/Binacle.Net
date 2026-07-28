# Binacle.Net task runner.
# The docs/web dev loops, the test module and the coverage module live here. Benchmarks, performance and
# build are still config/*.sh.
# `just` with no args lists every task. Install: sudo apt install just

# List all tasks
default:
    @just --list

# Test leaves: `just test <leaf>`, everything with `just test all`, listed by `just --list test`.
mod test 'config/tests.just'

# Coverage on top of those leaves: `just coverage all [cobertura|sonar]`, `just coverage report` for the HTML.
mod coverage 'config/coverage.just'

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
