#!/bin/sh

VER=local

docker build --build-arg VERSION=$VER -t binacle-net:$VER .