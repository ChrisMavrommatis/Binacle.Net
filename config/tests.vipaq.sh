#!/bin/bash

FILE_PATH=$( realpath "$0"  )
FILE_DIR=$( dirname "$FILE_PATH" )
ROOT_DIR=$( dirname "$FILE_DIR" )
CS_PROJECT='vipaq/test/Binacle.ViPaq.UnitTests'
TS_PACKAGE='vipaq/packages/binacle-vipaq'

run_cs() { echo "Running C# unit tests: $CS_PROJECT"; dotnet test "$ROOT_DIR/$CS_PROJECT"; }
run_ts() { echo "Running TS tests: $TS_PACKAGE"; ( cd "$ROOT_DIR" && npx jest --projects "$TS_PACKAGE" ); }

echo "Running from $ROOT_DIR"

case "${1:-all}" in
    cs)  run_cs ;;
    ts)  run_ts ;;
    all) run_cs && run_ts ;;
    *)   echo "Usage: $(basename "$0") [cs|ts]   (no arg runs both)"; exit 1 ;;
esac
