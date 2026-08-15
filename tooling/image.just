# Run the built image locally, loaded by the root justfile as the `image` module.
#
#   just image up                same as `up full`
#   just image up full           everything on: all modules, all three backends, the aspire dashboard
#   just image up volume         the image alone, SQLite, data in the named volume binacle-net-data
#   just image up bind           the image alone, SQLite, data in tooling/data, a folder you can open
#   just image down [name]       stop it; add -v to drop the named volumes too
#   just image verify <version>  check a published image: digest, tags, signature, attestations, metadata
#
# These follow `just build image`, and they answer a different question from the `serve` module: not "does the
# code work" but "does the thing we ship work". The stacks all run binacle-net:local, which is why each checks
# for it first and `serve` has no such check. **`verify` is the exception** - it reads a published image off
# Docker Hub and GHCR, builds nothing, and never logs in.
#
# Two files, three stacks: `volume` and `bind` share tooling/image.local.yml and differ only in where
# /app/data goes. `full` includes that file and serve's services, so it is the same postgres on the same
# volume and the same port - it and `just serve services-up` cannot run at once.
#
# The folder setup below is copied from `serve`, not shared with it. A module that reaches into another one
# puts back the coupling that splitting them removed, and this is a few lines of mkdir and chmod.

# Module recipes run where the module file lives (tooling/), which would break every path below.
set working-directory := '..'

# docker prints why it failed, so just's own "Recipe failed" line adds nothing. The exit code still
# propagates.
set no-exit-message := true

# Where a release lands. GHCR is where the pipeline stages and smoke-tests; Docker Hub is copied from it by
# digest, which is what makes the two comparable at all.
hub_repo := 'binacle/binacle-net'
ghcr_repo := 'ghcr.io/chrismavrommatis/binacle-net'

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

# The version has no default on purpose: a default rots into a tag nobody meant to check, and green against
# last release is worse than no output.
#
# Needs no `docker login`, and must not grow one. These are the commands a user runs, and a check that only
# passes with a credential is not checking a public artifact.
#
# Only 3.0.0-beta.2 and later can pass. Signing, the SBOM and the GHCR staging copy all start there, so older
# tags fail on digest, signature and attestations - history, not a broken check.

# Verify a published image [all|digest|tags|signature|attestations|metadata]
verify version check="all":
    #!/usr/bin/env bash
    # No `set -e`. A failed check is the interesting output, so every check runs and the codes are OR-ed at the
    # end - otherwise the first failure hides the four answers that explain it.
    set -uo pipefail
    if [ -z "{{ version }}" ]; then
        echo "A version is required: just image verify 3.0.0" >&2
        exit 1
    fi
    case "{{ check }}" in
        all) checks="digest tags signature attestations metadata" ;;
        digest|tags|signature|attestations|metadata) checks="{{ check }}" ;;
        *)
            echo "Unknown check '{{ check }}'." >&2
            echo "Use all, digest, tags, signature, attestations or metadata." >&2
            exit 1
            ;;
    esac

    echo "Verifying binacle-net {{ version }}"
    rc=0
    failed=""
    for c in $checks; do
        just image::_verify-$c "{{ version }}" || { rc=1; failed="$failed $c"; }
        echo
    done
    if [ "$rc" -eq 0 ]; then
        echo "PASS - everything checked came back clean."
    else
        echo "FAIL -$failed" >&2
    fi
    exit $rc

# The stack name maps to a file, a project and an environment here and nowhere else, so `up` and `down` cannot
# disagree about what a name means. This is also the only place an unknown name is rejected - without it a typo
# falls through to a compose default and reports success for a stack nobody started.
#
# -p is not optional: `volume` and `bind` come out of one file, so without it `up bind` recreates the `volume`
# container and reads its data as an empty database.
[private]
_compose name *args:
    #!/usr/bin/env bash
    set -euo pipefail
    case "{{ name }}" in
        full)   file=image.full.yml;  project=binacle-net-full;   unset BINACLE_DATA_DIR ;;
        volume) file=image.local.yml; project=binacle-net-volume; unset BINACLE_DATA_DIR ;;
        # ./data resolves against the compose file, so tooling/data - where _prepare chmods. Unset above, or
        # a value exported for this stack turns the other two's named volume into a bind.
        bind)   file=image.local.yml; project=binacle-net-bind
                export BINACLE_DATA_DIR="${BINACLE_DATA_DIR:-./data}" ;;
        *) echo "Unknown stack '{{ name }}'. Use full, volume or bind." >&2; exit 1 ;;
    esac
    docker compose -f "tooling/$file" -p "$project" {{ args }}

