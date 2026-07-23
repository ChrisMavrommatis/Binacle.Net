#!/bin/bash

BUILD_FILE_PATH=$( realpath "$0"  )
BUILD_FILE_DIR=$( dirname "$BUILD_FILE_PATH" )
ROOT_DIR=$( dirname "$BUILD_FILE_DIR" )
API_PROJECT_PATH='api/src/Binacle.Net/Binacle.Net.csproj'
VER=local

# set working directory to the root of the project
cd "$ROOT_DIR" || exit 1

REQUIRED_DIRS=(
  "$BUILD_FILE_DIR/data/"
  "$BUILD_FILE_DIR/data/logs/"
  "$BUILD_FILE_DIR/data/pack-logs/"
  "$BUILD_FILE_DIR/azurite/"
)
  
for dir in "${REQUIRED_DIRS[@]}"; do
  if [ ! -d "$dir" ]; then
    echo "Creating directory: $dir"
    mkdir -p "$dir"
  fi
done

echo "Building from $ROOT_DIR"

# The containers write as their own users, not as you, and docker never chowns a bind mount — without this
# the app cannot write its logs.
chmod -R 777 "$BUILD_FILE_DIR/data/"
sudo chmod -R 777 "$BUILD_FILE_DIR/azurite/"

# Stop on the first failure. Without this, a failed build still runs compose, which then tries to pull
# binacle-net from Docker Hub and reports "pull access denied" — hiding the error that actually mattered.
set -e

dotnet restore $API_PROJECT_PATH --runtime linux-x64

dotnet publish $API_PROJECT_PATH -c Release -o build/output --no-restore --self-contained --runtime linux-x64

docker build --build-arg VERSION=$VER -t binacle-net:$VER .
