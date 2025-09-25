#!/bin/bash

FILE_PATH=$( realpath "$0"  )
FILE_DIR=$( dirname "$FILE_PATH" )
ROOT_DIR=$( dirname "$( dirname "$FILE_DIR" )" )
PROJECT_PATH='docs/'

# set working directory to the root of the project
cd "$ROOT_DIR/$PROJECT_PATH" || exit 1

echo "Running from $ROOT_DIR"

bundle exec jekyll serve
