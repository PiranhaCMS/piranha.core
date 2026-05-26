# 02.03-level2-data-web: Multi-target Level 2 data and web libraries

## Objective
Add net10.0 target framework to Level 2 libraries: data providers and AspNetCore components.

## Scope
- Piranha.AspNetCore (core\Piranha.AspNetCore\Piranha.AspNetCore.csproj) — has API issues
- Piranha.Data.EF.MySql (data\Piranha.Data.EF.MySql\Piranha.Data.EF.MySql.csproj)
- Piranha.Data.EF.PostgreSql (data\Piranha.Data.EF.PostgreSql\Piranha.Data.EF.PostgreSql.csproj)
- Piranha.Data.EF.SQLite (data\Piranha.Data.EF.SQLite\Piranha.Data.EF.SQLite.csproj)
- Piranha.Data.EF.SQLServer (data\Piranha.Data.EF.SQLServer\Piranha.Data.EF.SQLServer.csproj)
- Piranha.Manager (core\Piranha.Manager\Piranha.Manager.csproj)

## Prerequisites
Level 0-1 libraries (02.01-02.02) must be complete

## Research
- Query assessment for Piranha.AspNetCore API issues (source incompatibilities)
- Check EF provider package compatibility with net10.0

## Done when
All 6 projects target net8.0;net9.0;net10.0, build successfully, API issues in Piranha.AspNetCore resolved
