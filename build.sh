#!/usr/bin/env bash

#exit if any command fails
set -e

# Define directories.
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
TOOLS_DIR=$SCRIPT_DIR/tools
NUGET_EXE=$TOOLS_DIR/nuget.exe
NUGET_URL=https://dist.nuget.org/win-x86-commandline/latest/nuget.exe

# Make sure the tools folder exist.
if [ ! -d "$TOOLS_DIR" ]; then
  mkdir "$TOOLS_DIR"
fi

# Download NuGet if it does not exist.
if [ ! -f "$NUGET_EXE" ]; then
    echo "Downloading NuGet..."
    curl -Lsfo "$NUGET_EXE" $NUGET_URL
    if [ $? -ne 0 ]; then
        echo "An error occured while downloading nuget.exe."
        exit 1
    fi
fi

# Restore packages
echo "Restoring NuGet packages..."
ulimit -n 2048
dotnet restore

# Build solution
echo "Building solution..."
dotnet build

# Test
echo "Starting tests..."
dotnet test ./test/Piranha.Tests/Piranha.Tests.csproj
