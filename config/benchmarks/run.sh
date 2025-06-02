#!/bin/bash

BENCH_FILE_PATH=$( realpath "$0"  )
BENCH_FILE_DIR=$( dirname "$BENCH_FILE_PATH" )
ROOT_DIR=$( dirname "$( dirname "$BENCH_FILE_DIR" )" )
BENCHMARKS_PROJECT_PATH='test/Binacle.Lib.Benchmarks/'

# set working directory to the root of the project
cd "$ROOT_DIR/$BENCHMARKS_PROJECT_PATH" || exit 1

# Create a dictionary to hold aliases for the benchmark projects
declare -A benchmark_aliases=(
    ["FittingCubeScaling"]="*FittingCubeScaling*"
    ["PackingCubeScaling"]="*PackingCubeScaling*"
    
    ["FittingMultipleBins"]="*FittingMultipleBins*"
    ["PackingMultipleBins"]="*PackingMultipleBins*"
    
    ["FittingMultipleItems"]="*FittingMultipleItems*"
    ["PackingMultipleItems"]="*PackingMultipleItems*"
    ["PackingCubeScalingMultiAlgorithm"]="*PackingCubeScalingMultiAlgorithm*"
    ["FittingCubeScalingMultiAlgorithm"]="*FittingCubeScalingMultiAlgorithm*"
   
#    ["Scaling_Fitting_FFD"]="*Scaling.Fitting_FFD*"
#    ["Scaling_FittingComparison"]="*Scaling.FittingComparison*"
#    ["Scaling_Packing_FFD"]="*Scaling.Packing_FFD*"
#    ["Scaling_Packing_BFD"]="*Scaling.Packing_BFD*"
#    ["Scaling_Packing_WFD"]="*Scaling.Packing_WFD*"
#    ["Scaling_PackingComparison"]="*Scaling.PackingComparison*"
#    ["Combination_FittingSingleBaseLine"]="*Combination.FittingSingleBaseLine*"
#    ["Combination_PackingSingleBaseLine"]="*Combination.PackingSingleBaseLine*"
#    ["Combination_FittingMultipleBins"]="*Combination.FittingMultipleBins*"
#    ["Combination_PackingMultipleBins"]="*Combination.PackingMultipleBins*"
)

echo "Running from $ROOT_DIR"

# Get Argument
if [ $# -eq 0 ]; then
    echo "No arguments provided. Running all benchmarks."
    FILTER=""
else
    FILTER="${benchmark_aliases[$1]}"
    if [ -z "$FILTER" ]; then
        echo "Invalid benchmark."
        exit 1
    fi
fi

echo "Running benchmarks with filter: $FILTER"

dotnet run -c Release --filter "$FILTER"
