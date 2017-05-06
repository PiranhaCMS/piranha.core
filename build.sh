#!/usr/bin/env bash

#exit if any command fails
set -e

# Define directories.
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
TOOLS_DIR=$SCRIPT_DIR/tools
NUGET_EXE=$TOOLS_DIR/nuget.exe
NUGET_URL=https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
OPENCOVER=$SCRIPT_DIR/packages/OpenCover.4.6.519/tools/OpenCover.Console.exe
REPORTGENERATOR=$SCRIPT_DIR/packages/ReportGenerator.2.4.5.0/tools/ReportGenerator.exe

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
dotnet restore

# Install code coverage tools
nuget install -Verbosity quiet -OutputDirectory packages -Version 4.6.519 OpenCover
nuget install -Verbosity quiet -OutputDirectory packages -Version 2.4.5.0 ReportGenerator

# Build solution
echo "Building solution..."
dotnet build

# Test
echo "Starting tests..."
dotnet test ./test/Piranha.Tests/Piranha.Tests.csproj
dotnet test ./test/Piranha.AttributeBuilder.Tests/Piranha.AttributeBuilder.Tests.csproj

# Create report folers
echo "Creating coverage folders..."
coverage=./coverage
rm -rf $coverage
mkdir $coverage

# Generate test reports
echo "Calculating coverage..."
$OPENCOVER \
  -target:"dotnet" \
  -targetargs:"test -f netcoreapp1.1 test/Piranha.Tests test/Piranha.AttributeBuilder.Tests" \
  -mergeoutput \
  -hideskipped:File \
  -output:$coverage/coverage.xml \
  -oldStyle \
  -filter:"+[Piranha*]* -[Piranha.*Tests*]*" \
  -searchdirs:$testdir/bin/Release/netcoreapp1.1 \
  -register:user

echo "Generating HTML report..."
$REPORTGENERATOR \
  -reports:$coverage/coverage.xml \
  -targetdir:$coverage \
  -verbosity:Error