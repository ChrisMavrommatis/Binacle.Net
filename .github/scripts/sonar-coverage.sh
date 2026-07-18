#!/bin/bash

# Builds, runs every suite, and writes coverage in the formats SonarCloud reads. Run between the scanner's
# begin and end steps — the build has to happen inside that pair or the scanner never sees the projects.
#
# Lives here rather than in config/ because config/ is for local setup and this only ever runs on GitHub:
# it is useless without the begin/end pair wrapped around it.
#
# Sonar does NOT read cobertura, which is what config/coverage.sh produces for the local HTML report. Same
# collectors, different output:
#   C# -> Visual Studio coverage xml -> sonar.cs.vscoveragexml.reportsPaths
#   TS -> lcov                       -> sonar.javascript.lcov.reportPaths
#
# The service suite needs Azurite up. Without it all 106 of its tests fail and its assemblies read ~0% —
# a failed suite still writes a valid-looking report, so this script exits non-zero instead of letting
# Sonar record a fiction.

FILE_PATH=$( realpath "$0" )
FILE_DIR=$( dirname "$FILE_PATH" )
# Up out of .github/scripts — two levels, unlike the config/ scripts which sit one below the root.
ROOT_DIR=$( dirname "$( dirname "$FILE_DIR" )" )
ARTIFACTS_DIR="$ROOT_DIR/CoverageArtifacts/sonar"

declare -A cs_projects=(
    ["lib"]="lib/test/Binacle.Lib.UnitTests"
    ["api"]="api/test/Binacle.Net.IntegrationTests"
    ["service"]="api/test/Binacle.Net.ServiceModule.IntegrationTests"
    ["vipaq"]="vipaq/test/Binacle.ViPaq.UnitTests"
    ["compact-notation"]="shared/test/Binacle.CompactNotation.UnitTests"
)

echo "Running from $ROOT_DIR"
rm -rf "$ARTIFACTS_DIR"
mkdir -p "$ARTIFACTS_DIR"

echo ""
echo "=== build"
dotnet build "$ROOT_DIR/Binacle.Net.slnx" --configuration Release || exit 1

failed_targets=()

for target in "${!cs_projects[@]}"; do
    project="${cs_projects[$target]}"
    echo ""
    echo "=== coverage: $target ($project)"
    dotnet run --project "$ROOT_DIR/$project" --configuration Release --no-build -- \
        --coverage \
        --coverage-output-format xml \
        --coverage-output "$ARTIFACTS_DIR/$target.coverage.xml" \
    || failed_targets+=("$target")
done

# Run jest from the repo root (jest.config.js fans out to both workspaces) so each recorded path carries its
# workspace folder. Run it per-workspace and every path is "src/foo.ts" — Sonar resolves those from the repo
# root, finds nothing, and reports 0% without complaining.
echo ""
echo "=== coverage: typescript"
(
    cd "$ROOT_DIR" || exit 1
    npx jest --coverage --coverageReporters=lcovonly --coverageDirectory="$ARTIFACTS_DIR/ts"
) || failed_targets+=("typescript")

if [ ${#failed_targets[@]} -gt 0 ]; then
    echo ""
    echo "!!! These suites failed: ${failed_targets[*]}"
    echo "!!! Not reporting to Sonar — coverage from a failed run is meaningless."
    echo "!!! If 'service' is in that list, check Azurite is up: docker compose -f config/docker-compose.yml up -d"
    exit 1
fi

echo ""
echo "Coverage written to $ARTIFACTS_DIR"
