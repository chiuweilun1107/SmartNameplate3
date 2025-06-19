using Microsoft.EntityFrameworkCore;

namespace SmartNameplate.Api.Data;

public class DatabaseConfiguration
{
    public DatabaseProvider Provider { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
}

public static class DatabaseConfigurationExtensions
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConfig = new DatabaseConfiguration();
        
        // 讀取資料庫提供者設定
        var providerName = configuration.GetValue<string>("Database:Provider") ?? "PostgreSQL";
        
        // 確定資料庫提供者和連接字串
        switch (providerName.ToUpperInvariant())
        {
            case "POSTGRESQL":
            case "POSTGRES":
                databaseConfig.Provider = DatabaseProvider.PostgreSQL;
                databaseConfig.ConnectionString = configuration.GetConnectionString("PostgreSQLConnection") 
                    ?? configuration.GetConnectionString("DefaultConnection") 
                    ?? throw new InvalidOperationException("PostgreSQL 連接字串未設置");
                break;
                
            case "SQLSERVER":
            case "SQL SERVER":
                databaseConfig.Provider = DatabaseProvider.SqlServer;
                databaseConfig.ConnectionString = configuration.GetConnectionString("SqlServerConnection") 
                    ?? throw new InvalidOperationException("SQL Server 連接字串未設置");
                break;
                
            case "SQLITE":
                databaseConfig.Provider = DatabaseProvider.Sqlite;
                databaseConfig.ConnectionString = configuration.GetConnectionString("SqliteConnection") 
                    ?? "Data Source=smartnameplate.db";
                break;
                
            default:
                throw new InvalidOperationException($"不支援的資料庫提供者: {providerName}。支援的提供者: PostgreSQL, SqlServer, Sqlite");
        }
        
        // 註冊配置
        services.AddSingleton(databaseConfig);
        
        // 配置 DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            switch (databaseConfig.Provider)
            {
                case DatabaseProvider.PostgreSQL:
                    options.UseNpgsql(databaseConfig.ConnectionString, npgsqlOptions =>
                    {
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorCodesToAdd: null);
                    });
                    break;
                    
                case DatabaseProvider.SqlServer:
                    options.UseSqlServer(databaseConfig.ConnectionString, sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    });
                    break;
                    
                case DatabaseProvider.Sqlite:
                    options.UseSqlite(databaseConfig.ConnectionString);
                    break;
                    
                default:
                    throw new InvalidOperationException($"不支援的資料庫提供者: {databaseConfig.Provider}");
            }
            
            // 開發環境啟用敏感資料記錄
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });
    }
} 