using Microsoft.EntityFrameworkCore;

namespace SmartNameplate.Api.Data;

public class DatabaseConfiguration
{
    public DatabaseProvider Provider { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public bool UseNeon { get; set; }
    public bool UseSqlServer { get; set; }
}

public static class DatabaseConfigurationExtensions
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConfig = new DatabaseConfiguration();
        
        // 讀取配置
        var useSqlServer = configuration.GetValue<bool>("Database:UseSqlServer");
        var useNeon = configuration.GetValue<bool>("Database:UseNeon");
        
        // 確定資料庫提供者
        if (useSqlServer)
        {
            databaseConfig.Provider = DatabaseProvider.SqlServer;
            databaseConfig.ConnectionString = configuration.GetConnectionString("SqlServerConnection") 
                ?? configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("SQL Server 連接字串未設置");
        }
        else
        {
            databaseConfig.Provider = DatabaseProvider.PostgreSQL;
            databaseConfig.ConnectionString = useNeon
                ? configuration.GetConnectionString("NeonConnection")
                : configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(databaseConfig.ConnectionString))
            {
                throw new InvalidOperationException("PostgreSQL 連接字串未設置");
            }
        }
        
        databaseConfig.UseNeon = useNeon;
        databaseConfig.UseSqlServer = useSqlServer;
        
        // 註冊配置
        services.AddSingleton(databaseConfig);
        
        // 配置 DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            switch (databaseConfig.Provider)
            {
                case DatabaseProvider.PostgreSQL:
                    options.UseNpgsql(databaseConfig.ConnectionString);
                    break;
                case DatabaseProvider.SqlServer:
                    options.UseSqlServer(databaseConfig.ConnectionString);
                    break;
                default:
                    throw new InvalidOperationException($"不支援的資料庫提供者: {databaseConfig.Provider}");
            }
        });
    }
} 