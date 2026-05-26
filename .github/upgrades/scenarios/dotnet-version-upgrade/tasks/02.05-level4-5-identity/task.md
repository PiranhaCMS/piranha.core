# 02.05-level4-5-identity: Multi-target Level 4-5 Identity libraries

## Objective
Add net10.0 target framework to Identity libraries at the top of the dependency chain.

## Scope
- Piranha.AspNetCore.Identity (identity\Piranha.AspNetCore.Identity\Piranha.AspNetCore.Identity.csproj) — Level 4, has API issues
- Piranha.AspNetCore.Identity.SQLite (identity\Piranha.AspNetCore.Identity.SQLite\Piranha.AspNetCore.Identity.SQLite.csproj) — Level 5

## Prerequisites
Level 0-3 libraries (02.01-02.04) must be complete

## Research
- Query assessment for Piranha.AspNetCore.Identity API issues (source incompatibilities)

## Done when
Both projects target net8.0;net9.0;net10.0, build successfully, API issues resolved
