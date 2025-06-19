# ASP.NET Core 架構指南與程式規範

## 🎯 概述

本文件基於 Microsoft 官方文檔，提供 ASP.NET Core 應用程式的架構指南、程式規範和最佳實踐。

## 🏛️ SOLID 設計原則

### 📋 SOLID 原則概述

SOLID 是物件導向設計的五個基本原則，確保程式碼的可維護性、可擴展性和可測試性：

#### 🔸 S - 單一職責原則 (Single Responsibility Principle)
每個類別應該只有一個改變的理由，專注於單一職責。

```csharp
// ❌ 違反 SRP：一個類別處理多種職責
public class UserManager
{
    public void CreateUser(User user) { /* 建立使用者 */ }
    public void SendEmail(string email) { /* 發送郵件 */ }
    public void LogActivity(string message) { /* 記錄日誌 */ }
}

// ✅ 遵循 SRP：職責分離
public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IEmailService emailService,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        _logger.LogInformation("開始建立使用者：{Email}", createUserDto.Email);
        
        var user = new User
        {
            Email = createUserDto.Email,
            Name = createUserDto.Name
        };

        var createdUser = await _userRepository.AddAsync(user);
        await _emailService.SendWelcomeEmailAsync(user.Email);
        
        _logger.LogInformation("使用者建立成功：{UserId}", createdUser.Id);
        return MapToDto(createdUser);
    }
}
```

#### 🔸 O - 開放封閉原則 (Open/Closed Principle)
類別應該對擴展開放，對修改封閉。

```csharp
// ✅ 使用策略模式實現 OCP
public interface INotificationStrategy
{
    Task SendAsync(string message, string recipient);
}

public class EmailNotificationStrategy : INotificationStrategy
{
    public async Task SendAsync(string message, string recipient)
    {
        // 電子郵件發送邏輯
    }
}

public class SmsNotificationStrategy : INotificationStrategy
{
    public async Task SendAsync(string message, string recipient)
    {
        // 簡訊發送邏輯
    }
}

public class NotificationService
{
    private readonly INotificationStrategy _notificationStrategy;

    public NotificationService(INotificationStrategy notificationStrategy)
    {
        _notificationStrategy = notificationStrategy;
    }

    public async Task NotifyAsync(string message, string recipient)
    {
        await _notificationStrategy.SendAsync(message, recipient);
    }
}
```

#### 🔸 L - 里氏替換原則 (Liskov Substitution Principle)
子類別應該能夠替換其基類別而不影響程式的正確性。

```csharp
// ✅ 正確的 LSP 實現
public abstract class PaymentProcessor
{
    public abstract Task<PaymentResult> ProcessAsync(decimal amount);
    
    protected virtual bool ValidateAmount(decimal amount)
    {
        return amount > 0;
    }
}

public class CreditCardProcessor : PaymentProcessor
{
    public override async Task<PaymentResult> ProcessAsync(decimal amount)
    {
        if (!ValidateAmount(amount))
            throw new ArgumentException("金額必須大於零");
            
        // 信用卡處理邏輯
        return new PaymentResult { Success = true };
    }
}

public class PayPalProcessor : PaymentProcessor
{
    public override async Task<PaymentResult> ProcessAsync(decimal amount)
    {
        if (!ValidateAmount(amount))
            throw new ArgumentException("金額必須大於零");
            
        // PayPal 處理邏輯
        return new PaymentResult { Success = true };
    }
}
```

#### 🔸 I - 介面隔離原則 (Interface Segregation Principle)
不應該強迫客戶端依賴它們不使用的介面。

```csharp
// ❌ 違反 ISP：介面過於龐大
public interface IUserOperations
{
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
    Task SendEmailAsync(string email);
    Task GenerateReportAsync();
    Task BackupDataAsync();
}

// ✅ 遵循 ISP：介面分離
public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(int id);
    Task<User> GetByIdAsync(int id);
}

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string body);
}

public interface IReportService
{
    Task<Report> GenerateUserReportAsync();
}

public interface IBackupService
{
    Task BackupUsersAsync();
}
```

#### 🔸 D - 依賴反轉原則 (Dependency Inversion Principle)
高層模組不應該依賴低層模組，兩者都應該依賴抽象。

