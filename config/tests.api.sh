#!/bin/bash

FILE_PATH=$( realpath "$0"  )
FILE_DIR=$( dirname "$FILE_PATH" )
ROOT_DIR=$( dirname "$FILE_DIR" )
CORE='api/test/Binacle.Net.IntegrationTests'
SERVICE='api/test/Binacle.Net.ServiceModule.IntegrationTests'

run_core()    { echo "Running core integration tests: $CORE"; dotnet run --project "$ROOT_DIR/$CORE"; }
run_service() { echo "Running ServiceModule integration tests: $SERVICE"; dotnet run --project "$ROOT_DIR/$SERVICE"; }

echo "Running from $ROOT_DIR"

case "${1:-all}" in
    core)    run_core ;;
    service) run_service ;;
    all)     run_core && run_service ;;
    *)       echo "Usage: $(basename "$0") [core|service]   (no arg runs both)"; exit 1 ;;
esac
