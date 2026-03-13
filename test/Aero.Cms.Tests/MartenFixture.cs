using Marten;
using Npgsql;
using System.Threading;

namespace Aero.Cms.Tests;


/// <summary>
/// Concrete xUnit fixture that owns the Marten DocumentStore lifetime.
/// Registered via IClassFixture&lt;MartenFixture&gt; — initializes once per test class.
/// </summary>
public class MartenFixture : IAsyncLifetime
{
    private readonly string _dbName = $"aero-test-{Guid.NewGuid():N}";

    private const string MainConnString =
        "Host=localhost;Database=postgres;Username=postgres;Password=*strongPassword1;";

    private string AppConnString =>
        $"Host=localhost;Port=5432;Database={_dbName};Username=postgres;Password=*strongPassword1;";

    public IDocumentStore Store { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await EnsureDatabaseExistsAsync(MainConnString, _dbName);

        Store = DocumentStore.For(opts =>
        {
            opts.Connection(AppConnString);
            opts.AutoCreateSchemaObjects = AutoCreate.All;
        });

        await Store.Storage.ApplyAllConfiguredChangesToDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        if (Store is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();
        else
            Store.Dispose();

        await DropDatabaseAsync(MainConnString, _dbName);
    }

    private static readonly SemaphoreSlim DbLock = new SemaphoreSlim(1, 1);

    private static async Task EnsureDatabaseExistsAsync(string mainConnString, string dbName)
    {
        await DbLock.WaitAsync();
        try
        {
            await using var conn = new NpgsqlConnection(mainConnString);
            await conn.OpenAsync();

            await using var checkCmd = new NpgsqlCommand(
                "SELECT 1 FROM pg_database WHERE datname = @dbName", conn);
            checkCmd.Parameters.AddWithValue("dbName", dbName);

            // Database names cannot be parameterized — extract via builder to sanitize
            var safeName = new NpgsqlConnectionStringBuilder { Database = dbName }.Database;
            var exists = await checkCmd.ExecuteScalarAsync() != null;

            if (!exists)
            {
                await using var createCmd = new NpgsqlCommand(
                    $"CREATE DATABASE \"{safeName}\" WITH OWNER postgres ENCODING 'UTF-8'", conn);

                await createCmd.ExecuteNonQueryAsync();
            }
        }
        finally
        {
            DbLock.Release();
        }
    }

    private static async Task DropDatabaseAsync(string mainConnString, string dbName)
    {
        await DbLock.WaitAsync();
        try
        {
            await using var conn = new NpgsqlConnection(mainConnString);
            await conn.OpenAsync();

            var safeName = new NpgsqlConnectionStringBuilder { Database = dbName }.Database;
            
            await using var dropCmd = new NpgsqlCommand(
                $"DROP DATABASE IF EXISTS \"{safeName}\" WITH (FORCE)", conn);
            await dropCmd.ExecuteNonQueryAsync();
        }
        catch
        {
            // Ignore errors during cleanup
        }
        finally
        {
            DbLock.Release();
        }
    }
}