using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace TestingExample.Website.IntegrationTests.Database;

internal sealed class SqlServerDatabase : IDatabaseResource
{
    private readonly MsSqlContainer _dbContainer;
    private string? _connectionString;

    public SqlServerDatabase()
    {
        _dbContainer = new MsSqlBuilder(image: "mcr.microsoft.com/mssql/server:2022-latest")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .Build();
    }

    public DatabaseConnectionString GetConnectionString()
        => DatabaseConnectionString.CreateSqlServer(_connectionString
        ?? throw new InvalidOperationException("Cannot get connectionstring before the database is started"));

    public async ValueTask InitializeAsync()
    {
        await _dbContainer.StartAsync();

        _connectionString = _dbContainer.GetConnectionString();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }
}
