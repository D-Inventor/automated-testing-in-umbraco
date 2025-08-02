using System.Globalization;
using System.Text;
using Microsoft.Data.Sqlite;

namespace TestingExample.Website.IntegrationTests.Database;

internal sealed class SqliteDatabase : IDatabaseResource
{
    private const string _inMemoryConnectionStringTemplate = "Data Source={0};Mode=Memory;Cache=Shared";
    private static readonly CompositeFormat _inMemoryConnectionStringFormat = CompositeFormat.Parse(_inMemoryConnectionStringTemplate);
    private readonly string _inMemoryConnectionString;
    private readonly SqliteConnection _imConnection;

    public SqliteDatabase()
    {
        // Shared in-memory databases get destroyed when the last connection is closed.
        // Keeping a connection open while this web application is used, ensures that the database does not get destroyed in the middle of a test.
        // Use a guid as name for the database to make sure that we always have a unique database
        //    and we don't accidentally share.
        _inMemoryConnectionString = string.Format(CultureInfo.InvariantCulture, _inMemoryConnectionStringFormat, Guid.NewGuid());
        _imConnection = new SqliteConnection(_inMemoryConnectionString);
    }

    public DatabaseConnectionString GetConnectionString()
        => DatabaseConnectionString.CreateSqlite(_inMemoryConnectionString);

    public ValueTask InitializeAsync() => new(_imConnection.OpenAsync());

    public async ValueTask DisposeAsync()
    {
        await _imConnection.CloseAsync();
        await _imConnection.DisposeAsync();
    }
}
