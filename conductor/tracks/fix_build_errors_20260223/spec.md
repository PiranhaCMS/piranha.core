# Track Specification: Resolve Solution Build Errors

## Overview
This track focuses on identifying and resolving all compilation errors across the entire TriviaTitans solution resulting from the Piranha migration. The goal is to reach a state where the entire solution, including core libraries, infrastructure, examples, and unit tests, compiles successfully.

## Scope
- **Core Projects**: Resolve errors in Piranha core libraries.
- **Infrastructure Projects**: Resolve errors in Aero.Identity and Data access layers.
- **Example Projects**: Fix errors in MvcWeb and other sample applications.
- **Test Projects**: Ensure all unit test projects compile (even if tests are not run or passing).

## Requirements
- **Compilation**: Every project in the solution must compile without errors.
- **Migration Alignment**: Fixes must align with the intended architectural direction (RavenDB integration, removal of EF Core).
- **Warning Management**: Focus strictly on errors; warnings may be left for subsequent optimization tracks.

## Acceptance Criteria
- `dotnet build` executed at the solution root (`Piranha.slnx`) returns a success status.
- Individual projects can be built successfully using `dotnet build <ProjectPath>`.
- The `Aero.Identity` and `Piranha.Data.EF` (or its successor) projects are error-free.

## Out of Scope
- Fixing failing unit tests (only compilation is required).
- Resolving compiler warnings.
- New feature implementation.
