# 04-prepare-mvcweb-libraries: Multi-target remaining libraries for MvcWeb

Add net10.0 target framework to any libraries not already multi-targeted from the RazorWeb work. This primarily includes libraries unique to MvcWeb's dependency chain.

**Scope**: Check MvcWeb dependencies against already multi-targeted libraries from task 02
**Expected**: Most libraries already multi-targeted; may need to add net10.0 to any MvcWeb-specific dependencies if present

**Done when**: All MvcWeb dependency libraries target net8.0;net9.0;net10.0, solution builds successfully
