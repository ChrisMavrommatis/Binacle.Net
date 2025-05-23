#!/bin/bash

BENCH_FILE_PATH=$( realpath "$0"  )
BENCH_FILE_DIR=$( dirname "$BENCH_FILE_PATH" )
ROOT_DIR=$( dirname "$( dirname "$BENCH_FILE_DIR" )" )
BENCHMARKS_PROJECT_PATH='test/Binacle.Lib.Benchmarks/'

# set working directory to the root of the project
cd "$ROOT_DIR/$BENCHMARKS_PROJECT_PATH" || exit 1

echo "Running from $ROOT_DIR"

# Scaling_Fitting_FFD  = --filter *Scaling.Fitting_FFD*
# Scaling_FittingComparison = --filter *Scaling.FittingComparison*
# Scaling_Packing_FFD = --filter *Scaling.Packing_FFD*
# Scaling_PackingComparison = --filter *Scaling.PackingComparison*
# Combination_FittingSingleBaseLine = --filter *Combination.FittingSingleBaseLine*
# Combination_PackingSingleBaseLine = --filter *Combination.PackingSingleBaseLine*
# Combination_FittingMultipleBins = --filter *Combination.FittingMultipleBins*
# Combination_PackingMultipleBins = --filter *Combination.PackingMultipleBins*


dotnet run -c Release --filter *Scaling.Fitting_FFD*
