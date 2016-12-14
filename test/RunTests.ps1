$failedProjectCount = 0

echo "`n`n"
echo "build: Running test/core tests"
foreach ($test in ls core/*.Tests) {
    Push-Location $test

    echo "Testing project in $test"

    dotnet restore
    & dotnet test
    if ($LASTEXITCODE -ne 0) {
        $failedProjectCount += 1
    }

    Pop-Location
}

echo "`n`n"
echo "build: Running test/data tests"
foreach ($test in ls data/*.Tests) {
    Push-Location $test

    echo "Testing project in $test"

    dotnet restore
    & dotnet test
    if ($LASTEXITCODE -ne 0) {
        $failedProjectCount += 1
    }

    Pop-Location
}

echo "`n`n"
echo "build: Running test/env tests"
foreach ($test in ls env/*.Tests) {
    Push-Location $test

    echo "Testing project in $test"

    dotnet restore
    & dotnet test
    if ($LASTEXITCODE -ne 0) {
        $failedProjectCount += 1
    }

    Pop-Location
}

echo "`n`n"
echo "build: Running test/examples tests"
foreach ($test in ls examples/*.Tests) {
    Push-Location $test

    echo "Testing project in $test"

    dotnet restore
    & dotnet test
    if ($LASTEXITCODE -ne 0) {
        $failedProjectCount += 1
    }

    Pop-Location
}

if ($failedProjectCount -ne 0)
{
    exit 1
}