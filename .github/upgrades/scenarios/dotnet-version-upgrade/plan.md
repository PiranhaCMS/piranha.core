# .NET 10 Upgrade Plan

## Overview

**Target**: Upgrade Piranha CMS solution from .NET 8/9 to .NET 10.0 (LTS)
**Scope**: 26 projects across 7 dependency tiers, ~65 issues identified (26 mandatory TFM changes, 19 package updates, 17 API compatibility issues)

### Selected Strategy
**Top-Down (Application-First)** — Applications upgraded first, libraries multi-targeted temporarily.
**Rationale**: 26 projects with deep 7-tier dependency graph benefit from incremental buildability. Critical example applications upgraded first while maintaining solution stability through multi-targeting of shared libraries.

### Application vs Library Classification

**Applications** (priority order):
1. RazorWeb — Razor Pages example application (Level 6, depends on 13 libraries)
2. MvcWeb — MVC example application (Level 6, depends on 10 libraries)

**Test Projects** (upgraded with their dependencies):
- Piranha.Tests — Core functionality tests (Level 3)
- Piranha.Manager.Tests — Manager UI tests (Level 3)

**Libraries requiring multi-targeting** (Phases 1-2):
All 22 class libraries will be multi-targeted (net8.0;net9.0;net10.0) during Phase 1, then consolidated to net10.0 only in Phase 2.

### Phase Transition
Phase 2 (library consolidation) begins after both example applications are successfully upgraded and validated.

## Tasks

### 01-prerequisites: Verify SDK and toolchain readiness

Validate that .NET 10 SDK is installed and all build infrastructure supports the target framework. Update global.json if present to allow .NET 10 SDK. Verify project format compatibility.

**Scope**: Solution-wide infrastructure check
**Key checks**: 
- .NET 10 SDK availability
- global.json compatibility
- Build tooling support

**Done when**: .NET 10 SDK verified available, global.json updated (if exists), solution builds successfully on current frameworks

---

### 02-prepare-razorweb-libraries: Multi-target libraries for RazorWeb

Add net10.0 target framework to all libraries that RazorWeb depends on, working bottom-up through the dependency chain. This includes: Piranha (Level 0), Piranha.Manager.Localization (Level 0), foundation libraries (Level 1), data access libraries (Level 2), manager and identity libraries (Levels 2-5), and RazorWeb's direct dependencies.

**Affected libraries** (13 total):
- Level 0-1: Piranha, Piranha.Manager.Localization, Piranha.AttributeBuilder, Piranha.Local.FileStorage, Piranha.ImageSharp, Piranha.AspNetCore.Hosting, Piranha.Data.EF
- Level 2: Piranha.AspNetCore, Piranha.Data.EF.MySql, Piranha.Data.EF.PostgreSql, Piranha.Data.EF.SQLite, Piranha.Data.EF.SQLServer, Piranha.Manager
- Level 3: Piranha.Manager.LocalAuth, Piranha.Manager.TinyMCE
- Level 4-5: Piranha.AspNetCore.Identity, Piranha.AspNetCore.Identity.SQLite

**Assessment signals**: 19 package updates recommended, 1 security vulnerability in Piranha.Data.EF, API behavioral changes in Piranha and Piranha.Azure.BlobStorage, source incompatibilities in Piranha.AspNetCore and Piranha.AspNetCore.Identity.

**Research starting points**: Check for conditional compilation needs in projects with API issues, review package compatibility for Entity Framework providers, investigate security vulnerability details in Piranha.Data.EF.

**Done when**: All RazorWeb dependency libraries target net8.0;net9.0;net10.0, solution builds successfully with multi-targeting, no new errors introduced

---

### 03-upgrade-razorweb: Upgrade RazorWeb application to .NET 10

Update RazorWeb's target framework to net10.0, resolve any API breaking changes, update packages to versions compatible with .NET 10, and validate the application builds and runs correctly.

