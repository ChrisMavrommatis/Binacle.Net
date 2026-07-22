# Binacle.Net task runner.
# Only the docs/web dev loops live here for now. The rest stays in config/*.sh until we know
# what we actually want out of `just`.
# `just` with no args lists every task. Install: sudo apt install just

# List all tasks
default:
    @just --list

# Docs dev: jekyll serve + webpack watch, one terminal (Ctrl-C stops both)
docs:
    @just _serve-n-watch docs

# Web dev: jekyll serve + webpack watch, one terminal (Ctrl-C stops both)
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
