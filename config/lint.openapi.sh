#!/bin/bash

FILE_PATH=$( realpath "$0" )
FILE_DIR=$( dirname "$FILE_PATH" )
ROOT_DIR=$( dirname "$FILE_DIR" )

# set working directory to the root of the project
cd "$ROOT_DIR" || exit 1

# Generate the OpenAPI documents, then lint them against .spectral.yaml.
bash config/api.sh openapi || exit 1

echo "Linting OpenAPI documents in build/openapi/ ..."
npx --yes @stoplight/spectral-cli lint build/openapi/*