```csharp
// ❌ 違反 DIP：直接依賴具體實現
public class OrderService
{
    private readonly SqlServerRepository _repository; // 直接依賴具體實現
    private readonly SmtpEmailService _emailService; // 直接依賴具體實現

    public OrderService()
    {
        _repository = new SqlServerRepository(); // 緊耦合
        _emailService = new SmtpEmailService(); // 緊耦合
    }
}

// ✅ 遵循 DIP：依賴抽象
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IEmailService emailService,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<ApiResponse<OrderDto>> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        try
        {
            _logger.LogInformation("開始建立訂單");
            
            var order = new Order
            {
                CustomerEmail = createOrderDto.CustomerEmail,
                TotalAmount = createOrderDto.TotalAmount
            };

            var createdOrder = await _orderRepository.AddAsync(order);
            await _emailService.SendOrderConfirmationAsync(order.CustomerEmail, order.Id);
            
            return ApiResponse<OrderDto>.Success(MapToDto(createdOrder), "訂單建立成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立訂單時發生錯誤");
            return ApiResponse<OrderDto>.Error("建立訂單失敗");
        }
    }
}
```

## 📦 NuGet 套件風格統一說明

### 🎯 標配套件清單

#### 核心框架套件
```xml
<!-- ASP.NET Core 基礎套件 -->
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.8.1" />

<!-- 資料存取套件 -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />

<!-- 認證授權套件 -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
```

#### 🏗️ 架構模式套件（標配）
```xml
<!-- CQRS 與 Mediator 模式套件 -->
<PackageReference Include="MediatR" Version="12.2.0" />
<PackageReference Include="MediatR.Extensions.Microsoft.DependencyInjection" Version="11.1.0" />

<!-- 驗證框架套件 -->
<PackageReference Include="FluentValidation" Version="11.10.0" />
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.0" />

<!-- 物件映射套件 -->
<PackageReference Include="AutoMapper" Version="12.0.1" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />

<!-- 結果模式套件 -->
<PackageReference Include="FluentResults" Version="3.15.2" />
```

#### 📝 日誌與監控套件
```xml
<!-- 結構化日誌套件 -->
<PackageReference Include="Serilog.AspNetCore" Version="8.0.1" />
<PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
<PackageReference Include="Serilog.Sinks.File" Version="6.0.0" />
<PackageReference Include="Serilog.Sinks.Seq" Version="7.0.1" />

<!-- 健康檢查套件 -->
<PackageReference Include="Microsoft.Extensions.Diagnostics.HealthChecks" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore" Version="8.0.0" />
```

#### 🔒 安全性套件
```xml
<!-- 資料保護與加密套件 -->
<PackageReference Include="HtmlSanitizer" Version="8.0.838" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="Microsoft.AspNetCore.DataProtection" Version="8.0.0" />

<!-- CORS 與安全標頭套件 -->
<PackageReference Include="Microsoft.AspNetCore.Cors" Version="2.2.0" />
```

### 🎨 套件版本管理策略

#### Directory.Build.props 統一版本管理
```xml
<Project>
  <PropertyGroup>
    <!-- .NET 版本設定 -->
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>12.0</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    
    <!-- 套件版本變數定義 -->
    <MicrosoftAspNetCoreVersion>8.0.0</MicrosoftAspNetCoreVersion>
    <EntityFrameworkVersion>8.0.0</EntityFrameworkVersion>
    <MediatRVersion>12.2.0</MediatRVersion>
    <FluentValidationVersion>11.10.0</FluentValidationVersion>
    <SerilogVersion>8.0.1</SerilogVersion>
  </PropertyGroup>

  <!-- 全域套件參考設定 -->
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="$(MicrosoftAspNetCoreVersion)" />
    <PackageReference Include="MediatR" Version="$(MediatRVersion)" />
    <PackageReference Include="FluentValidation" Version="$(FluentValidationVersion)" />
    <PackageReference Include="Serilog.AspNetCore" Version="$(SerilogVersion)" />
  </ItemGroup>
</Project>
```

### 🔧 MediatR + FluentValidation 標準實作

#### ValidationBehavior 管道行為
```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            _logger.LogWarning("驗證失敗，請求類型：{RequestType}，錯誤：{Errors}", 
                typeof(TRequest).Name, 
                string.Join(", ", failures.Select(f => f.ErrorMessage)));

            throw new ValidationException(failures);
        }

        return await next();
    }
}
```

