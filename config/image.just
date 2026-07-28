# Run the built image locally, loaded by the root justfile as the `image` module.
#
#   just image up                same as `up full`
#   just image up full           everything on: all modules, all three backends, the aspire dashboard
#   just image up volume         the image alone, SQLite, data in a named volume
#   just image up bind           the image alone, SQLite, data in a folder you can open
#   just image down [name]       stop it; add -v to drop the named volumes too
#
# These follow `just build image`, and they answer a different question from the `serve` module: not "does the
# code work" but "does the thing we ship work". Everything here runs binacle-net:local, which is why every
# stack checks for it first and `serve` has no such check.
#
# The folder setup below is copied from `serve`, not shared with it. A module that reaches into another one
# puts back the coupling that splitting them removed, and this is a few lines of mkdir and chmod.

# Module recipes run where the module file lives (config/), which would break every path below.
set working-directory := '..'

# docker prints why it failed, so just's own "Recipe failed" line adds nothing. The exit code still
# propagates.
set no-exit-message := true

# List the recipes
default:
    @just --list image

# `full` is the default because it is the one that tests the whole image - every module on, every backend
# running. `volume` and `bind` are the same single container twice, differing only in where /app/data goes.
# The name is positional, so pass it whenever you pass compose flags - `just image up -d` reads -d as the
# stack name and is rejected.

# Bring a stack up [full|volume|bind]
up name="full" *args:
    #!/usr/bin/env bash
    set -euo pipefail
    just image::_prepare "{{ name }}"
    just image::_compose "{{ name }}" up {{ args }}

# Stop a stack [full|volume|bind]
down name="full" *args:
    @just image::_compose "{{ name }}" down {{ args }}

# The stack name maps to a file here and nowhere else, so `up` and `down` cannot disagree about which file a
# name means. This is also the only place an unknown name is rejected - without it a typo falls through to a
# compose default and reports success for a stack nobody started.
[private]
_compose name *args:
    #!/usr/bin/env bash
    set -euo pipefail
    case "{{ name }}" in
        full)   file=docker-compose.build.yml ;;
        volume) file=docker-compose.volume.yml ;;
        bind)   file=docker-compose.bind.yml ;;
        *) echo "Unknown stack '{{ name }}'. Use full, volume or bind." >&2; exit 1 ;;
    esac
    docker compose -f "config/$file" {{ args }}

# Which folders a stack bind-mounts. Compose resolves a relative bind source against the compose file's own
# directory, so every path here is relative to config/ and this recipe works from there too.
#
# An unknown name does nothing and falls through to _compose, which is where it is rejected - keeping one
# reject means this case only has to answer "what does it mount", not "is it real".
[private]
_prepare name:
    #!/usr/bin/env bash
    set -euo pipefail
    cd config

    # Creates the directory if it is missing, then makes sure the container can write into it.
    #
    # The directory only, never `-R`: what the container needs is a writable mount point, and the files inside
    # belong to whoever wrote them - the app as APP_UID, azurite as root - and stay writable to that same
    # writer. A recursive chmod would fail on exactly those files while making nothing more writable, which
    # means asking for a password on every `up` for no gain.
    ensure_writable() {
        mkdir -p "$1"
        [ "$(stat -c '%a' "$1")" = "777" ] && return 0
        # A plain chmod covers a directory we just made. It cannot cover one docker made for us: a missing
        # bind source is created by the daemon, as root, and only sudo reopens that.
        chmod 777 "$1" 2>/dev/null || sudo chmod 777 "$1"
    }

    # Stops with the command that fixes it if the image is missing. Without this, compose falls back to
    # pulling binacle-net from Docker Hub and reports "pull access denied", which reads like a credentials
    # problem rather than the missing local build it is.
    require_image() {
        docker image inspect binacle-net:local >/dev/null 2>&1 && return 0
        echo "binacle-net:local not found. Build it first: just build image" >&2
        exit 1
    }

    case "{{ name }}" in
        full)
            require_image
            ensure_writable ./data/logs
            ensure_writable ./data/pack-logs
            ensure_writable ./azurite
            ;;
        volume)
            require_image
            ;;
        bind)
            require_image
            # BINACLE_DATA_DIR moves the folder; the compose file defaults it to ./data the same way.
            ensure_writable "${BINACLE_DATA_DIR:-./data}"
            ;;
    esac
