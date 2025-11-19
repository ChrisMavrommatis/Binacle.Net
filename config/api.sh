#!/bin/bash

FILE_PATH=$( realpath "$0"  )
ROOT_DIR=$( dirname "$FILE_PATH" )
ROOT_DIR=$( dirname "$( dirname "$FILE_DIR" )" )
PROJECT_PATH='src/Binacle.Net/'

# set working directory to the root of the project
cd "$ROOT_DIR/$PROJECT_PATH" || exit 1

# Create a dictionary to hold aliases for the launch profiles
#WithUiModuleOnly WithAllModules WithServiceModuleOnly Normal
declare -A launch_profile_aliases=(
    ["Normal"]="Normal"
    ["N"]="Normal"
    
    ["WithServiceModuleOnly"]="WithServiceModuleOnly"
    ["S"]="WithServiceModuleOnly"
    
    ["WithUiModuleOnly"]="WithUiModuleOnly"
    ["U"]="WithServiceModuleOnly"
    
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