#### 標準驗證器範例
```csharp
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("電子郵件為必填項目")
            .EmailAddress().WithMessage("電子郵件格式不正確")
            .MustAsync(BeUniqueEmail).WithMessage("此電子郵件已被使用");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("姓名為必填項目")
            .Length(2, 50).WithMessage("姓名長度必須在 2-50 字元之間");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密碼為必填項目")
            .MinimumLength(8).WithMessage("密碼至少需要 8 個字元")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("密碼必須包含大小寫字母、數字和特殊字元");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _userRepository.ExistsByEmailAsync(email, cancellationToken);
    }
}
```

#### 服務註冊配置
```csharp
// Program.cs 中的服務註冊
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddBehavior<ValidationBehavior<,>>();
    cfg.AddBehavior<LoggingBehavior<,>>();
    cfg.AddBehavior<PerformanceBehavior<,>>();
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
```

## 📁 標準專案結構

### 🏗️ Web API 專案結構（簡化版）
```
ProjectName/
├── Controllers/              # API 控制器層
├── Services/                # 業務邏輯服務層
│   ├── Interfaces/         # 服務介面定義
│   └── Implementations/    # 服務具體實作
├── Data/                   # 資料存取層
│   ├── Configurations/     # EF Core 實體配置
│   └── Repositories/       # 儲存庫模式實作
├── Models/                 # 資料模型層
│   ├── Entities/          # 資料庫實體類別
│   ├── DTOs/              # 資料傳輸物件
│   └── ViewModels/        # 視圖模型類別
├── Middlewares/           # 自訂中介軟體
├── Filters/               # 動作篩選器
├── Extensions/            # 擴充方法
├── Configurations/        # 配置類別
├── Migrations/           # EF Core 資料庫遷移
├── wwwroot/              # 靜態檔案資源
├── appsettings.json      # 應用程式設定檔
└── Program.cs            # 應用程式進入點
```

### 🌐 Razor Pages 專案結構
```
ProjectName/
├── Pages/                 # Razor 頁面
│   ├── Shared/           # 共用頁面元件
│   ├── _ViewStart.cshtml
│   └── Index.cshtml
├── Areas/                # 區域組織結構
├── Models/               # 頁面模型類別
├── Services/             # 業務服務層
├── Data/                 # 資料存取層
├── wwwroot/              # 靜態資源檔案
└── Program.cs
```

## 🏗️ 專案結構設計

### 📁 Web API 專案完整結構

