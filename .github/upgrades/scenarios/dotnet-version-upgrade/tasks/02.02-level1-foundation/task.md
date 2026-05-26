# 02.02-level1-foundation: Multi-target Level 1 foundation libraries

## Objective
Add net10.0 target framework to Level 1 libraries that depend only on Level 0.

## Scope
- Piranha.AttributeBuilder (core\Piranha.AttributeBuilder\Piranha.AttributeBuilder.csproj)
- Piranha.Local.FileStorage (core\Piranha.Local.FileStorage\Piranha.Local.FileStorage.csproj)
- Piranha.ImageSharp (core\Piranha.ImageSharp\Piranha.ImageSharp.csproj)
- Piranha.AspNetCore.Hosting (core\Piranha.AspNetCore.Hosting\Piranha.AspNetCore.Hosting.csproj)
- Piranha.Data.EF (data\Piranha.Data.EF\Piranha.Data.EF.csproj)

## Prerequisites
Level 0 libraries (02.01) must be complete

## Approach
1. Add net10.0 to each project's target frameworks
2. Build each project individually
3. Address security vulnerability in Piranha.Data.EF during package updates
4. Resolve any EF Core package compatibility issues

## Done when
All 5 projects target net8.0;net9.0;net10.0, build successfully, Piranha.Data.EF security vulnerability addressed
