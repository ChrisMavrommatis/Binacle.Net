#!/bin/bash

FILE_PATH=$( realpath "$0"  )
FILE_DIR=$( dirname "$FILE_PATH" )
ROOT_DIR=$( dirname "$FILE_DIR" )
PROJECT_PATH='test/Binacle.Lib.Benchmarks/'

# set working directory to the root of the project
cd "$ROOT_DIR/$PROJECT_PATH" || exit 1

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
    
    ["PackingMultiAlgorithm"]="*PackingMultiAlgorithm*"
    ["FittingMultiAlgorithm"]="*FittingMultiAlgorithm*"
   
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
