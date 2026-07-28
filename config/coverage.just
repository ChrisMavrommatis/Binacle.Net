# Coverage, built on top of the test module. Loaded by the root justfile as the `coverage` module.
#
#   just coverage all              every test with coverage, then the table
#   just coverage all sonar        the same, in the formats Sonar imports
#   just coverage report           merge the last cobertura run into one HTML report
#
# The format names the consumer, not the file format: `cobertura` is what the table and the HTML report read,
# `sonar` is Visual Studio xml for C# plus lcov for TS. Each writes one flat file per suite into
# build/coverage/<format>/, named after the project or package.
#
# Nothing here knows which test project or package exists, and nothing here runs a test. Each recipe sets
# COVERAGE_FORMAT and calls `just test all`; every leaf writes its own files into build/, because coverage is
# the same run with extra output, not a second run. What is left here is only what happens afterwards: merge
# and report.
#
# The steps are public on purpose - `just --list coverage` shows them and each one can be run alone.

# Module recipes run where the module file lives (config/), which would break every path below.
set working-directory := '..'

# The runner already said why it failed and `table` prints the verdict, so just's own line adds nothing.
set no-exit-message := true

# Run every test with coverage, then print the table [cobertura|sonar]
all format='cobertura': (run format) (table format)

# Run every test with the coverage collector attached [cobertura|sonar]
run format:
    #!/usr/bin/env bash
    set -euo pipefail
    case '{{ format }}' in
        cobertura | sonar) ;;
        *) echo "Unknown format '{{ format }}'. Use cobertura or sonar." >&2; exit 1 ;;
    esac

    # The whole folder, not just this format's: a leftover cobertura run would otherwise still be sitting there
    # after a sonar run, and `report` would merge it and look perfectly normal. build/coverage always holds
    # exactly one run.
    rm -rf build/tests build/coverage

    # `|| true` because a failing suite still wrote its report, and `table` is what reports the verdict.
    # Without it a failed run would print no table at all - the one time you most want to see it.
    COVERAGE_FORMAT='{{ format }}' just test all || true

# Merge the last cobertura run into one HTML report
report:
    #!/usr/bin/env bash
    set -euo pipefail
    # Only cobertura carries the per-line data the report needs; a sonar run leaves nothing to merge.
    if [ ! -d build/coverage/cobertura ]; then
        echo "No cobertura coverage in build/coverage. Run 'just coverage all' first." >&2
        exit 1
    fi
    dotnet tool restore > /dev/null
    # Test-support assemblies are dropped: a test helper scoring itself says nothing about shipped code.
    dotnet reportgenerator \
        -reports:'build/coverage/cobertura/*.xml' \
        -targetdir:'build/coverage/html-report' \
        -assemblyfilters:'-Binacle.TestsKernel;-*.UnitTests;-*.IntegrationTests' \
        -reporttypes:'Html;TextSummary;MarkdownSummaryGithub' \
        > /dev/null
    echo "HTML report: build/coverage/html-report/index.html"

# One row per suite, and the run's exit code (the Coverage column needs cobertura)
table format='cobertura':
    #!/usr/bin/env bash
    set -uo pipefail
    shopt -s nullglob
    total_failed=0

    # The counts sit in a small summary object, so one grep each is enough - no JSON parser needed.
    count() { grep -o "\"$2\":[0-9]*" "$1" | head -1 | cut -d: -f2; }

    # Cobertura keeps the whole-file rate on its root element, which is all a row needs.
    rate() {
        local file="build/coverage/{{ format }}/$1.xml"
        [ -f "$file" ] || { echo "-"; return; }
        local value
        value=$(grep -o 'line-rate="[0-9.]*"' "$file" | head -1 | cut -d'"' -f2)
        [ -n "$value" ] && awk "BEGIN { printf \"%.1f%%\", $value * 100 }" || echo "-"
    }

    row() {
        printf '%-44s Passed:%-6s Failed:%-4s Skipped:%-4s Coverage:%s\n' "$1" "$2" "$3" "$4" "$(rate "$1")"
        total_failed=$((total_failed + $3))
    }

    echo ""
    # The C# suites write CTRF; jest writes its own JSON. Same numbers, different field names.
    for report in build/tests/*.ctrf.json; do
        name=$(basename "$report" .ctrf.json)
        row "$name" "$(count "$report" passed)" "$(count "$report" failed)" "$(count "$report" skipped)"
    done
    for report in build/tests/*.jest.json; do
        name=$(basename "$report" .jest.json)
        row "$name" "$(count "$report" numPassedTests)" "$(count "$report" numFailedTests)" \
            "$(count "$report" numPendingTests)"
    done

    echo ""
    if [ "$total_failed" -gt 0 ]; then
        echo "$total_failed failed - scroll up for the failures, each suite printed its own."
        exit 1
    fi
    echo "All suites passed."
