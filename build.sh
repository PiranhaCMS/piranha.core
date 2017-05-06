#!/usr/bin/env bash

#exit if any command fails
set -e

dotnet restore
dotnet build
dotnet test ./test/Piranha.Tests/Piranha.Tests.csproj
dotnet test ./test/Piranha.AttributeBuilder.Tests/Piranha.AttributeBuilder.Tests.csproj