```
SmartNameplate.Api/
├── 📁 Controllers/              # API 控制器層
│   ├── 📄 BaseController.cs    # 基礎控制器類別
│   ├── 📄 AuthController.cs    # 認證控制器
│   ├── 📄 UsersController.cs   # 使用者管理控制器
│   ├── 📄 CardsController.cs   # 卡片管理控制器
│   └── 📄 DevicesController.cs # 設備管理控制器
├── 📁 Services/                # 業務邏輯層
│   ├── 📁 Interfaces/          # 服務介面定義
│   │   ├── 📄 IUserService.cs
│   │   ├── 📄 ICardService.cs
│   │   ├── 📄 IJwtService.cs
│   │   └── 📄 ISecurityService.cs
│   └── 📁 Implementations/     # 服務具體實作
│       ├── 📄 UserService.cs
│       ├── 📄 CardService.cs
│       ├── 📄 JwtService.cs
│       └── 📄 SecurityService.cs
├── 📁 Data/                    # 資料存取層
│   ├── 📄 ApplicationDbContext.cs
│   ├── 📄 DatabaseConfiguration.cs
│   ├── 📁 Configurations/      # EF Core 實體配置
│   │   ├── 📄 UserConfiguration.cs
│   │   ├── 📄 CardConfiguration.cs
│   │   └── 📄 DeviceConfiguration.cs
│   └── 📁 Repositories/        # 儲存庫模式實作
│       ├── 📁 Interfaces/
│       │   ├── 📄 IRepository.cs
│       │   ├── 📄 IUserRepository.cs
│       │   └── 📄 ICardRepository.cs
│       └── 📁 Implementations/
│           ├── 📄 Repository.cs
│           ├── 📄 UserRepository.cs
│           └── 📄 CardRepository.cs
├── 📁 Models/                  # 資料模型層
│   ├── 📁 Entities/           # 資料庫實體類別
│   │   ├── 📄 User.cs
│   │   ├── 📄 Card.cs
│   │   ├── 📄 Device.cs
│   │   └── 📄 BaseEntity.cs
│   ├── 📁 DTOs/               # 資料傳輸物件
│   │   ├── 📁 Requests/       # 請求 DTO 類別
│   │   │   ├── 📄 CreateUserDto.cs
│   │   │   ├── 📄 UpdateUserDto.cs
│   │   │   └── 📄 LoginDto.cs
│   │   ├── 📁 Responses/      # 回應 DTO 類別
│   │   │   ├── 📄 UserDto.cs
│   │   │   ├── 📄 CardDto.cs
│   │   │   └── 📄 AuthResponseDto.cs
│   │   └── 📁 Common/         # 共用 DTO 類別
│   │       ├── 📄 PagedResultDto.cs
│   │       └── 📄 ApiResponseDto.cs
│   └── 📁 ViewModels/         # 視圖模型類別
│       ├── 📄 DashboardViewModel.cs
│       └── 📄 ReportViewModel.cs
├── 📁 Middlewares/            # 自訂中介軟體
│   ├── 📄 GlobalExceptionMiddleware.cs
│   ├── 📄 RequestLoggingMiddleware.cs
│   └── 📄 SecurityHeadersMiddleware.cs
├── 📁 Filters/                # 動作篩選器
│   ├── 📄 ValidateModelFilter.cs
│   ├── 📄 AuthorizeFilter.cs
│   └── 📄 LoggingFilter.cs
├── 📁 Extensions/             # 擴充方法類別
│   ├── 📄 ServiceCollectionExtensions.cs
│   ├── 📄 ApplicationBuilderExtensions.cs
│   └── 📄 StringExtensions.cs
├── 📁 Configurations/         # 配置類別
│   ├── 📄 JwtSettings.cs
│   ├── 📄 DatabaseSettings.cs
│   ├── 📄 EmailSettings.cs
│   └── 📄 AppSettings.cs
├── 📁 Validators/             # 驗證器類別
│   ├── 📄 CreateUserValidator.cs
│   ├── 📄 UpdateUserValidator.cs
│   └── 📄 LoginValidator.cs
├── 📁 Mappings/               # AutoMapper 配置
│   ├── 📄 UserMappingProfile.cs
│   ├── 📄 CardMappingProfile.cs
│   └── 📄 MappingProfile.cs
├── 📁 Migrations/             # EF Core 資料庫遷移
│   ├── 📄 20240101000000_InitialCreate.cs
│   ├── 📄 20240102000000_AddUserTable.cs
│   └── 📄 ApplicationDbContextModelSnapshot.cs
├── 📁 wwwroot/                # 靜態檔案資源
│   ├── 📁 css/
│   ├── 📁 js/
│   ├── 📁 images/
│   └── 📁 uploads/
├── 📁 Properties/             # 專案屬性設定
│   └── 📄 launchSettings.json
├── 📄 appsettings.json        # 應用程式設定檔
├── 📄 appsettings.Development.json
├── 📄 appsettings.Production.json
├── 📄 Program.cs              # 應用程式進入點
├── 📄 Startup.cs              # 啟動配置（可選）
├── 📄 SmartNameplateApi.csproj # 專案檔案
└── 📄 web.config              # IIS 部署配置
```

## 🚨 統一 API 回應格式與例外處理

### 📋 統一 API 回應格式

#### ApiResponse<T> 基礎類別
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string TraceId { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> Success(T data, string message = "操作成功")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Error(string message, string? errorCode = null, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            Errors = errors ?? new List<string>()
        };
    }

    public static ApiResponse<T> ValidationError(List<string> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = "資料驗證失敗",
            ErrorCode = "VALIDATION_ERROR",
            Errors = errors
        };
    }
}

// 無資料回應的簡化版本
public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Success(string message = "操作成功")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    public new static ApiResponse Error(string message, string? errorCode = null, List<string>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            Errors = errors ?? new List<string>()
        };
    }
}
```

#### 分頁回應格式
```csharp
public class PagedApiResponse<T> : ApiResponse<PagedResult<T>>
{
    public static PagedApiResponse<T> Success(
        IEnumerable<T> items, 
        int totalCount, 
        int page, 
        int pageSize, 
        string message = "查詢成功")
    {
        var pagedResult = new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };

