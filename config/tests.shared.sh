#!/bin/bash

FILE_PATH=$( realpath "$0"  )
FILE_DIR=$( dirname "$FILE_PATH" )
ROOT_DIR=$( dirname "$FILE_DIR" )
CS_PROJECT='shared/test/Binacle.CompactNotation.UnitTests'
TS_PACKAGE='packages/binacle-compact-notation'

run_cs() { echo "Running C# unit tests: $CS_PROJECT"; dotnet run --project "$ROOT_DIR/$CS_PROJECT"; }
run_ts() { echo "Running TS tests: $TS_PACKAGE"; ( cd "$ROOT_DIR/$TS_PACKAGE" && npm test ); }

echo "Running from $ROOT_DIR"

case "${1:-all}" in
    cs)  run_cs ;;
    ts)  run_ts ;;
    all) run_cs && run_ts ;;
    *)   echo "Usage: $(basename "$0") [cs|ts]   (no arg runs both)"; exit 1 ;;
esac
