#!/usr/bin/env bash

#exit if any command fails
set -e

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then  
  rm -R $artifactsFolder
fi

dotnet restore

# Ideally we would use the 'dotnet test' command to test netcoreapp and net451 so restrict for now 
# but this currently doesn't work due to https://github.com/dotnet/cli/issues/3073 so restrict to netcoreapp

dotnet test ./test/Piranha.Tests/Piranha.Tests.csproj -c Release -f netcoreapp1.1
dotnet test ./test/Piranha.AttributeBuilder.Tests/Piranha.AttributeBuilder.Tests.csproj -c Release -f netcoreapp1.1