        return new PagedApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = pagedResult
        };
    }
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
```

### 🔧 全域例外處理中介軟體

#### GlobalExceptionMiddleware 實作
```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "發生未處理的例外：{Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        
        var response = exception switch
        {
            ValidationException validationEx => new ApiResponse<object>
            {
                Success = false,
                Message = "資料驗證失敗",
                ErrorCode = "VALIDATION_ERROR",
                Errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList(),
                TraceId = context.TraceIdentifier
            },
            UnauthorizedAccessException => new ApiResponse<object>
            {
                Success = false,
                Message = "未授權的存取",
                ErrorCode = "UNAUTHORIZED",
                TraceId = context.TraceIdentifier
            },
            ArgumentException argEx => new ApiResponse<object>
            {
                Success = false,
                Message = argEx.Message,
                ErrorCode = "INVALID_ARGUMENT",
                TraceId = context.TraceIdentifier
            },
            KeyNotFoundException => new ApiResponse<object>
            {
                Success = false,
                Message = "找不到指定的資源",
                ErrorCode = "NOT_FOUND",
                TraceId = context.TraceIdentifier
            },
            _ => new ApiResponse<object>
            {
                Success = false,
                Message = _environment.IsDevelopment() ? exception.Message : "系統發生錯誤",
                ErrorCode = "INTERNAL_ERROR",
                TraceId = context.TraceIdentifier
            }
        };

        context.Response.StatusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
```

#### 中介軟體註冊
```csharp
// Program.cs 中註冊全域例外處理中介軟體
var app = builder.Build();

// 全域例外處理（必須在最前面）
app.UseMiddleware<GlobalExceptionMiddleware>();

// 其他中介軟體...
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### 🎯 基礎控制器實作

#### BaseController 統一回應格式
```csharp
[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected readonly ILogger<BaseController> _logger;

    protected BaseController(ILogger<BaseController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 回傳成功結果
    /// </summary>
    protected ActionResult<ApiResponse<T>> SuccessResult<T>(T data, string message = "操作成功")
    {
        return Ok(ApiResponse<T>.Success(data, message));
    }

    /// <summary>
    /// 回傳成功結果（無資料）
    /// </summary>
    protected ActionResult<ApiResponse> SuccessResult(string message = "操作成功")
    {
        return Ok(ApiResponse.Success(message));
    }

    /// <summary>
    /// 回傳分頁結果
    /// </summary>
    protected ActionResult<PagedApiResponse<T>> PagedResult<T>(
        IEnumerable<T> items, 
        int totalCount, 
        int page, 
        int pageSize, 
        string message = "查詢成功")
    {
        return Ok(PagedApiResponse<T>.Success(items, totalCount, page, pageSize, message));
    }

    /// <summary>
    /// 回傳錯誤結果
    /// </summary>
    protected ActionResult<ApiResponse<T>> ErrorResult<T>(string message, string? errorCode = null)
    {
        return BadRequest(ApiResponse<T>.Error(message, errorCode));
    }

    /// <summary>
    /// 回傳驗證錯誤結果
    /// </summary>
    protected ActionResult<ApiResponse<T>> ValidationErrorResult<T>(List<string> errors)
    {
        return BadRequest(ApiResponse<T>.ValidationError(errors));
    }

    /// <summary>
    /// 回傳找不到資源結果
    /// </summary>
    protected ActionResult<ApiResponse<T>> NotFoundResult<T>(string message = "找不到指定的資源")
    {
        return NotFound(ApiResponse<T>.Error(message, "NOT_FOUND"));
    }
}
```

### 🔧 標準控制器實作範例

