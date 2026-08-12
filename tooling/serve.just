# Run something locally from source, loaded by the root justfile as the `serve` module.
#
#   just serve api [profile]     the API, via its launch profiles
#   just serve docs              the documentation site
#   just serve web               the marketing site
#   just serve services [-d]     what the API talks to: aspire dashboard, azurite, postgres
#   just serve services-down     stop them (only needed after -d; Ctrl-C is enough otherwise)
#
# One module because it is all the same job: work on the code on this machine. `services` is here rather than
# with the image stacks for that reason - it runs no binacle-net at all, it brings up what `just serve api`
# talks to, and the Postgres and AzureStorage test leaves want the same thing.
#
# Running the *built image* is the other job and lives in the `image` module. The two do not call each other:
# the few lines of folder setup below are copied there rather than shared, because a module that reaches into
# another one puts the coupling back that splitting them removed. Nothing here is called by CI.

# Module recipes run where the module file lives (tooling/), which would break every path below.
set working-directory := '..'

# dotnet, jekyll and webpack all print why they failed, so just's own "Recipe failed" line adds nothing.
set no-exit-message := true

# List the recipes
default:
    @just --list serve

# The launch profiles are a positional argument rather than four recipes: they are one run with the modules
# switched on and off, not four different things to run. The short aliases are what gets typed daily, and the
# `case` rejects anything else - a misspelled profile would otherwise reach `dotnet run` and start the app on
# its default profile, which looks like a working run of something nobody asked for.
#
# The profiles themselves live in api/src/Binacle.Net/Properties/launchSettings.json.

# The API [Normal|WithServiceModuleOnly|WithUiModuleOnly|WithAllModules] (N|S|U|All)
api profile="Normal":
    #!/usr/bin/env bash
    set -euo pipefail
    case "{{ profile }}" in
        Normal | N)                launch_profile=Normal ;;
        WithServiceModuleOnly | S) launch_profile=WithServiceModuleOnly ;;
        WithUiModuleOnly | U)      launch_profile=WithUiModuleOnly ;;
        WithAllModules | All | A)  launch_profile=WithAllModules ;;
        *)
            echo "Unknown profile '{{ profile }}'." >&2
            echo "Use Normal (N), WithServiceModuleOnly (S), WithUiModuleOnly (U) or WithAllModules (All)." >&2
            exit 1
            ;;
    esac
    echo "Running Binacle.Net with launch profile: $launch_profile"
    dotnet run --project api/src/Binacle.Net -lp "$launch_profile"

# Backing services only - no binacle-net. Extra arguments go straight to `docker compose`, so `-d` detaches.
# In the foreground Ctrl-C stops them, which is why this is one recipe and its `down` is a second one rather
# than an `up`/`down` pair like the image stacks.

# What the API talks to: aspire dashboard, azurite, postgres [-d]
services *args:
    #!/usr/bin/env bash
    set -euo pipefail

    # Azurite bind-mounts tooling/azurite for its state. Left to itself docker creates a missing bind source as
    # root, which leaves emulator state in the repo you cannot open - so make it first, and make sure the
    # container can write into it.
    #
    # The directory only, never `-R`: azurite writes its files as root and they stay root's, so a recursive
    # chmod would fail on exactly those while making nothing more writable - a password prompt on every run
    # for no gain.
    mkdir -p tooling/azurite
    if [ "$(stat -c '%a' tooling/azurite)" != "777" ]; then
        # A plain chmod covers a directory we just made. Only one docker made for us needs sudo.
        chmod 777 tooling/azurite 2>/dev/null || sudo chmod 777 tooling/azurite
    fi

    docker compose -f tooling/docker-compose.yml up {{ args }}

# Stop the backing services [-v drops the postgres volume]
services-down *args:
    docker compose -f tooling/docker-compose.yml down {{ args }}

# Docs site: jekyll serve + webpack watch, one terminal (Ctrl-C stops both)
docs: (_serve-n-watch 'docs')

# Web site: jekyll serve + webpack watch, one terminal (Ctrl-C stops both)
web: (_serve-n-watch 'web')

# Serve and watch together, each line prefixed with its source.
# --kill-others makes one Ctrl-C stop both.
[private]
_serve-n-watch dir:
    cd "{{ dir }}" && npx concurrently \
        --kill-others \
        --names 'serve,watch' \
        --prefix-colors 'magenta,cyan' \
        'npm run serve' 'npm run watch'
