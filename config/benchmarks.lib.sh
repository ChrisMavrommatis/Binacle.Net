#!/bin/bash

FILE_PATH=$( realpath "$0"  )
FILE_DIR=$( dirname "$FILE_PATH" )
ROOT_DIR=$( dirname "$FILE_DIR" )
PROJECT_PATH='lib/test/Binacle.Lib.Benchmarks/'

# set working directory to the root of the project
cd "$ROOT_DIR/$PROJECT_PATH" || exit 1

# Aliases for the benchmark classes (glob patterns matched against the type name).
declare -A benchmark_aliases=(
    ["FastValidation"]="*FastValidation*"
    ["AlgorithmRacing"]="*AlgorithmRacing*"
    ["BischoffSuite"]="*BischoffSuite*"
    ["Parallelization"]="*ParallelizationThreshold*"
    ["ResultSelection"]="*ResultSelection*"
)

echo "Running from $ROOT_DIR"

# Get Argument
if [ $# -eq 0 ]; then
    echo "No arguments provided. Running all benchmarks."
    FILTER="*"
else
    FILTER="${benchmark_aliases[$1]}"
    if [ -z "$FILTER" ]; then
        echo "Invalid benchmark. Available: ${!benchmark_aliases[*]}"
        exit 1
    fi
fi

echo "Running benchmarks with filter: $FILTER"

dotnet run -c Release --filter "$FILTER"

# Reports land (markdown only, pinned) in BenchmarkDotNet.Artifacts/results/ — copy keepers into
# results/lib/benchmarks/ by hand. See results/README.md for the scratch-vs-curated convention.
echo "Reports in ${PROJECT_PATH}BenchmarkDotNet.Artifacts/results/ — curate keepers into results/lib/benchmarks/"
