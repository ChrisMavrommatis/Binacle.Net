#!/bin/bash

FILE_PATH=$( realpath "$0"  )
FILE_DIR=$( dirname "$FILE_PATH" )
ROOT_DIR=$( dirname "$FILE_DIR" )
TESTS_ROOT_DIR='test/'

# set working directory to the root of the tests
cd "$ROOT_DIR/$TESTS_ROOT_DIR" || exit 1

# Create a dictionary to hold aliases for the test projects
declare -A test_project_aliases=(
    ["lib"]="Binacle.Lib.UnitTests"
    ["api"]="Binacle.Net.IntegrationTests"
    ["api_service"]="Binacle.Net.ServiceModule.IntegrationTests"
    ["vipaq"]="Binacle.ViPaq.UnitTests"
)

echo "Running from $ROOT_DIR/$TESTS_ROOT_DIR"

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

dotnet run --project "$PROJ_ARG"
