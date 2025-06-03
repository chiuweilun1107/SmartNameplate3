using Microsoft.EntityFrameworkCore;
using SmartNameplate.Api.Data;
using SmartNameplate.Api.Services;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using AutoMapper;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// IIS 集成配置
builder.Services.Configure<IISServerOptions>(options =>
{
    options.AutomaticAuthentication = false;
});

// 支援反向代理配置
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                              Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
});

// Configure URLs
// 使用命令行參數或默認端口
// builder.WebHost.UseUrls("http://localhost:5002");

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Configure Entity Framework with multi-provider support
builder.Services.ConfigureDatabase(builder.Configuration);

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Add custom services
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<IBackgroundImageService, BackgroundImageService>();
builder.Services.AddScoped<IElementImageService, ElementImageService>();
builder.Services.AddScoped<ICardTextElementService, CardTextElementService>();
builder.Services.AddScoped<IUserService, UserService>();
// 添加藍牙服務 - 使用原生 .NET 實現（不依賴外部庫）
builder.Services.AddScoped<IBluetoothService, NativeBluetoothService>();

// 🛡️ 安全服務
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();

// 🔐 JWT 認證配置
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
                       ?? builder.Configuration["JwtSettings:SecretKey"];
        
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT SecretKey not configured");
        }

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") 
                         ?? builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") 
                           ?? builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true
        };
    });

builder.Services.AddAuthorization();

// 🚦 速率限制 (簡化版本，避免套件相容性問題)
// 在生產環境中建議使用 IIS 或反向代理的速率限制功能

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // 開發環境：允許本地端口
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200", "http://localhost:4203", "https://localhost:4203", "http://localhost:56403")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            // 生產環境：設定實際域名
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
                               ?? new[] { "https://yourdomain.com" };
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() {
        Title = "智慧桌牌系統 API",
        Version = "v1",
        Description = "智慧桌牌系統後端 API"
    });
});

var app = builder.Build();

// 支援反向代理標頭
app.UseForwardedHeaders();

// 靜態檔案支援（Angular SPA）
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "智慧桌牌系統 API v1");
        c.RoutePrefix = "api";
    });
}
else
{
    // 生產環境錯誤處理
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

// 🔐 認證和授權
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SPA 回退路由（Angular）
if (!app.Environment.IsDevelopment())
{
    app.MapFallbackToFile("index.html");
}

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("正在初始化資料庫...");
        Console.WriteLine("正在初始化資料庫...");

        // 檢查資料庫連接
        logger.LogInformation("檢查資料庫連接...");
        Console.WriteLine("檢查資料庫連接...");
        var canConnect = context.Database.CanConnect();
        logger.LogInformation($"資料庫連接狀態: {(canConnect ? "✅ 成功" : "❌ 失敗")}");
        Console.WriteLine($"資料庫連接狀態: {(canConnect ? "✅ 成功" : "❌ 失敗")}");

        if (canConnect)
        {
            // 開發和測試環境：直接建立資料庫
            logger.LogInformation("建立資料庫結構...");
            Console.WriteLine("建立資料庫結構...");
            var created = context.Database.EnsureCreated();
            if (created)
            {
                logger.LogInformation("資料庫已建立");
                Console.WriteLine("資料庫已建立");
            }
            else
            {
                logger.LogInformation("資料庫已存在");
                Console.WriteLine("資料庫已存在");
            }
        }
        else
        {
            logger.LogError("無法連接到資料庫，請檢查連接字串設定");
            Console.WriteLine("無法連接到資料庫，請檢查連接字串設定");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "資料庫初始化失敗: {ErrorMessage}", ex.Message);
        Console.WriteLine($"資料庫初始化失敗: {ex.Message}");
        throw;
    }
}

Console.WriteLine("正在啟動 API 服務器...");
Console.WriteLine("智慧桌牌系統 API 已啟動");
Console.WriteLine("API 文檔位於: https://localhost:5001/api");
Console.WriteLine("Swagger UI: https://localhost:5001/api");

app.Run();