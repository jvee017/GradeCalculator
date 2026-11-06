#!/bin/bash
cd "$(dirname "$0")"
if ! command -v dotnet >/dev/null 2>&1; then
  echo ".NET SDK is not installed. Get it at https://dotnet.microsoft.com/download"
  exit 1
fi
echo "Starting... open http://localhost:5080 (Ctrl+C to stop)"
dotnet run
