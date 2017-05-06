#!/usr/bin/env bash

#exit if any command fails
set -e

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then  
  rm -R $artifactsFolder
fi

dotnet restore
dotnet build
dotnet test ./test/Piranha.Tests/Piranha.Tests.csproj
dotnet test ./test/Piranha.AttributeBuilder.Tests/Piranha.AttributeBuilder.Tests.csproj
