#!/bin/bash

FILE_PATH=$( realpath "$0"  )
FILE_DIR=$( dirname "$FILE_PATH" )
ROOT_DIR=$( dirname "$FILE_DIR" )
ARTIFACTS_DIR="$ROOT_DIR/CoverageArtifacts"

# C# suites. Each writes cobertura via Microsoft.Testing.Extensions.CodeCoverage (the `--coverage` flag).
# 18.0.4 is the ceiling while we are on xunit.v3 (MTP v1): CodeCoverage 18.1+ needs MTP 2.x and dies
# with a TypeLoadException. Moving up means switching to the xunit.v3.mtp-v2 package first.
#
# The service suite needs Azurite up (config/docker-compose). With it down all 106 tests fail and its
# assemblies report ~0% — a failed suite still produces a valid-looking report, so this script refuses
# to print one unless every suite passed.
declare -A cs_projects=(
    ["lib"]="lib/test/Binacle.Lib.UnitTests"
    ["api"]="api/test/Binacle.Net.IntegrationTests"
    ["service"]="api/test/Binacle.Net.ServiceModule.IntegrationTests"
    ["vipaq"]="vipaq/test/Binacle.ViPaq.UnitTests"
    ["compact-notation"]="shared/test/Binacle.CompactNotation.UnitTests"
)

# TS packages. Jest emits cobertura natively, so both languages land in one format and merge into
# a single report.
declare -A ts_packages=(
    ["ts-compact-notation"]="packages/binacle-compact-notation"
    ["ts-vipaq"]="vipaq/packages/binacle-vipaq"
)

echo "Running from $ROOT_DIR"
rm -rf "$ARTIFACTS_DIR"
mkdir -p "$ARTIFACTS_DIR/reports"

failed_targets=()

for target in "${!cs_projects[@]}"; do
    project="${cs_projects[$target]}"
    echo ""
    echo "=== coverage: $target ($project)"
    dotnet run --project "$ROOT_DIR/$project" -- \
        --coverage \
        --coverage-output-format cobertura \
        --coverage-output "$ARTIFACTS_DIR/reports/$target.cobertura.xml" \
    || failed_targets+=("$target")
done

for target in "${!ts_packages[@]}"; do
    package="${ts_packages[$target]}"
    echo ""
    echo "=== coverage: $target ($package)"
    (
        cd "$ROOT_DIR/$package" || exit 1
        npx jest --coverage --coverageReporters=cobertura --coverageDirectory="$ARTIFACTS_DIR/$target"
    ) || failed_targets+=("$target")
    # Jest names each cobertura package after the source folder ("src.layouts", or "main" for loose files),
    # because JS has no namespace to put there. That splits one npm package across many rows and lets two
    # packages collide on "src". Rewrite them all to the npm package name so a TS row means the same thing
    # as a C# row: one deliverable, one line.
    if [ -f "$ARTIFACTS_DIR/$target/cobertura-coverage.xml" ]; then
        pkg_name=$(node -p "require('$ROOT_DIR/$package/package.json').name")
        sed "s|<package name=\"[^\"]*\"|<package name=\"$pkg_name\"|g" \
            "$ARTIFACTS_DIR/$target/cobertura-coverage.xml" \
            > "$ARTIFACTS_DIR/reports/$target.cobertura.xml"
        rm -rf "$ARTIFACTS_DIR/$target"
    fi
done

# A suite that fails still writes a report, and the merged number comes out looking perfectly normal —
# just wrong, because unrun code reads as uncovered. Refuse to report rather than publish a fiction.
if [ ${#failed_targets[@]} -gt 0 ]; then
    echo ""
    echo "!!! These suites failed: ${failed_targets[*]}"
    echo "!!! No report written — coverage from a failed run is meaningless."
    echo "!!! If 'service' is in that list, check Azurite is up: docker compose -f config/docker-compose.yml up -d"
    exit 1
fi

echo ""
echo "=== merging with ReportGenerator"
dotnet tool restore > /dev/null
# Test-support assemblies are dropped: a test helper scoring itself says nothing about shipped code.
dotnet reportgenerator \
    -reports:"$ARTIFACTS_DIR/reports/*.cobertura.xml" \
    -targetdir:"$ARTIFACTS_DIR/html" \
    -assemblyfilters:"-Binacle.TestsKernel;-*.UnitTests;-*.IntegrationTests" \
    -reporttypes:"Html;TextSummary;MarkdownSummaryGithub" \
    > /dev/null

echo ""
cat "$ARTIFACTS_DIR/html/Summary.txt"
echo ""
echo "HTML report: $ARTIFACTS_DIR/html/index.html"
echo "Artifacts are gitignored scratch — copy a keeper into results/ by hand."
