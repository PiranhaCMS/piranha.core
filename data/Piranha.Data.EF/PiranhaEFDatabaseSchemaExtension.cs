/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
/// <summary>
/// A database context extension used to store a custom database schema. 
/// The schema set will be used as the default schema in the database context that this extension is configured.
/// Used by the <see cref="PiranhaEFExtensions.UseCustomDatabaseSchema(DbContextOptionsBuilder, string)"/> extension method.
/// </summary>
internal class PiranhaEFDatabaseSchemaExtension : IDbContextOptionsExtension
{
    public PiranhaEFDatabaseSchemaExtension(string schema) => SetSchema(schema);

    /// <summary>
    /// Sets the schema.
    /// </summary>
    /// <param name="schema">The schema to use.</param>
    /// <exception cref="ArgumentException">When schema is null or an empty string.</exception>
    public void SetSchema(string schema)
    {
        if (string.IsNullOrWhiteSpace(schema))
        {
            throw new ArgumentException("Schema must be a non-empty string", nameof(schema));
        }
        Schema = schema;
    }

    private PiranhaEFDarabaseSchemaExtensionInfo _info;

    public DbContextOptionsExtensionInfo Info => _info ??= new PiranhaEFDarabaseSchemaExtensionInfo(this);

    public string Schema { get; private set; }

    public void ApplyServices(IServiceCollection services)
    {
    }

    public void Validate(IDbContextOptions options)
    {
    }
}

/// <summary>
/// The database schema extension info.
/// Used by <see cref="PiranhaEFDatabaseSchemaExtension"/> extension.
/// </summary>
internal class PiranhaEFDarabaseSchemaExtensionInfo : DbContextOptionsExtensionInfo
{
    public PiranhaEFDarabaseSchemaExtensionInfo(IDbContextOptionsExtension extension) : base(extension)
    {
    }

    public override bool IsDatabaseProvider { get; } = false;
    public override string LogFragment { get; } = string.Empty;

    public override int GetServiceProviderHashCode() => 0;

    public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
    {
    }

    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => true;
}