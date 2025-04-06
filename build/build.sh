#!/bin/sh


API_PROJECT_PATH='src/Binacle.Net/Binacle.Net.csproj'

dotnet restore --runtime linux-x64

dotnet publish $API_PROJECT_PATH -c Release -o build/output --no-restore --self-contained --runtime linux-x64