# Which folders a stack bind-mounts. Compose resolves a relative bind source against the compose file's own
# directory, so every path here is relative to tooling/ and this recipe works from there too.
#
# An unknown name does nothing and falls through to _compose, which is where it is rejected - keeping one
# reject means this case only has to answer "what does it mount", not "is it real".
[private]
_prepare name:
    #!/usr/bin/env bash
    set -euo pipefail
    cd tooling

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
            # Azurite only - /app/data is the named volume here, same as `volume`.
            ensure_writable ./azurite
            ;;
        volume)
            require_image
            ;;
        bind)
            require_image
            # _compose defaults this to ./data the same way. Two different defaults and the folder is
            # prepared here, written somewhere else.
            ensure_writable "${BINACLE_DATA_DIR:-./data}"
            ;;
    esac

# The five checks, one question each, in the order that makes sense: each answers something the next assumes.
# Every one prints what it found before it says pass or fail - a check whose only output is "ok" cannot be
# read over someone's shoulder.

# Same digest on both registries
[private]
_verify-digest version:
    #!/usr/bin/env bash
    set -uo pipefail
    echo "== digest =="
    # Four braces open, two close. Two opening braces are just's own interpolation, so a Go template needs the
    # extra pair only going in - four closing braces append a literal pair to every value and still look right.
    # No backticks in a comment down here either: just evaluates them as a command before the shell sees it.
    hub=$(docker buildx imagetools inspect "{{ hub_repo }}:{{ version }}" --format '{{{{ .Manifest.Digest }}' 2>/dev/null)
    ghcr=$(docker buildx imagetools inspect "{{ ghcr_repo }}:{{ version }}" --format '{{{{ .Manifest.Digest }}' 2>/dev/null)
    printf '  docker hub  %s\n' "${hub:-<tag not found>}"
    printf '  ghcr.io     %s\n' "${ghcr:-<tag not found>}"
    if [ -z "$hub" ] || [ -z "$ghcr" ]; then
        echo "  the tag is missing on one of the two registries" >&2
        exit 1
    fi
    # `publish` copies by digest instead of rebuilding, and a copy preserves the hash. Equal digests are what
    # says Docker Hub is serving the artifact the smoke job passed; unequal means nothing below matters.
    if [ "$hub" != "$ghcr" ]; then
        echo "  MISMATCH - docker hub is not serving what ghcr smoke-tested" >&2
        exit 1
    fi
    echo "  match"

# Which docker hub tags are this same image
[private]
_verify-tags version:
    #!/usr/bin/env bash
    set -uo pipefail
    echo "== docker hub tags =="
    json=$(curl -fsSL 'https://hub.docker.com/v2/repositories/{{ hub_repo }}/tags?page_size=100') || {
        echo "  could not read the tag list" >&2
        exit 1
    }
    digest=$(echo "$json" | jq -r --arg v '{{ version }}' '.results[] | select(.name==$v) | .digest')
    if [ -z "$digest" ] || [ "$digest" = "null" ]; then
        echo "  no '{{ version }}' tag on docker hub" >&2
        exit 1
    fi
    # Rows sharing a digest are one image under several names - that is how you see what `latest` resolves to.
    # The date is the trap: it moves for reasons that are not a retag, so it is printed but never compared.
    echo "$json" | jq -r --arg d "$digest" '
        .results[] | select(.digest==$d)
        | "  \(.name)   \(.digest[7:19])   \(.full_size)   \(.last_updated[0:10])"'
    echo "  every row above is the same image under another name"

