# Smoke test the built image: its static content and its HTTP surface. Loaded as the `smoke` module.
#
#   just smoke all                 build binacle-net:local, then per profile up -> test -> down
#   just smoke up <profile>        bring one profile up   [zero|quickstart|prod|full]
#   just smoke test <profile>      run the tools against an up stack
#   just smoke down <profile>      stop it (add -v to drop the volume)
#
# Static content is one container-structure-test file (config/smoke/image.yaml) run against the image; the HTTP
# surface is one .hurl per profile run against a running stack. Stacks run binacle-net:local - the image under
# test, not the published one - so they are separate from the samples. See
# .agents/plans/api/smoke-testing-the-image.md.

# Module recipes run where the module file lives (config/), which would break every path below.
set working-directory := '..'

# docker, container-structure-test and hurl all print why they failed; just's own line adds nothing.
set no-exit-message := true

# Tool binaries. container-structure-test installs under its asset name; override here if you symlink it.
cst := "container-structure-test-linux-amd64"
hurl := "hurl"

# List the recipes
default:
    @just --list smoke

# Bring a profile up [zero|quickstart|prod|full]
up profile:
    @just smoke::_compose "{{ profile }}" up -d

# Stop a profile [zero|quickstart|prod|full]; add -v to drop the volume
down profile *args:
    @just smoke::_compose "{{ profile }}" down {{ args }}

# Run the tools against an already-up profile [zero|quickstart|prod|full]
test profile:
    #!/usr/bin/env bash
    set -euo pipefail
    just smoke::_check "{{ profile }}"
    echo "== image content =="
    {{ cst }} test --image binacle-net:local --config config/smoke/image.yaml
    echo "== http surface: {{ profile }} =="
    {{ hurl }} --test --retry 30 --retry-interval 1000 config/smoke/{{ profile }}.hurl

# Build the image, then run every profile up -> test -> down. Never leaks a container: the trap tears the
# current stack down even when a test fails under set -e.
all:
    #!/usr/bin/env bash
    set -euo pipefail
    just build image
    for profile in zero quickstart prod full; do
        echo "===== smoke: $profile ====="
        trap 'just smoke::_compose "'"$profile"'" down -v >/dev/null 2>&1 || true' EXIT
        just smoke::_compose "$profile" up -d
        just smoke test "$profile"
        just smoke::_compose "$profile" down -v
        trap - EXIT
    done
    echo "all profiles green"

# Reject an unknown profile in one place.
[private]
_check profile:
    #!/usr/bin/env bash
    case "{{ profile }}" in
        zero|quickstart|prod|full) ;;
        *) echo "Unknown profile '{{ profile }}'. Use zero, quickstart, prod or full." >&2; exit 1 ;;
    esac

# The profile name maps to a compose file here and nowhere else, so up and down cannot disagree.
[private]
_compose profile *args:
    #!/usr/bin/env bash
    set -euo pipefail
    just smoke::_check "{{ profile }}"
    docker compose -f "config/smoke/{{ profile }}.yml" {{ args }}
