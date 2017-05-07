#!/usr/bin/env bash

# Exit if any command fails
set -e

# Setup
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
OPENCOVER=$SCRIPT_DIR/packages/OpenCover.4.6.519/tools/OpenCover.Console.exe
REPORTGENERATOR=$SCRIPT_DIR/packages/ReportGenerator.2.4.5.0/tools/ReportGenerator.exe

# Install code coverage tools
nuget install -Verbosity quiet -OutputDirectory $SCRIPT_DIR/packages -Version 4.6.519 OpenCover
nuget install -Verbosity quiet -OutputDirectory $SCRIPT_DIR/packages -Version 2.4.5.0 ReportGenerator

# Run tests
echo "Starting tests..."
dotnet test ./test/Piranha.Tests/Piranha.Tests.csproj
dotnet test ./test/Piranha.AttributeBuilder.Tests/Piranha.AttributeBuilder.Tests.csproj

# Create report folers
echo "Creating coverage folders..."
coverage=./coverage
rm -rf $coverage
mkdir $coverage

# Generate coverage
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

# Generate report
echo "Generating HTML report..."
$REPORTGENERATOR \
  -reports:$coverage/coverage.xml \
  -targetdir:$coverage \
  -verbosity:Error

    Contact GitHub API Training Shop Blog About 