# Signed by this repo's release workflow, on both registries
[private]
_verify-signature version:
    #!/usr/bin/env bash
    set -uo pipefail
    echo "== signature =="
    if ! command -v cosign >/dev/null 2>&1; then
        echo "  cosign is not installed - DEVELOPMENT.md has the pinned install" >&2
        exit 1
    fi
    # Both registries, always. A signature is a referrer stored beside the image rather than inside the index,
    # so it does not survive `imagetools create` and the pipeline signs twice. One says nothing about the other.
    rc=0
    for ref in "{{ hub_repo }}:{{ version }}" "{{ ghcr_repo }}:{{ version }}"; do
        # Never drop either flag. Without the identity this only asks "did anyone sign this" - and anyone can,
        # Sigstore is open to every GitHub account alive.
        if out=$(cosign verify "$ref" \
            --certificate-identity-regexp '^https://github\.com/ChrisMavrommatis/Binacle\.Net/\.github/workflows/release-docker-image\.yml@' \
            --certificate-oidc-issuer https://token.actions.githubusercontent.com 2>&1); then
            printf '  %-52s signed by the release workflow\n' "$ref"
        else
            printf '  %-52s NOT VERIFIED\n' "$ref" >&2
            echo "$out" | sed 's/^/      /' >&2
            rc=1
        fi
    done
    exit $rc

# What is attached: SPDX SBOM and SLSA provenance
[private]
_verify-attestations version:
    #!/usr/bin/env bash
    set -uo pipefail
    echo "== attestations =="
    ref="{{ hub_repo }}:{{ version }}"
    sbom=$(docker buildx imagetools inspect "$ref" --format '{{{{ json .SBOM }}' 2>/dev/null)
    prov=$(docker buildx imagetools inspect "$ref" --format '{{{{ json .Provenance }}' 2>/dev/null)
    rc=0
    # Both are manifests inside the index, so the index digest hashes them and the one signature above already
    # covers them. There is nothing extra to verify here - this reports what is attached.
    packages=$(echo "$sbom" | jq -r '[.. | objects | select(has("packages")) | .packages | length] | add' 2>/dev/null)
    if [ -n "$packages" ] && [ "$packages" != "null" ]; then
        printf '  sbom        %s packages\n' "$packages"
    else
        echo "  sbom        MISSING" >&2
        rc=1
    fi
    builder=$(echo "$prov" | jq -r '[.. | objects | select(has("builder")) | .builder.id] | first' 2>/dev/null)
    if [ -n "$builder" ] && [ "$builder" != "null" ]; then
        printf '  provenance  %s\n' "$builder"
    else
        echo "  provenance  MISSING" >&2
        rc=1
    fi
    exit $rc

# Labels, and what the container says about itself
[private]
_verify-metadata version:
    #!/usr/bin/env bash
    set -uo pipefail
    echo "== metadata =="
    ref="{{ hub_repo }}:{{ version }}"
    docker pull -q "$ref" >/dev/null 2>&1 || {
        echo "  could not pull $ref" >&2
        exit 1
    }
    docker image inspect "$ref" --format '{{{{ json .Config.Labels }}' | jq -r '
        "  version     \(.["org.opencontainers.image.version"] // "-")",
        "  revision    \(.["org.opencontainers.image.revision"] // "-")",
        "  base        \(.["org.opencontainers.image.base.name"] // "-")"'
    # From inside a throwaway run: what the image says about itself, not what the registry says about it. The
    # System dll count is the framework-dependent proof - a self-contained build shows ~170, not a handful.
    docker run --rm --entrypoint sh "$ref" -c '
        printf "  reports     %s\n" "${BINACLE_VERSION:--}"
        printf "  runs as     %s (%s)\n" "$(id -un)" "$(id -u)"
        printf "  /app/data   %s\n" "$(stat -c "%U:%G %a" /app/data 2>/dev/null || echo -)"
        printf "  framework   %s System.*.dll in /app\n" "$(ls /app/System.*.dll 2>/dev/null | wc -l)"
    '
