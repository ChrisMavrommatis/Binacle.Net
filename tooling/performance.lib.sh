#!/bin/bash

FILE_PATH=$( realpath "$0"  )
FILE_DIR=$( dirname "$FILE_PATH" )
ROOT_DIR=$( dirname "$FILE_DIR" )
PROJECT_PATH='lib/test/Binacle.Lib.PerformanceTests/'

# set working directory to the root of the project
cd "$ROOT_DIR/$PROJECT_PATH" || exit 1

echo "Running from $ROOT_DIR"
echo "Running lib performance tests"

dotnet run -c Release

# Reports land in PerformanceTests.Artifacts (gitignored scratch). Copy keepers into
# results/lib/efficiency/ by hand. See results/README.md for the scratch-vs-curated convention.
echo "Reports in ${PROJECT_PATH}PerformanceTests.Artifacts/ — curate keepers into results/lib/efficiency/"
