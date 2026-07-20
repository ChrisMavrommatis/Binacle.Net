#!/bin/bash

FILE_PATH=$( realpath "$0"  )
FILE_DIR=$( dirname "$FILE_PATH" )
ROOT_DIR=$( dirname "$FILE_DIR" )
CORE='api/test/Binacle.Net.IntegrationTests'
SERVICE='api/test/Binacle.Net.ServiceModule.IntegrationTests'

run_core()    { echo "Running core integration tests: $CORE"; dotnet test "$ROOT_DIR/$CORE"; }

# Infra picks the ServiceModule DB backend via BINACLE_TEST_INFRA. Assumes its service is already up
# (docker compose -f config/docker-compose.yml up -d). No arg: BINACLE_TEST_INFRA is left unset and the
# harness falls back to SQLite on its own.
run_service() {
    local infra="$1"
    if [ -n "$infra" ]; then
        echo "Running ServiceModule integration tests ($infra): $SERVICE"
        BINACLE_TEST_INFRA="$infra" dotnet test "$ROOT_DIR/$SERVICE"
    else
        echo "Running ServiceModule integration tests (harness default): $SERVICE"
        dotnet test "$ROOT_DIR/$SERVICE"
    fi
}

echo "Running from $ROOT_DIR"

case "${1:-all}" in
    core)    run_core ;;
    service) run_service "$2" ;;
    all)     run_core && run_service ;;
    *)       echo "Usage: $(basename "$0") [core | service [Sqlite|Postgres|AzureStorage]]   (no arg runs both)"; exit 1 ;;
esac