#### UsersController 使用統一格式
```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseController
{
    private readonly IUserService _userService;

    public UsersController(
        IUserService userService,
        ILogger<UsersController> logger) : base(logger)
    {
        _userService = userService;
    }

    /// <summary>
    /// 取得使用者清單
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<UserDto>>> GetUsers(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        var result = await _userService.GetUsersAsync(page, pageSize);
        return PagedResult(result.Items, result.TotalCount, page, pageSize);
    }

    /// <summary>
    /// 根據 ID 取得使用者
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFoundResult<UserDto>("找不到指定的使用者");

        return SuccessResult(user, "使用者資料取得成功");
    }

    /// <summary>
    /// 建立新使用者
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser(CreateUserDto createUserDto)
    {
        var user = await _userService.CreateUserAsync(createUserDto);
        return CreatedAtAction(
            nameof(GetUser), 
            new { id = user.Id }, 
            ApiResponse<UserDto>.Success(user, "使用者建立成功"));
    }

    /// <summary>
    /// 更新使用者資料
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, UpdateUserDto updateUserDto)
    {
        var user = await _userService.UpdateUserAsync(id, updateUserDto);
        if (user == null)
            return NotFoundResult<UserDto>("找不到指定的使用者");

        return SuccessResult(user, "使用者資料更新成功");
    }

    /// <summary>
    /// 刪除使用者
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteUser(int id)
    {
        var success = await _userService.DeleteUserAsync(id);
        if (!success)
            return NotFoundResult<object>("找不到指定的使用者");

        return SuccessResult("使用者刪除成功");
    }
}
```

## 🏛️ 依賴注入架構

### 📋 .NET 8 DI 新特性

#### 🔑 Keyed Services（鍵值服務）
```csharp
// 註冊多個相同介面的不同實作
builder.Services.AddKeyedScoped<INotificationService, EmailNotificationService>("email");
builder.Services.AddKeyedScoped<INotificationService, SmsNotificationService>("sms");
builder.Services.AddKeyedScoped<INotificationService, PushNotificationService>("push");

// 在控制器中使用 FromKeyedServices
[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _emailService;
    private readonly INotificationService _smsService;

    public NotificationController(
        [FromKeyedServices("email")] INotificationService emailService,
        [FromKeyedServices("sms")] INotificationService smsService)
    {
        _emailService = emailService;
        _smsService = smsService;
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmail([FromBody] NotificationRequest request)
    {
        await _emailService.SendAsync(request.Message);
        return Ok(ApiResponse.Success("電子郵件發送成功"));
    }

    [HttpPost("send-sms")]
    public async Task<IActionResult> SendSms([FromBody] NotificationRequest request)
    {
        await _smsService.SendAsync(request.Message);
        return Ok(ApiResponse.Success("簡訊發送成功"));
    }
}
```

#### 🔧 主要建構函式 (.NET 8+)
```csharp
// 使用主要建構函式簡化語法
public class UserService(
    IRepository<User> userRepository,
    ILogger<UserService> logger,
    IMapper mapper) : IUserService
{
    private readonly IRepository<User> _userRepository = userRepository;
    private readonly ILogger<UserService> _logger = logger;
    private readonly IMapper _mapper = mapper;

    public async Task<UserDto> GetUserAsync(int id)
    {
        _logger.LogInformation("取得使用者資料：{UserId}", id);
        var user = await _userRepository.GetByIdAsync(id);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        _logger.LogInformation("取得所有使用者資料");
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }
}

// 控制器也可以使用主要建構函式
[ApiController]
[Route("api/[controller]")]
public class UsersController(
    IUserService userService,
    ILogger<UsersController> logger) : BaseController(logger)
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<UserDto>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return PagedResult(users, users.Count(), 1, users.Count());
    }
}
```

#### 📦 多重實作註冊與解析
```csharp
// 註冊多個實作
builder.Services.AddScoped<INotificationService, EmailNotificationService>();
builder.Services.AddScoped<INotificationService, SmsNotificationService>();
builder.Services.AddScoped<INotificationService, PushNotificationService>();

// 解析所有實作
public class NotificationManager(IEnumerable<INotificationService> notificationServices)
{
    private readonly IEnumerable<INotificationService> _notificationServices = notificationServices;

    public async Task SendToAllAsync(string message)
    {
        foreach (var service in _notificationServices)
        {
            await service.SendAsync(message);
        }
    }

    public async Task SendByTypeAsync(string message, string serviceType)
    {
        var service = _notificationServices.FirstOrDefault(s => 
            s.GetType().Name.Contains(serviceType, StringComparison.OrdinalIgnoreCase));
        
        if (service != null)
        {
            await service.SendAsync(message);
        }
    }
}
```

### 📋 服務註冊模式

