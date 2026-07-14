#!/bin/bash

FILE_PATH=$( realpath "$0"  )
FILE_DIR=$( dirname "$FILE_PATH" )
ROOT_DIR=$( dirname "$FILE_DIR" )
CS_PROJECT='lib/test/Binacle.Lib.UnitTests'

echo "Running from $ROOT_DIR"
echo "Running C# unit tests: $CS_PROJECT"

dotnet run --project "$ROOT_DIR/$CS_PROJECT"
