# .NET Version Upgrade to .NET 10

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0 (LTS)
- **Scope**: All projects in solution

## Source Control
- **Source Branch**: master
- **Working Branch**: dotnet-version-upgrade
- **Commit Strategy**: After Each Task

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: Top-Down

## Strategy
**Selected**: Top-Down (Application-First)
**Rationale**: 26 projects with deep 7-tier dependency graph. Incremental buildability needed at scale. Example applications upgraded first while maintaining solution stability through library multi-targeting.

### Execution Constraints
- Applications first: upgrade example applications before consolidating libraries
- Multi-target libraries temporarily: add net10.0 alongside existing TFMs during Phase 1
- Prepare dependencies bottom-up: multi-target libraries in dependency order before upgrading consuming application
- Phase transition gate: Phase 2 (library consolidation) only starts after ALL applications successfully upgraded
- Between-application validation: confirm solution builds after each application upgrade before proceeding to next
