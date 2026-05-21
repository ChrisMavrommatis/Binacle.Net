#!/bin/bash

FILE_PATH=$( realpath "$0"  )
FILE_DIR=$( dirname "$FILE_PATH" )
ROOT_DIR=$( dirname "$FILE_DIR" )

# Create a dictionary to hold aliases for the test projects
declare -A test_project_aliases=(
    ["lib"]="lib/test/Binacle.Lib.UnitTests"
    ["api"]="api/test/Binacle.Net.IntegrationTests"
    ["api_service"]="api/test/Binacle.Net.ServiceModule.IntegrationTests"
    ["vipaq"]="vipaq/test/Binacle.ViPaq.UnitTests"
    ["performance"]="lib/test/Binacle.Lib.PerformanceTests"
)

echo "Running from $ROOT_DIR"

# Get Argument
if [ $# -eq 0 ]; then
    echo "No arguments provided. You need to provide arguments"
    exit 1
fi

PROJ_ARG=${test_project_aliases[$1]}
if [ -z "$PROJ_ARG" ]; then
    echo "Invalid Project"
    exit 1
fi

echo "Running tests for $PROJ_ARG:"

dotnet run --project "$ROOT_DIR/$PROJ_ARG"
