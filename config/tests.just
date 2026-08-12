# Test leaves, loaded by the root justfile as the `test` module. Run one with `just test <leaf>`, all of
# them with `just test all`, and list them with `just --list test`.
#
# A leaf is one project and one run, so it is also one CI step. One recipe per leaf because shell
# completion only completes recipe names - a leaf passed as an argument cannot complete without a
# hand-written list that drifts.

# Module recipes run where the module file lives (config/), which would break every path below.
set working-directory := '..'

# The test runner already printed why it failed, so just's own "Recipe failed" line adds nothing. The exit
# code still propagates, so CI still fails.
set no-exit-message := true

# CI builds the solution once and runs every leaf against that build; on a laptop a plain `dotnet test` is
# what you want. An env var, not a just variable: a module variable cannot be set from the command line, so
# `just dotnet_test_args=... test lib-unit` would be ignored in silence.
#   DOTNET_TEST_ARGS="--configuration Release --no-build" just test all
dotnet_test_args := env('DOTNET_TEST_ARGS', '')

# List the leaves
default:
    @just --list test

# The leaves are listed by hand rather than read out of this file, so `all` states what it runs. It is the
# infra-free set: every leaf here passes on a machine with nothing brought up, the ServiceModule one on its
# SQLite fallback. That is a judgement per leaf, not something a recipe list can tell you - a new leaf joins
# `all` only once someone decides it needs nothing. Cheap leaves first, so failures show up early.
#
# A failing leaf does not stop the rest, the same as CI's `if: !cancelled()` - one command should report
# everything that is broken, not just the first thing.

# Every leaf that needs nothing brought up first
all:
    #!/usr/bin/env bash
    set -uo pipefail

    # Takes the leaf's arguments too, so a line can name a backend: `leaf api-service-integration Postgres`.
    failed=""
    test_leaf() {
        echo "=== just test $* ==="
        just test::"$@" || failed="$failed $1"
    }

    test_leaf lib-unit
    test_leaf shared-cs-unit
    test_leaf shared-ts-unit
    test_leaf vipaq-cs-unit
    test_leaf vipaq-ts-unit
    test_leaf api-core-unit
    test_leaf api-kernel-unit
    test_leaf api-diagnostics-unit
    test_leaf api-service-unit
    test_leaf api-core-integration
    test_leaf api-service-integration Sqlite

    if [ -n "$failed" ]; then
        echo "FAILED:$failed" >&2
        exit 1
    fi
    echo "All leaves passed."

# Binacle.Lib unit
[group('lib')]
lib-unit: (_dotnet_test 'lib/test/Binacle.Lib.UnitTests')

# CompactNotation unit (C#)
[group('shared')]
shared-cs-unit: (_dotnet_test 'shared/test/Binacle.CompactNotation.UnitTests')

# binacle-compact-notation unit (TS)
[group('shared')]
shared-ts-unit: (_jest_test 'binacle-compact-notation')

# ViPaq unit (C#)
[group('vipaq')]
vipaq-cs-unit: (_dotnet_test 'vipaq/test/Binacle.ViPaq.UnitTests')

# binacle-vipaq unit (TS)
[group('vipaq')]
vipaq-ts-unit: (_jest_test 'binacle-vipaq')

# The api leaves run units first, then integrations, so a bare `just test all` reports the cheap failures
# before it spends minutes in a test host.

# Binacle.Net unit
[group('api')]
api-core-unit: (_dotnet_test 'api/test/Binacle.Net.UnitTests')

# Kernel unit
[group('api')]
api-kernel-unit: (_dotnet_test 'api/test/Binacle.Net.Kernel.UnitTests')

# DiagnosticsModule unit
[group('api')]
api-diagnostics-unit: (_dotnet_test 'api/test/Binacle.Net.DiagnosticsModule.UnitTests')

# ServiceModule unit
[group('api')]
api-service-unit: (_dotnet_test 'api/test/Binacle.Net.ServiceModule.UnitTests')

# Binacle.Net integration
[group('api')]
api-core-integration: (_dotnet_test 'api/test/Binacle.Net.IntegrationTests')

# The backend is a positional argument, not a variable: a module variable cannot be set from the command
# line, and `just infra=Postgres test api-service-integration` would be ignored in silence - a green run for
# a backend nobody exercised. Empty lets the harness pick (SQLite), so a bare run needs nothing brought up.
# Postgres and AzureStorage need theirs up:
#   just serve services -d

