# Smoke test a built image: its static content and its HTTP surface. Loaded as the `smoke` module.
#
#   just smoke all                     build binacle-net:local, check its structure, then every profile
#   just smoke test-structure [image]  the image's static content - no container, no stack
#   just smoke test <profile> [image]  one profile end to end: up -> hurl -> down
#   just smoke up <profile> [image]    bring one up and leave it   [minimal|quickstart|prod|service|full]
#   just smoke down <profile>          stop it (add -v to drop the volume)
#
# Every recipe takes the image last and defaults to binacle-net:local, so the same suite runs against a local
# build or a published tag - `just smoke test service binacle/binacle-net:3.0.0-beta.1` smokes what is actually on
# Docker Hub. The stacks read it as $BINACLE_IMAGE with the same default, so a bare `docker compose` still works.
#
# Static content is one container-structure-test file (tooling/smoke/structure.yaml) read straight from the
# image; the HTTP surface is one .hurl per profile run against a running stack. The structure check reads the
# image rather than a container, so it runs once per `all` and not once per profile - the same 31 assertions
# repeated behind five different stacks answer the same question five times.
#
# The stacks are separate files from samples/ on purpose: they raise the anonymous rate limit and throw their
# storage away, which the samples must never do. The Smoke section of tooling/README.md has the two rules that
# decide what belongs in this suite - read them before adding a check.

# Module recipes run where the module file lives (tooling/), which would break every path below.
set working-directory := '..'

# docker, container-structure-test and hurl all print why they failed; just's own line adds nothing.
set no-exit-message := true

# The tool binaries. Install them per DEVELOPMENT.md - they are not part of `just install`.
cst := "container-structure-test"
hurl := "hurl"

# What `just build image` tags, and so what every recipe here smokes unless told otherwise.
local_image := "binacle-net:local"

# The profiles, declared once. just has no list type, so this is a space-separated string - which is what both
# `all`'s loop and `_check`'s match want anyway. Adding a profile is this line plus the two files under
# tooling/smoke/ named after it.
profiles := "minimal quickstart prod service full"

# List the recipes
default:
    @just --list smoke

# The image's static content: the shipped files, their permissions, the metadata and the OCI labels
test-structure image=local_image:
    {{ cst }} test --image {{ image }} --config tooling/smoke/structure.yaml

# One profile end to end: up -> hurl -> down [minimal|quickstart|prod|service|full]
test profile image=local_image:
    #!/usr/bin/env bash
    set -euo pipefail
    just smoke::_check "{{ profile }}"
    # Tears the stack down even when the hurl run fails under set -e. The teardown is not silenced: a failed
    # `down` is how a container leaks, which is the one thing this trap exists to prevent.
    trap 'just smoke::_compose "{{ profile }}" "{{ image }}" down -v || true' EXIT
    just smoke::_compose "{{ profile }}" "{{ image }}" up -d
    just smoke::_test_profile "{{ profile }}"

# The HTTP surface against an already-up stack. Takes no image - it talks to whatever is listening, and which
# image that came from was decided by `up`. Private because `test` owns the lifecycle, but still reachable as
# `just smoke::_test_profile <profile>`, which is how you re-run a .hurl you are editing without paying for an
# up and a down each time.
[private]
_test_profile profile:
    {{ hurl }} --test --retry 30 --retry-interval 1000 tooling/smoke/{{ profile }}.hurl

# Bring a profile up and leave it running [minimal|quickstart|prod|service|full]
up profile image=local_image:
    @just smoke::_compose "{{ profile }}" "{{ image }}" up -d

# Stop a profile [minimal|quickstart|prod|service|full]; add -v to drop the volume
down profile *args:
    @just smoke::_compose "{{ profile }}" "{{ local_image }}" down {{ args }}

# Build the image, check its structure once, then every profile end to end
all image=local_image:
    #!/usr/bin/env bash
    set -euo pipefail
    # Only build when smoking the local tag. Given a published one there is nothing to build, and building
    # would tag binacle-net:local while the stacks went on pulling the image that was asked for - a green run
    # that says nothing about either.
    if [ "{{ image }}" = "{{ local_image }}" ]; then
        just build image
    else
        echo "smoking {{ image }} - not building"
        docker pull "{{ image }}"
    fi
    echo "===== smoke: image structure ====="
    just smoke test-structure "{{ image }}"
    for profile in {{ profiles }}; do
        echo "===== smoke: $profile ====="
        just smoke test "$profile" "{{ image }}"
    done
    echo "all profiles green: {{ image }}"

# Reject an unknown profile in one place. $want is quoted inside the pattern so it stays a literal - unquoted it
# would be read as a glob, and a profile named `*` would match anything.
[private]
_check profile:
    #!/usr/bin/env bash
    want="{{ profile }}"
    case " {{ profiles }} " in
        *" $want "*) ;;
        *) echo "Unknown profile '$want'. Use one of: {{ profiles }}" >&2; exit 1 ;;
    esac

# The profile name maps to a compose file here and nowhere else, so up and down cannot disagree about which
# file a name means. The image reaches the stack as $BINACLE_IMAGE, which every smoke compose file reads with
# the same binacle-net:local default - so a bare `docker compose -f tooling/smoke/<profile>.yml` still works.
[private]
_compose profile image *args:
    #!/usr/bin/env bash
    set -euo pipefail
    just smoke::_check "{{ profile }}"
    BINACLE_IMAGE="{{ image }}" docker compose -f "tooling/smoke/{{ profile }}.yml" {{ args }}
