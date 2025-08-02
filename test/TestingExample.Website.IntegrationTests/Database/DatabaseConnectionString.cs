namespace TestingExample.Website.IntegrationTests.Database;

public record DatabaseConnectionString(string ConnectionString, string Provider)
{
    public static DatabaseConnectionString CreateSqlServer(string connectionString)
        => new(connectionString, "Microsoft.Data.SqlClient");

    public static DatabaseConnectionString CreateSqlite(string connectionString)
        => new(connectionString, "Microsoft.Data.Sqlite");
}
