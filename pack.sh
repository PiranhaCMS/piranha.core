#!/usr/bin/env bash

# Define directories
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
OUTPUT_DIR=$SCRIPT_DIR/artifacts

# Clean and build in release
dotnet clean
dotnet build -c Release

# Make sure output folder exist.
if [ ! -d "$OUTPUT_DIR" ]; then
  mkdir "$OUTPUT_DIR"
fi

# Create all NuGet packages
nuget pack nuspec/Piranha.AspNetCore.Identity.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.AspNetCore.Identity.SQLite.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.AspNetCore.Identity.SQLServer.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.AspNetCore.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.AspNetCore.SimpleSecurity.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.AttributeBuilder.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.Azure.BlobStorage.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.Data.EF.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.Data.EF.MySql.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.Data.EF.PostgreSql.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.Data.EF.SQLite.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.Data.EF.SQLServer.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.Extensions.Sync.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.ImageSharp.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.Local.FileStorage.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.Manager.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.Manager.Localization.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.Manager.Summernote.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.Manager.TinyMCE.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.nuspec -OutputDirectory $OUTPUT_DIR
nuget pack nuspec/Piranha.WebApi.nuspec -OutputDirectory $OUTPUT_DIR
