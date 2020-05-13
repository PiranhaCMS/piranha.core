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
dotnet pack core/Piranha/Piranha.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack core/Piranha.AspNetCore/Piranha.AspNetCore.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack core/Piranha.AspNetCore.Identity/Piranha.AspNetCore.Identity.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack core/Piranha.AspNetCore.Identity.SQLite/Piranha.AspNetCore.Identity.SQLite.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack core/Piranha.AspNetCore.Identity.SQLServer/Piranha.AspNetCore.Identity.SQLServer.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack core/Piranha.AspNetCore.SimpleSecurity/Piranha.AspNetCore.SimpleSecurity.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack core/Piranha.AttributeBuilder/Piranha.AttributeBuilder.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack core/Piranha.Azure.BlobStorage/Piranha.Azure.BlobStorage.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack core/Piranha.ImageSharp/Piranha.ImageSharp.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack core/Piranha.Local.FileStorage/Piranha.Local.FileStorage.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack core/Piranha.Manager/Piranha.Manager.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack core/Piranha.Manager.Localization/Piranha.Manager.Localization.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack core/Piranha.Manager.Summernote/Piranha.Manager.Summernote.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack core/Piranha.Manager.TinyMCE/Piranha.Manager.TinyMCE.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack core/Piranha.WebApi/Piranha.WebApi.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack data/Piranha.Data.EF/Piranha.Data.EF.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack data/Piranha.Data.EF.MySql/Piranha.Data.EF.MySql.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack data/Piranha.Data.EF.PostgreSql/Piranha.Data.EF.PostgreSql.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack data/Piranha.Data.EF.SQLite/Piranha.Data.EF.SQLite.csproj --no-build -c Release -o $OUTPUT_DIR
dotnet pack data/Piranha.Data.EF.SQLServer/Piranha.Data.EF.SQLServer.csproj --no-build -c Release -o $OUTPUT_DIR
