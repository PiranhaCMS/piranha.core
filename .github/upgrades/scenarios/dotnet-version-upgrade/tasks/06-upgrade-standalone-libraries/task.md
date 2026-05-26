# 06-upgrade-standalone-libraries: Upgrade standalone library projects

Upgrade library projects that are not consumed by example applications but are part of the solution: Piranha.WebApi (Level 1) and Piranha.Azure.BlobStorage (Level 1). These can be upgraded directly to net10.0 without multi-targeting since they have no dependent projects.

**Scope**: 2 standalone libraries
- Piranha.WebApi (1 issue)
- Piranha.Azure.BlobStorage (9 issues including behavioral changes)

**Assessment signals**: Piranha.Azure.BlobStorage has API behavioral changes that need review and testing

**Done when**: Both projects target net10.0 only, build without errors or warnings, unit tests pass (if present)
