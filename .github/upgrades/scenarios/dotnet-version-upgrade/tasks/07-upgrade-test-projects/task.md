# 07-upgrade-test-projects: Upgrade test projects to .NET 10

Update test projects to target net10.0 and resolve deprecated package issues. Update test framework packages (xUnit, NUnit, MSTest) to versions compatible with .NET 10.

**Scope**: 2 test projects
- Piranha.Tests (Level 3) — depends on 6 libraries
- Piranha.Manager.Tests (Level 3) — depends on Piranha.Manager

**Assessment signals**: Both projects have deprecated NuGet packages (NuGet.0005) that need replacement or updating
**Key concerns**: Test framework compatibility, test discovery and execution, deprecated package replacements

**Done when**: Both test projects target net10.0, all tests build successfully, deprecated packages replaced, test suite runs and passes
