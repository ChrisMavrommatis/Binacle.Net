#!/bin/bash

FILE_PATH=$( realpath "$0"  )
FILE_DIR=$( dirname "$FILE_PATH" )
ROOT_DIR=$( dirname "$FILE_DIR" )
PROJECT_PATH='api/src/Binacle.Net/'

# set working directory to the root of the project
cd "$ROOT_DIR/$PROJECT_PATH" || exit 1

# OpenAPI option: build with the spec-generation flag and emit the documents, instead of running the app.
if [ "$1" == "openapi" ] || [ "$1" == "oa" ]; then
    echo "Generating OpenAPI documents (v3, v4)..."
    dotnet build -p:GenerateOpenApi=true || exit 1
    echo "OpenAPI documents written to build/openapi/"
    exit 0
fi

# Create a dictionary to hold aliases for the launch profiles
#WithUiModuleOnly WithAllModules WithServiceModuleOnly Normal
declare -A launch_profile_aliases=(
    ["Normal"]="Normal"
    ["N"]="Normal"
    
    ["WithServiceModuleOnly"]="WithServiceModuleOnly"
    ["S"]="WithServiceModuleOnly"
    
    ["WithUiModuleOnly"]="WithUiModuleOnly"
    ["U"]="WithUiModuleOnly"
    
    ["WithAllModules"]="WithAllModules"
    ["All"]="WithAllModules"
)

echo "Running from $ROOT_DIR"

# Get Argument
if [ $# -eq 0 ]; then
    echo "No arguments provided. Running 'Normal' launch profile"
    LP_ARG="Normal"
else
    LP_ARG="${launch_profile_aliases[$1]}"
    if [ -z "$LP_ARG" ]; then
        echo "Invalid Launch Profile."
        exit 1
    fi
fi

echo "Running Binacle.Net with launch Profile: $LP_ARG"

dotnet run -lp "$LP_ARG"
