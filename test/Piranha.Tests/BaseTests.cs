/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.TestDriver;

namespace Piranha.Tests;

/// <summary>
/// Base class for using the api.
/// </summary>
public abstract class BaseTests : RavenTestDriver, IDisposable
{
    protected IStorage storage = new Local.FileStorage("uploads/", "~/uploads/");
    protected IServiceProvider services = new ServiceCollection()
        .BuildServiceProvider();

    protected IDocumentStore _store;
    protected IAsyncDocumentSession _session;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public BaseTests() {
        _store = GetDocumentStore();
        _session = _store.OpenAsyncSession();
        Init();
    }

    /// <summary>
    /// Disposes the test class.
    /// </summary>
    public new void Dispose() {
        _session.Dispose();
        _store.Dispose();
        Cleanup();
    }

    /// <summary>
    /// Sets up & initializes the tests.
    /// </summary>
    protected abstract void Init();

    /// <summary>
    /// Cleans up any possible data and resources
    /// created by the test.
    /// </summary>
    protected abstract void Cleanup();

    /// <summary>
    /// Gets the test context.
    /// </summary>
    protected IDb GetDb() {
        return new TestDb(_session);
    }
}
