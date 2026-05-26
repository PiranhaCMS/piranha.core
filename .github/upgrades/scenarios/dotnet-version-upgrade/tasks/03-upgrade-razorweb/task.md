# 03-upgrade-razorweb: Upgrade RazorWeb application to .NET 10

Update RazorWeb's target framework to net10.0, resolve any API breaking changes, update packages to versions compatible with .NET 10, and validate the application builds and runs correctly.

**Scope**: examples/RazorWeb project (Level 6)
**Assessment signals**: 1 mandatory TFM change, depends on 13 multi-targeted libraries (now supporting net10.0)
**Key concerns**: Razor Pages-specific behavioral changes, routing changes in .NET 10, middleware pipeline compatibility

**Done when**: RazorWeb targets net10.0 only, builds without errors or warnings, runtime validation successful (application starts and serves pages)