# ServiceModule integration [Sqlite|Postgres|AzureStorage]
[group('api')]
api-service-integration backend="":
    #!/usr/bin/env bash
    set -euo pipefail
    # A misspelled backend would otherwise fall through to the harness default and pass, reporting
    # a green run for a backend nobody exercised.
    case "{{ backend }}" in
        "" | Sqlite | Postgres | AzureStorage) ;;
        *) echo "Unknown backend '{{ backend }}'. Use Sqlite, Postgres or AzureStorage." >&2; exit 1 ;;
    esac
    export BINACLE_TEST_INFRA="{{ backend }}"
    just test::_dotnet_test api/test/Binacle.Net.ServiceModule.IntegrationTests

# Runs one test project. Every C# leaf goes through here, so the flags live in one place.
#
# Files are named after the project alone. Nothing needs to tell C# from TS today - in build/coverage/sonar the
# extension already does it (.xml here, .info from jest), and in build/coverage/cobertura both are .xml and
# every consumer wants them merged anyway. If that changes, put the language in the name (`<name>.cs.xml` /
# `<name>.ts.info`) rather than in a folder, so the Sonar globs stay one line each.
#
# Coverage is not a second run - the collector rides along inside this one. Set COVERAGE_FORMAT and the same
# leaf also writes its results and coverage into build/, named after the project. `just coverage all` sets it.
#
# The output path must be absolute: a relative --coverage-output lands inside the project's own
# bin/.../TestResults instead of where you asked.
[private]
_dotnet_test project:
    #!/usr/bin/env bash
    set -euo pipefail

    if [ -z "${COVERAGE_FORMAT:-}" ]; then
        dotnet test --project {{ project }} {{ dotnet_test_args }}
        exit 0
    fi

    # COVERAGE_FORMAT names the consumer, not the file format, so the folder can be named after it too.
    case "$COVERAGE_FORMAT" in
        cobertura) output_format=cobertura ;;
        sonar)     output_format=xml ;;  # Visual Studio coverage, the only C# format Sonar imports
        *) echo "Unknown COVERAGE_FORMAT '$COVERAGE_FORMAT'. Use cobertura or sonar." >&2; exit 1 ;;
    esac

    name=$(basename {{ project }})
    results="$(pwd)/build/tests"
    coverage="$(pwd)/build/coverage/$COVERAGE_FORMAT"
    mkdir -p "$results" "$coverage"

    # No `--` before the runner options: the Microsoft.Testing.Platform `dotnet test` hands unrecognized
    # options to the test app itself, and a `--` makes it read the project path as a stray argument.
    dotnet test --project {{ project }} {{ dotnet_test_args }} \
        --results-directory "$results" \
        --report-ctrf --report-ctrf-filename "$name.ctrf.json" \
        --coverage --coverage-output-format "$output_format" \
        --coverage-output "$coverage/$name.xml"

# Runs one TS package - the jest half of _dotnet_test, same two modes.
#
# `--selectProjects` runs the package through the *root* jest.config.js, which is the point: the root config's
# `projects` list keeps the workspace folder in recorded paths, and its `collectCoverageFrom` is ignored when a
# package's own config drives the run (`--projects <path>`), quietly pulling test helpers into the numbers. The
# name is the package's `displayName`, which matches its npm name.
#
# Paths are absolute because jest resolves them against the package, so a relative one would land inside the
# package instead of build/.
[private]
_jest_test package:
    #!/usr/bin/env bash
    set -euo pipefail

    if [ -z "${COVERAGE_FORMAT:-}" ]; then
        npx jest --selectProjects {{ package }}
        exit 0
    fi

    # Same consumer names as _dotnet_test, mapped to what jest calls them: Sonar reads JS coverage as lcov,
    # not the Visual Studio xml it wants for C#.
    case "$COVERAGE_FORMAT" in
        cobertura) reporter=cobertura ;;
        sonar)     reporter=lcovonly ;;
        *) echo "Unknown COVERAGE_FORMAT '$COVERAGE_FORMAT'. Use cobertura or sonar." >&2; exit 1 ;;
    esac

    results="$(pwd)/build/tests"
    coverage="$(pwd)/build/coverage/$COVERAGE_FORMAT"
    mkdir -p "$results" "$coverage"

    npx jest --selectProjects {{ package }} \
        --coverage --coverageReporters="$reporter" --coverageDirectory="$coverage/{{ package }}" \
        --json --outputFile="$results/{{ package }}.jest.json"

    # jest names the file after its reporter and buries it in a folder. Lift it out, named after the package
    # and keeping its extension, so the folder holds one flat file per suite like the C# ones.
    for produced in "$coverage/{{ package }}"/*; do
        [ -f "$produced" ] || continue
        mv "$produced" "$coverage/{{ package }}.${produced##*.}"
    done
    rmdir "$coverage/{{ package }}"
