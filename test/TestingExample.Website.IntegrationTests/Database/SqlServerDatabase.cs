using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace TestingExample.Website.IntegrationTests.Database;

internal sealed class SqlServerDatabase : IDatabaseResource
{
    private readonly MsSqlContainer _dbContainer;
    private string? _connectionString;

    public SqlServerDatabase()
    {
        _dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .Build();
    }

    public DatabaseConnectionString GetConnectionString()
        => DatabaseConnectionString.CreateSqlServer(_connectionString
        ?? throw new InvalidOperationException("Cannot get connectionstring before the database is started"));

    public async ValueTask InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // NOTE: entity framework cannot properly clean up all the tables on the default connection string
        // This connection string connects to a 'different' database on which ef core can do everything that it needs 
        _connectionString = new SqlConnectionStringBuilder(_dbContainer.GetConnectionString())
        {
            InitialCatalog = Guid.NewGuid().ToString("D")
        }.ToString();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }
}
