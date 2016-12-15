$failedProjectCount = 0
$unitTestScript = "RunTests.ps1"

echo "`n"
echo "build: Running test/core tests"
foreach ($test in ls core/*.Tests) {
    Push-Location $test

    echo "build: Running '$unitTestScript' in $test"

    dotnet restore
    dotnet build
	& powershell -ExecutionPolicy ByPass -File "$unitTestScript"
    if ($LASTEXITCODE -ne 0) {
        $failedProjectCount += 1
    }

    Pop-Location
}

echo "`n`n"
echo "build: Running test/data tests"
foreach ($test in ls data/*.Tests) {
    Push-Location $test

    echo "build: Running 'RunTests.ps1' in $test"

    dotnet restore
    dotnet build
	& powershell -ExecutionPolicy ByPass -File "$unitTestScript"
    if ($LASTEXITCODE -ne 0) {
        $failedProjectCount += 1
    }

    Pop-Location
}

echo "`n`n"
echo "build: Running test/env tests"
foreach ($test in ls env/*.Tests) {
    Push-Location $test

    echo "build: Running 'RunTests.ps1' in $test"

    dotnet restore
    dotnet build
	& powershell -ExecutionPolicy ByPass -File "$unitTestScript"
    if ($LASTEXITCODE -ne 0) {
        $failedProjectCount += 1
    }

    Pop-Location
}

echo "`n`n"
echo "build: Running test/examples tests"
foreach ($test in ls examples/*.Tests) {
    Push-Location $test

    echo "build: Running 'RunTests.ps1' in $test"

    dotnet restore
    dotnet build
	& powershell -ExecutionPolicy ByPass -File "$unitTestScript"
    if ($LASTEXITCODE -ne 0) {
        $failedProjectCount += 1
    }

    Pop-Location
}

if ($failedProjectCount -ne 0) {
    exit 1
}