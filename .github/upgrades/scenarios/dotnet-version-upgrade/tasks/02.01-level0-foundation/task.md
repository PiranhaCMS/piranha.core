# 02.01-level0-foundation: Multi-target Level 0 foundation libraries

## Objective
Add net10.0 target framework to Level 0 foundation libraries that have no dependencies.

## Scope
- Piranha (core\Piranha\Piranha.csproj)
- Piranha.Manager.Localization (core\Piranha.Manager.Localization\Piranha.Manager.Localization.csproj)

## Approach
1. Convert TargetFramework to TargetFrameworks for both projects
2. Add net10.0 to existing net8.0;net9.0
3. Build each project to verify multi-targeting works
4. Resolve any package compatibility issues

## Done when
Both projects target net8.0;net9.0;net10.0, build successfully, no new errors
