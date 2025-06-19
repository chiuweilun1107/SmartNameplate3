namespace SmartNameplate.Api.Data;

public enum DatabaseProvider
{
    PostgreSQL,
    SqlServer,
    Sqlite
}

public static class DatabaseProviderExtensions
{
    public static string GetJsonColumnType(this DatabaseProvider provider)
    {
        return provider switch
        {
            DatabaseProvider.PostgreSQL => "jsonb",
            DatabaseProvider.SqlServer => "nvarchar(max)",
            DatabaseProvider.Sqlite => "TEXT",
            _ => throw new ArgumentOutOfRangeException(nameof(provider))
        };
    }

    public static string GetTextColumnType(this DatabaseProvider provider)
    {
        return provider switch
        {
            DatabaseProvider.PostgreSQL => "text",
            DatabaseProvider.SqlServer => "nvarchar(max)",
            DatabaseProvider.Sqlite => "TEXT",
            _ => throw new ArgumentOutOfRangeException(nameof(provider))
        };
    }

    public static string GetTimestampColumnType(this DatabaseProvider provider)
    {
        return provider switch
        {
            DatabaseProvider.PostgreSQL => "timestamp with time zone",
            DatabaseProvider.SqlServer => "datetime2",
            DatabaseProvider.Sqlite => "TEXT",
            _ => throw new ArgumentOutOfRangeException(nameof(provider))
        };
    }
} 