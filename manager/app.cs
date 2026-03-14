using System.Runtime.InteropServices;
using Docker.DotNet;
using Docker.DotNet.Models;
using Npgsql;

// --- Constants ---
const string HealthcheckFile = "/healthcheck/manager-ready";

// --- Configuration ---
string citusHost = Environment.GetEnvironmentVariable("CITUS_HOST") ?? "master";
string postgresPass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "";
string postgresUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
string postgresDb = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? postgresUser;
string dockerUri = "unix:///var/run/docker.sock";

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    dockerUri = "npipe://./pipe/docker_engine";
}

// Ensure healthcheck file is removed at start
if (File.Exists(HealthcheckFile)) File.Delete(HealthcheckFile);

Console.WriteLine("Starting Citus Manager (C#)...");

// --- Docker Client ---
using var dockerClient = new DockerClientConfiguration(new Uri(dockerUri)).CreateClient();

// --- Connect to Master ---
NpgsqlConnection? conn = null;
while (conn == null)
{
    try
    {
        var connString = $"Host={citusHost};Username={postgresUser};Password={postgresPass};Database={postgresDb}";
        conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();
        Console.Error.WriteLine($"Connected to {citusHost}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not connect to {citusHost}, trying again in 1 second: {ex.Message}");
        await Task.Delay(1000);
        conn = null;
    }
}

// --- Introspect Compose Project ---
string hostname = Environment.GetEnvironmentVariable("HOSTNAME") ?? Environment.MachineName.ToLower();
var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true });
// Find our container by hostname/ID prefix
var thisContainer = containers.FirstOrDefault(c => c.ID.StartsWith(hostname) || (c.Names?.Any(n => n.Contains(hostname)) ?? false));

if (thisContainer == null)
{
    Console.Error.WriteLine($"Could not find own container with hostname: {hostname}. Falling back to list all containers in compose project to find labels.");
    // In some environments, we might need a different way to find our own labels, 
    // but for now we assume we can find ourselves.
}

string? composeProject = null;
if (thisContainer?.Labels.TryGetValue("com.docker.compose.project", out composeProject) != true)
{
    Console.Error.WriteLine("Could not find own container's compose project label. Monitoring all Citus workers.");
}
else
{
    Console.Error.WriteLine($"Found compose project: {composeProject}");
}

// --- Event Listener ---
var progress = new Progress<Message>(async message =>
{
    if (message.Actor?.Attributes == null) return;
    
    var workerName = message.Actor.Attributes.GetValueOrDefault("name");
    if (string.IsNullOrEmpty(workerName)) return;

    try
    {
        if (message.Status == "health_status: healthy")
        {
            await AddWorker(conn, workerName);
        }
        else if (message.Status == "destroy")
        {
            await RemoveWorker(conn, workerName);
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error processing event {message.Status} for {workerName}: {ex.Message}");
    }
});

var filters = new Dictionary<string, IDictionary<string, bool>>
{
    ["event"] = new Dictionary<string, bool> { ["health_status: healthy"] = true, ["destroy"] = true },
    ["label"] = new Dictionary<string, bool>
    {
        ["com.citusdata.role=Worker"] = true
    },
    ["type"] = new Dictionary<string, bool> { ["container"] = true }
};

if (composeProject != null)
{
    filters["label"].Add($"com.docker.compose.project={composeProject}", true);
}

// Subscribe to events
using var cts = new CancellationTokenSource();
var eventTask = dockerClient.System.MonitorEventsAsync(new ContainerEventsParameters { Filters = filters }, progress, cts.Token);

// Handle Shutdown
var tcs = new TaskCompletionSource();
PosixSignalRegistration.Create(PosixSignal.SIGTERM, context =>
{
    Console.Error.WriteLine("Shutting down...");
    cts.Cancel();
    tcs.TrySetResult();
});

// Signal readiness
await File.WriteAllTextAsync(HealthcheckFile, DateTime.UtcNow.ToString("O"));
Console.Error.WriteLine("Listening for events...");

// Wait for events or cancellation
await Task.WhenAny(eventTask, tcs.Task);

async Task AddWorker(NpgsqlConnection conn, string host)
{
    Console.Error.WriteLine($"Adding {host}");
    try 
    {
        using var cmd = new NpgsqlCommand("SELECT master_add_node($1, $2)", conn);
        cmd.Parameters.AddWithValue(host);
        cmd.Parameters.AddWithValue(5432);
        await cmd.ExecuteScalarAsync();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Failed to add node {host}: {ex.Message}");
    }
}

async Task RemoveWorker(NpgsqlConnection conn, string host)
{
    Console.Error.WriteLine($"Removing {host}");
    try 
    {
        using var batch = new NpgsqlBatch(conn);
        batch.BatchCommands.Add(new NpgsqlBatchCommand("""
            DELETE FROM pg_dist_placement 
            WHERE groupid = (SELECT groupid FROM pg_dist_node WHERE nodename = $1 AND nodeport = $2 LIMIT 1)
            """)
        {
            Parameters = { new NpgsqlParameter { Value = host }, new NpgsqlParameter { Value = 5432 } }
        });
        batch.BatchCommands.Add(new NpgsqlBatchCommand("SELECT master_remove_node($1, $2)")
        {
            Parameters = { new NpgsqlParameter { Value = host }, new NpgsqlParameter { Value = 5432 } }
        });
        
        await batch.ExecuteNonQueryAsync();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Failed to remove node {host}: {ex.Message}");
    }
}
