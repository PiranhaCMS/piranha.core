#!/usr/bin/env bash

# Clean and build in release
dotnet restore
dotnet clean
dotnet build -c Release

# Create all NuGet packages
dotnet pack core/Piranha/Piranha.csproj --no-build -c Release -o ./artifacts
dotnet pack core/Piranha.AspNetCore/Piranha.AspNetCore.csproj --no-build -c Release -o ./artifacts
dotnet pack core/Piranha.AspNetCore.Hosting/Piranha.AspNetCore.Hosting.csproj --no-build -c Release -o ./artifacts
dotnet pack core/Piranha.AttributeBuilder/Piranha.AttributeBuilder.csproj --no-build -c Release -o ./artifacts
dotnet pack core/Piranha.Azure.BlobStorage/Piranha.Azure.BlobStorage.csproj --no-build -c Release -o ./artifacts
dotnet pack core/Piranha.ImageSharp/Piranha.ImageSharp.csproj --no-build -c Release -o ./artifacts
dotnet pack core/Piranha.Local.FileStorage/Piranha.Local.FileStorage.csproj --no-build -c Release -o ./artifacts
dotnet pack core/Piranha.Manager/Piranha.Manager.csproj --no-build -c Release -o ./artifacts
dotnet pack core/Piranha.Manager.LocalAuth/Piranha.Manager.LocalAuth.csproj --no-build -c Release -o ./artifacts
dotnet pack core/Piranha.Manager.Localization/Piranha.Manager.Localization.csproj --no-build -c Release -o ./artifacts
dotnet pack core/Piranha.Manager.TinyMCE/Piranha.Manager.TinyMCE.csproj --no-build -c Release -o ./artifacts
dotnet pack core/Piranha.WebApi/Piranha.WebApi.csproj --no-build -c Release -o ./artifacts
dotnet pack data/Piranha.Data.EF/Piranha.Data.EF.csproj --no-build -c Release -o ./artifacts
dotnet pack data/Piranha.Data.EF.MySql/Piranha.Data.EF.MySql.csproj --no-build -c Release -o ./artifacts
dotnet pack data/Piranha.Data.EF.PostgreSql/Piranha.Data.EF.PostgreSql.csproj --no-build -c Release -o ./artifacts
dotnet pack data/Piranha.Data.EF.SQLite/Piranha.Data.EF.SQLite.csproj --no-build -c Release -o ./artifacts
dotnet pack data/Piranha.Data.EF.SQLServer/Piranha.Data.EF.SQLServer.csproj --no-build -c Release -o ./artifacts
dotnet pack identity/Piranha.AspNetCore.Identity/Piranha.AspNetCore.Identity.csproj --no-build -c Release -o ./artifacts
dotnet pack identity/Piranha.AspNetCore.Identity.MySQL/Piranha.AspNetCore.Identity.MySQL.csproj --no-build -c Release -o ./artifacts
dotnet pack identity/Piranha.AspNetCore.Identity.PostgreSQL/Piranha.AspNetCore.Identity.PostgreSQL.csproj --no-build -c Release -o ./artifacts
dotnet pack identity/Piranha.AspNetCore.Identity.SQLite/Piranha.AspNetCore.Identity.SQLite.csproj --no-build -c Release -o ./artifacts
dotnet pack identity/Piranha.AspNetCore.Identity.SQLServer/Piranha.AspNetCore.Identity.SQLServer.csproj --no-build -c Release -o ./artifacts
