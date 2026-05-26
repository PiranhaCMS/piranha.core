# 05-upgrade-mvcweb: Upgrade MvcWeb application to .NET 10

Update MvcWeb's target framework to net10.0, resolve any API breaking changes, update packages, and validate the application builds and runs correctly.

**Scope**: examples/MvcWeb project (Level 6)
**Assessment signals**: 1 mandatory TFM change, depends on 10 multi-targeted libraries
**Key concerns**: MVC-specific routing changes, controller API compatibility, view rendering changes in .NET 10

**Done when**: MvcWeb targets net10.0 only, builds without errors or warnings, runtime validation successful (application starts and serves pages)
