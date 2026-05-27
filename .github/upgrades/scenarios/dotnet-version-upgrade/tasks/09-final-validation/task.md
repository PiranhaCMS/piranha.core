# 09-final-validation: Full solution validation and documentation

Build entire solution, run complete test suite, verify no warnings remain, document any deferred recommendations or known issues. Review and address security vulnerability in Piranha.Data.EF identified in assessment. Validate all package updates applied successfully.

**Scope**: Complete solution
**Key validations**:
- Zero build errors across all projects
- Zero warnings (or documented acceptable warnings)
- Complete test suite passes
- Security vulnerability in Piranha.Data.EF resolved
- All 19 recommended package updates applied

**Done when**: Clean solution build, all tests passing, security issues resolved, upgrade summary documented with any post-upgrade recommendations

---

## Research Findings

### Current State (from previous task completions)
- All upgradeable projects now target `net10.0` via Directory.Build.props
- Four MySQL/PostgreSQL provider projects remain on `net8.0;net9.0` by user decision (Option 1)
- Solution builds successfully after consolidation
- RazorWeb and MvcWeb both upgraded to net10.0

### Key Package Vulnerability from Assessment
- **AutoMapper 12.0.1** → 16.1.1 recommended (security vulnerability)
  - Used in: `data/Piranha.Data.EF/Piranha.Data.EF.csproj`
  - Status: Needs verification and potential update

### Build Tool Selection (per building-projects skill)
- **Primary tool**: `dotnet build` (SDK-style projects, modern .NET targets)
- **Solution scope**: Full solution build required for final validation
- **Restore strategy**: Always restore during final validation (packages changed throughout)

### Test Projects
- `test/Piranha.Tests/Piranha.Tests.csproj` - upgraded to net10.0
- `test/Piranha.Manager.Tests/Piranha.Manager.Tests.csproj` - upgraded to net10.0

### Warning Policy
- Per building-projects skill: fix ALL warnings in modified projects, not just new ones
- Current expectation: solution should build warning-free or with documented exceptions

### Affected Files
- **Solution file**: `Piranha.sln`
- **All 26 projects**: Already analyzed during earlier tasks
- **Central config**: `Directory.Build.props` (consolidated to net10.0)
- **Unsupported providers** (remain on older TFMs):
  - `data/Piranha.Data.EF.MySql/Piranha.Data.EF.MySql.csproj`
  - `data/Piranha.Data.EF.PostgreSql/Piranha.Data.EF.PostgreSql.csproj`
  - `identity/Piranha.AspNetCore.Identity.MySQL/Piranha.AspNetCore.Identity.MySQL.csproj`
  - `identity/Piranha.AspNetCore.Identity.PostgreSQL/Piranha.AspNetCore.Identity.PostgreSQL.csproj`