#### Program.cs 標準配置
```csharp
var builder = WebApplication.CreateBuilder(args);

// 🔧 基礎服務註冊
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🗄️ 資料庫配置
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔐 認證授權配置
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* JWT 配置 */ });
builder.Services.AddAuthorization();

// 📝 日誌配置
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// 🛠️ 應用程式服務註冊
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// 🔒 安全服務註冊
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// 🌐 CORS 配置
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
        policy.WithOrigins("https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

// 🔄 中介軟體管道配置
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### 🔄 服務生命週期

#### Transient（暫時性）
```csharp
// 每次請求都建立新實例
builder.Services.AddTransient<IEmailService, EmailService>();
```

#### Scoped（範圍性）
```csharp
// 每個 HTTP 請求一個實例
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ApplicationDbContext>();
```

#### Singleton（單例）
```csharp
// 應用程式生命週期內單一實例
builder.Services.AddSingleton<IConfiguration>();
builder.Services.AddSingleton<IMemoryCache>();
```

## 🗄️ 資料存取層設計

### 📊 DbContext 配置
```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 實體配置
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // 種子資料
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Email = "admin@example.com", Name = "Admin" }
        );
    }
}
```

### 🏪 儲存庫模式
```csharp
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
```

## 🛡️ 安全性最佳實踐

### 🔐 JWT 認證配置
```csharp
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
        _secretKey = _configuration["JwtSettings:SecretKey"] 
                    ?? throw new InvalidOperationException("JWT SecretKey 未設定");
        _issuer = _configuration["JwtSettings:Issuer"] ?? "DefaultIssuer";
        _audience = _configuration["JwtSettings:Audience"] ?? "DefaultAudience";
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### 🛡️ 輸入驗證與清理
```csharp
public class SecurityService : ISecurityService
{
    private readonly HtmlSanitizer _htmlSanitizer;

    public SecurityService()
    {
        _htmlSanitizer = new HtmlSanitizer();
        _htmlSanitizer.AllowedTags.Clear();
    }

    public string SanitizeHtml(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return _htmlSanitizer.Sanitize(input);
    }

    public bool ValidatePasswordStrength(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
            return false;

        var hasUpper = password.Any(char.IsUpper);
        var hasLower = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }
}
```

## 📝 配置管理

### ⚙️ appsettings.json 結構
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=myapp;Username=postgres;Password=password",
    "SqlServerConnection": "Server=localhost;Database=myapp;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-here-must-be-at-least-32-characters",
    "Issuer": "MyApp",
    "Audience": "MyAppUsers",
    "ExpirationMinutes": 60
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "noreply@myapp.com",
    "SenderName": "MyApp"
  },
  "AllowedHosts": "*"
}
```

### 🔧 強型別配置
```csharp
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}

