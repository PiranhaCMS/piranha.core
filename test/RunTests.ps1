echo "Running test/core tests"
foreach ($test in ls core/*.Tests) {
    Push-Location $test

    echo "Testing project in $test"

    dotnet restore
    dotnet test

    Pop-Location
}

echo ""
echo "Running test/data tests"
foreach ($test in ls data/*.Tests) {
    Push-Location $test

    echo "Testing project in $test"

    dotnet restore
    dotnet test

    Pop-Location
}

echo ""
echo "Running test/env tests"
foreach ($test in ls env/*.Tests) {
    Push-Location $test

    echo "Testing project in $test"

    dotnet restore
    dotnet test

    Pop-Location
}

echo ""
echo "Running test/examples tests"
foreach ($test in ls examples/*.Tests) {
    Push-Location $test

    echo "Testing project in $test"

    dotnet restore
    dotnet test

    Pop-Location
}