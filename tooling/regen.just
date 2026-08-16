# The committed-data generators, loaded by the root justfile as the `regen` module.
#
#   just regen all                     every generator, in dependency order
#   just regen or-lib-scenarios        OR-Library text -> shared/data/bischoff-suite
#   just regen vipaq-packed-data       those + custom-problems, packed -> vipaq/data/packed
#   just regen vipaq-interop-vectors   the interop pair -> vipaq/test-vectors
#   just regen check                   regenerate, then fail if any of that changed
#
# Every tool here rewrites files that are committed. All four take no arguments by design - a run does every
# generator in its list, so it cannot half-run and leave the data inconsistent.

# Module recipes run where the module file lives (tooling/), which would break every path below.
set working-directory := '..'

# The tools print why they failed, so just's own "Recipe failed" line adds nothing. The exit code still
# propagates.
set no-exit-message := true

ts_package := "vipaq/packages/binacle-vipaq"

# List the recipes
default:
    @just --list regen

# or-lib-scenarios writes what vipaq-packed-data reads, so the order is the recipe.

# Run every generator
all: or-lib-scenarios vipaq-packed-data vipaq-interop-vectors

# OR-Library text -> the tests-kernel scenarios in shared/data/bischoff-suite
or-lib-scenarios:
    dotnet run --project shared/tools/Binacle.OrLibrary.Converter

# Pack those + custom-problems -> the placed results in vipaq/data/packed
vipaq-packed-data:
    dotnet run --project vipaq/tools/Binacle.ViPaq.PackedDataGenerator

# The two halves write cs/ and ts/ of the same interop matrix off one input.json. Regenerating one alone is
# the drift InteropIntegrityTests exists to catch, so they are one recipe.

# Both interop halves and the header bytes -> vipaq/test-vectors
vipaq-interop-vectors:
    dotnet run --project vipaq/tools/Binacle.ViPaq.VectorGenerators
    npx ts-node --project {{ ts_package }}/tsconfig.json {{ ts_package }}/tools/generateVectors.ts

# Every generator is deterministic and its output is committed, so a clean tree after a full run is the whole
# check. Nothing in CI calls this - it is here for the maintainer who edited a tool or a source problem.

# Regenerate everything, then fail if the committed data changed
check: all
    #!/usr/bin/env bash
    set -euo pipefail

    # Only the .json the generators write. These folders also hold their own README, and vipaq/test-vectors
    # as a whole holds the hand-authored vectors no generator touches.
    generated=(
        ':(glob)shared/data/bischoff-suite/**/*.json'
        ':(glob)vipaq/data/packed/**/*.json'
        ':(glob)vipaq/test-vectors/header/**/*.json'
        ':(glob)vipaq/test-vectors/interop/cs/**/*.json'
        ':(glob)vipaq/test-vectors/interop/ts/**/*.json'
    )

    changed="$(git status --porcelain -- "${generated[@]}")"
    if [[ -n "$changed" ]]; then
        echo "Committed data does not match what the generators produce:" >&2
        echo "$changed" >&2
        exit 1
    fi
    echo "Committed data matches the generators."