// 在 Program.cs 中註冊
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// 在服務中使用
public class JwtService
{
    private readonly JwtSettings _jwtSettings;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
}
```

## 📋 程式碼規範與命名慣例

### 🎯 命名慣例統一規範
- **類別**: PascalCase (`UserService`, `ProductController`)
- **方法**: PascalCase (`GetUserAsync`, `CreateProduct`)
- **屬性**: PascalCase (`FirstName`, `CreatedAt`)
- **私有欄位**: camelCase with underscore (`_logger`, `_userService`)
- **參數**: camelCase (`userId`, `productName`)
- **常數**: PascalCase (`MaxRetryCount`, `DefaultTimeout`)
- **介面**: IPascalCase (`IUserService`, `IRepository`)

### 📝 註解規範
```csharp
/// <summary>
/// 根據使用者 ID 取得使用者資訊
/// </summary>
/// <param name="userId">使用者唯一識別碼</param>
/// <returns>使用者資訊，如果不存在則返回 null</returns>
/// <exception cref="ArgumentException">當 userId 小於等於 0 時拋出</exception>
public async Task<User?> GetUserByIdAsync(int userId)
{
    if (userId <= 0)
        throw new ArgumentException("使用者 ID 必須大於 0", nameof(userId));

    return await _repository.GetByIdAsync(userId);
}
```

### 🔧 程式碼組織順序
1. **using 語句**: 按字母順序排列，系統命名空間在前
2. **類別成員順序**:
   - 常數
   - 私有欄位
   - 建構函式
   - 公開屬性
   - 公開方法
   - 私有方法

## 📁 資料夾命名與檔案組織規範

### 📋 資料夾命名統一規範表格

| 層級 | 資料夾 | 職責 | 命名慣例 | 範例 |
|------|--------|------|----------|------|
| **Presentation** | Controllers | API 端點控制 | `{Entity}Controller.cs` | `UsersController.cs` |
| | Middlewares | 請求處理管道 | `{Purpose}Middleware.cs` | `GlobalExceptionMiddleware.cs` |
| | Filters | 動作篩選器 | `{Purpose}Filter.cs` | `ValidateModelFilter.cs` |
| **Application** | Services/Interfaces | 服務介面定義 | `I{Entity}Service.cs` | `IUserService.cs` |
| | Services/Implementations | 業務邏輯實作 | `{Entity}Service.cs` | `UserService.cs` |
| | DTOs/Requests | 請求資料模型 | `{Action}{Entity}Dto.cs` | `CreateUserDto.cs` |
| | DTOs/Responses | 回應資料模型 | `{Entity}Dto.cs` | `UserDto.cs` |
| | Validators | 資料驗證器 | `{Entity}Validator.cs` | `CreateUserValidator.cs` |
| **Domain** | Entities | 領域實體 | `{Entity}.cs` | `User.cs` |
| | Models | 值物件/枚舉 | `{Purpose}.cs` | `UserRole.cs` |
| **Infrastructure** | Data | 資料存取 | `{Entity}Configuration.cs` | `UserConfiguration.cs` |
| | Repositories | 儲存庫實作 | `{Entity}Repository.cs` | `UserRepository.cs` |
| | Migrations | 資料庫遷移 | `{DateTime}_{Description}.cs` | `20240101000000_InitialCreate.cs` |

### 🗂️ 資料夾命名一致性原則

#### ✅ 統一使用複數形式
```
Controllers/        # ✅ 複數
Services/          # ✅ 複數
Entities/          # ✅ 複數
DTOs/              # ✅ 複數
Configurations/    # ✅ 複數
Repositories/      # ✅ 複數
Middlewares/       # ✅ 複數
Validators/        # ✅ 複數
Extensions/        # ✅ 複數
Mappings/          # ✅ 複數
Migrations/        # ✅ 複數
```

#### ❌ 避免混用單複數
```
Controller/        # ❌ 單數
Service/           # ❌ 單數
Entity/            # ❌ 單數
DTO/               # ❌ 單數
Configuration/     # ❌ 單數
Repository/        # ❌ 單數
Middleware/        # ❌ 單數
```

## 🎯 效能最佳化

### ⚡ 非同步程式設計
```csharp
// ✅ 正確的非同步模式
public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetUsersAsync()
{
    var users = await _userService.GetAllUsersAsync();
    return SuccessResult(users, "使用者清單取得成功");
}

// ❌ 避免的反模式
public ActionResult<ApiResponse<IEnumerable<UserDto>>> GetUsers()
{
    var users = _userService.GetAllUsersAsync().Result; // 可能造成死鎖
    return SuccessResult(users, "使用者清單取得成功");
}
```

### 🗄️ 資料庫查詢優化
```csharp
// ✅ 使用投影減少資料傳輸
public async Task<IEnumerable<UserDto>> GetUsersAsync()
{
    return await _context.Users
        .Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email
        })
        .ToListAsync();
}

// ✅ 使用分頁
public async Task<PagedResult<UserDto>> GetUsersPagedAsync(int page, int pageSize)
{
    var query = _context.Users.AsQueryable();
    
    var totalCount = await query.CountAsync();
    var users = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(u => new UserDto 
        { 
            Id = u.Id,
            Name = u.Name,
            Email = u.Email
        })
        .ToListAsync();

    return new PagedResult<UserDto>
    {
        Items = users,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}
```

## 📚 參考資源

- [ASP.NET Core 官方文檔](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core 文檔](https://docs.microsoft.com/ef/core/)
- [.NET API 設計指南](https://docs.microsoft.com/dotnet/standard/design-guidelines/)
- [ASP.NET Core 安全性最佳實踐](https://docs.microsoft.com/aspnet/core/security/)
- [Clean Architecture 設計模式](https://docs.microsoft.com/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [SOLID 原則詳解](https://docs.microsoft.com/dotnet/standard/modern-web-apps-azure/architectural-principles)

---

**🤖 本架構指南基於 ASP.NET Core 8.0 和最新的 .NET 生態系統最佳實踐，遵循 SOLID 原則，統一命名規範，並提供完整的 API 回應格式和全域例外處理機制。請根據專案實際需求進行調整。**