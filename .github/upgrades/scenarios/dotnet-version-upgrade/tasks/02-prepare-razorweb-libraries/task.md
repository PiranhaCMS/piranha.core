# 02-prepare-razorweb-libraries: Multi-target libraries for RazorWeb

Add net10.0 target framework to all libraries that RazorWeb depends on, working bottom-up through the dependency chain. This includes: Piranha (Level 0), Piranha.Manager.Localization (Level 0), foundation libraries (Level 1), data access libraries (Level 2), manager and identity libraries (Levels 2-5), and RazorWeb's direct dependencies.

**Affected libraries** (13 total):
- Level 0-1: Piranha, Piranha.Manager.Localization, Piranha.AttributeBuilder, Piranha.Local.FileStorage, Piranha.ImageSharp, Piranha.AspNetCore.Hosting, Piranha.Data.EF
- Level 2: Piranha.AspNetCore, Piranha.Data.EF.MySql, Piranha.Data.EF.PostgreSql, Piranha.Data.EF.SQLite, Piranha.Data.EF.SQLServer, Piranha.Manager
- Level 3: Piranha.Manager.LocalAuth, Piranha.Manager.TinyMCE
- Level 4-5: Piranha.AspNetCore.Identity, Piranha.AspNetCore.Identity.SQLite

**Assessment signals**: 19 package updates recommended, 1 security vulnerability in Piranha.Data.EF, API behavioral changes in Piranha and Piranha.Azure.BlobStorage, source incompatibilities in Piranha.AspNetCore and Piranha.AspNetCore.Identity.

**Research starting points**: Check for conditional compilation needs in projects with API issues, review package compatibility for Entity Framework providers, investigate security vulnerability details in Piranha.Data.EF.

**Done when**: All RazorWeb dependency libraries target net8.0;net9.0;net10.0, solution builds successfully with multi-targeting, no new errors introduced
