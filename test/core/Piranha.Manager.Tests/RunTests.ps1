$failedUnitTestCount = 0
$controllerNamespace = "Piranha.Manager.Tests.Areas.Manager.Controllers"
$controllerTests = @("PageControllerUnitTest", "BlockTypeControllerUnitTest", "BlockControllerUnitTest")

foreach ($test in $controllerTests) {
	echo "`n`nbuild: Running $controllerNamespace.$test"
	& dotnet test -class "$controllerNamespace.$test"
	
	if ($LASTEXITCODE -ne 0) {
        $failedUnitTestCount += 1
    }
}

if ($failedUnitTestCount -ne 0) {
	exit 1
}