**Scope**: examples/RazorWeb project (Level 6)
**Assessment signals**: 1 mandatory TFM change, depends on 13 multi-targeted libraries (now supporting net10.0)
**Key concerns**: Razor Pages-specific behavioral changes, routing changes in .NET 10, middleware pipeline compatibility

**Done when**: RazorWeb targets net10.0 only, builds without errors or warnings, runtime validation successful (application starts and serves pages)

---

### 04-prepare-mvcweb-libraries: Multi-target remaining libraries for MvcWeb

Add net10.0 target framework to any libraries not already multi-targeted from the RazorWeb work. This primarily includes libraries unique to MvcWeb's dependency chain.

**Scope**: Check MvcWeb dependencies against already multi-targeted libraries from task 02
**Expected**: Most libraries already multi-targeted; may need to add net10.0 to any MvcWeb-specific dependencies if present

**Done when**: All MvcWeb dependency libraries target net8.0;net9.0;net10.0, solution builds successfully

---

### 05-upgrade-mvcweb: Upgrade MvcWeb application to .NET 10

Update MvcWeb's target framework to net10.0, resolve any API breaking changes, update packages, and validate the application builds and runs correctly.

**Scope**: examples/MvcWeb project (Level 6)
**Assessment signals**: 1 mandatory TFM change, depends on 10 multi-targeted libraries
**Key concerns**: MVC-specific routing changes, controller API compatibility, view rendering changes in .NET 10

**Done when**: MvcWeb targets net10.0 only, builds without errors or warnings, runtime validation successful (application starts and serves pages)

---

### 06-upgrade-standalone-libraries: Upgrade standalone library projects

Upgrade library projects that are not consumed by example applications but are part of the solution: Piranha.WebApi (Level 1) and Piranha.Azure.BlobStorage (Level 1). These can be upgraded directly to net10.0 without multi-targeting since they have no dependent projects.

**Scope**: 2 standalone libraries
- Piranha.WebApi (1 issue)
- Piranha.Azure.BlobStorage (9 issues including behavioral changes)

**Assessment signals**: Piranha.Azure.BlobStorage has API behavioral changes that need review and testing

**Done when**: Both projects target net10.0 only, build without errors or warnings, unit tests pass (if present)

---

### 07-upgrade-test-projects: Upgrade test projects to .NET 10

Update test projects to target net10.0 and resolve deprecated package issues. Update test framework packages (xUnit, NUnit, MSTest) to versions compatible with .NET 10.

**Scope**: 2 test projects
- Piranha.Tests (Level 3) — depends on 6 libraries
- Piranha.Manager.Tests (Level 3) — depends on Piranha.Manager

**Assessment signals**: Both projects have deprecated NuGet packages (NuGet.0005) that need replacement or updating
**Key concerns**: Test framework compatibility, test discovery and execution, deprecated package replacements

**Done when**: Both test projects target net10.0, all tests build successfully, deprecated packages replaced, test suite runs and passes

---

### 08-consolidate-libraries: Remove multi-targeting from all libraries

Remove net8.0 and net9.0 target frameworks from all libraries, leaving only net10.0. Clean up any conditional compilation directives that were added for multi-targeting. Update package references to use single-target versions where applicable.

**Scope**: All 22 class libraries (Levels 0-5)
**Prerequisites**: All applications and test projects successfully upgraded to net10.0

**Done when**: All libraries target net10.0 only, solution builds successfully, no conditional compilation artifacts remain, all tests pass

---

### 09-final-validation: Full solution validation and documentation

Build entire solution, run complete test suite, verify no warnings remain, document any deferred recommendations or known issues. Review and address security vulnerability in Piranha.Data.EF identified in assessment. Validate all package updates applied successfully.

**Scope**: Complete solution
**Key validations**:
- Zero build errors across all projects
- Zero warnings (or documented acceptable warnings)
- Complete test suite passes
- Security vulnerability in Piranha.Data.EF resolved
- All 19 recommended package updates applied

**Done when**: Clean solution build, all tests passing, security issues resolved, upgrade summary documented with any post-upgrade recommendations
