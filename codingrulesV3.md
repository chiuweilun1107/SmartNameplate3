---
description: 
globs: 
alwaysApply: true
---
# Hamastar ABP Framework 專案編碼規範
- 請在回覆問題前，以機器人符號回覆

## 🏗️ 專案架構規範

### 目錄結構
- `Domain.Shared`: 共享領域物件、DTO、常數
- `Domain`: 核心領域層（實體、領域服務、設定）
- `Application.Contracts`: 應用服務介面層
- `Application`: 應用服務實作層
- `EntityFrameworkCore`: 資料存取層（Repository、DbContext）
- `HttpApi`: Web API 控制器層
- `HttpApi.Client`: API 客戶端
- `Web`: MVC Web 應用程式層
- `DbMigrator`: 資料庫遷移工具

### 完整目錄結構規範

```
src/
├── Hamastar.{ProjectName}.Application/                    # 應用服務實作層
│   ├── Properties/                                        # 專案屬性
│   ├── {Feature}/                                         # 功能模組資料夾
│   │   └── {Feature}AppService.cs                         # 功能應用服務
│   ├── AbpOrganizationUnits/                              # 組織單位管理
│   ├── AbpRoles/                                          # 角色管理
│   ├── AbpUsers/                                          # 使用者管理
│   ├── AuditTrails/                                       # 審計軌跡
│   ├── File/                                              # 檔案管理
│   ├── Permission/                                        # 權限管理
│   ├── {ProjectName}ApplicationModule.cs                  # 應用層模組
│   ├── {ProjectName}AppService.cs                         # 基礎應用服務
│   └── {ProjectName}ApplicationAutoMapperProfile.cs       # AutoMapper 設定
│
├── Hamastar.{ProjectName}.Application.Contracts/          # 應用服務介面層
│   ├── Enum/                                              # 列舉定義
│   ├── IApplication/                                      # 應用服務介面
│   │   ├── {Feature}/                                     # 功能介面資料夾
│   │   │   └── I{Feature}AppService.cs                    # 功能介面服務
│   │   ├── AbpOrganizationUnits/                          # 組織單位介面
│   │   ├── AbpRoles/                                      # 角色管理介面
│   │   ├── AbpUsers/                                      # 使用者管理介面
│   │   ├── AuditTrails/                                   # 審計軌跡介面
│   │   ├── File/                                          # 檔案管理介面
│   │   └── Permission/                                    # 權限管理介面
│   ├── Permissions/                                       # 權限定義
│   │   ├── {ProjectName}PermissionDefinitionProvider.cs   # 定義權限
│   │   └── {ProjectName}Permissions.cs                    # 定義權限欄位值 層級列表
│   ├── {ProjectName}ApplicationContractsModule.cs         # 合約層模組
│   └── {ProjectName}DtoExtensions.cs                      # DTO 擴展方法
│
├── Hamastar.{ProjectName}.DbMigrator/                     # 啟動工具層(資料庫初始化工具程式)
│   ├── Properties/                                        # 專案屬性
│   ├── appsettings.json                                   # 應用程式設定
│   ├── appsettings.Development.json                       # 開發環境設定
│   ├── appsettings.Production.json                        # 正式環境設定
│   ├── appsettings.secrets.json                           # 機密設定
│   ├── DbMigratorHostedService.cs                         # 資料庫遷移啟動服務
│   ├── {ProjectName}DbMigratorModule.cs                   # DbMigrator 模組
│   └── Program.cs                                         # 程式進入點
│
├── Hamastar.{ProjectName}.Domain/                         # 核心領域層
│   ├── Properties/                                        # 專案屬性
│   ├── Data/                                              # 資料初始化
│   │   ├── I{ProjectName}DbSchemaMigrator.cs              # 定義資料庫結構遷移的介面
│   │   ├── Null{ProjectName}DbSchemaMigrator.cs           # 資料庫提供者沒有定義使用類別
│   │   └── {ProjectName}DbMigrationService.cs             # 定義資料庫結構遷移
│   ├── IdentityServer/                                    # 身份認證伺服器
│   ├── Localization/                                      # 本地化設定
│   │   └── {ProjectName}/                                 # 專案本地化資源
│   ├── Settings/                                          # 設定管理
│   │   ├── {ProjectName}SettingDefinitionProvider.cs      # 設定定義的類別
│   │   └── {ProjectName}Settings.cs                       # 設定常數
│   ├── {ProjectName}Consts.cs                             # 專案常數
│   └── {ProjectName}DomainModule.cs                       # 領域層模組
│
├── Hamastar.{ProjectName}.Domain.Shared/                  # 共享領域物件層
│   ├── Dto/                                               # 資料傳輸物件
│   │   └── Backend/                                       # 後台 DTO
│   │       ├── {Feature}/                                 # 功能 DTO 資料夾
│   │       │   ├── Request/                               # 請求 DTO
│   │       │   ├── Response/                              # 回應 DTO
│   │       │   ├── {Feature}Item.cs                       # 功能項目
│   │       │   ├── {Feature}ItemForList.cs                # 列表項目
│   │       │   └── {Feature}ItemForListByPage.cs          # 分頁列表項目
│   │       ├── AbpOrganizationUnits/                      # 組織單位 DTO
│   │       │   ├── Request/
│   │       │   └── Response/
│   │       ├── AbpRoles/                                  # 角色 DTO
│   │       │   ├── Request/
│   │       │   └── Response/
│   │       ├── AbpUsers/                                  # 使用者 DTO
│   │       │   ├── Request/
│   │       │   └── Response/
│   │       ├── AuditTrails/                               # 審計軌跡 DTO
│   │       │   ├── Request/
│   │       │   └── Response/
│   │       ├── File/                                      # 檔案 DTO
│   │       │   ├── Request/
│   │       │   └── Response/
│   │       ├── General/                                   # 通用 DTO
│   │       │   └── Response/
│   │       ├── Permission/                                # 權限 DTO
│   │       ├── SaveMimaRecord/                            # 密碼變更記錄 DTO
│   │       │   └── Request/
│   │       ├── BusinessLogicResponse.cs                   # 業務邏輯回應
│   │       ├── PageRequest.cs                             # 分頁請求
│   │       ├── PageResponse.cs                            # 分頁回應
│   │       └── TurnstileResponse.cs                       # Turnstile 回應
│   ├── Localization/                                      # 本地化資源
│   │   └── {ProjectName}/                                 # 專案本地化檔案
│   ├── MultiTenancy/                                      # 多租戶設定
│   ├── Utils/                                             # 工具類別(共用方法)
│   ├── {ProjectName}DomainErrorCodes.cs                   # 錯誤代碼
│   ├── {ProjectName}DomainSharedModule.cs                 # 共享模組
│   ├── {ProjectName}GlobalFeatureConfigurator.cs          # 全域功能配置
│   └── {ProjectName}ModuleExtensionConfigurator.cs        # 模組擴展配置
│
├── Hamastar.{ProjectName}.EntityFrameworkCore/             # 資料存取層
│   ├── Properties/                                         # 專案屬性
│   ├── Entities/                                           # 資料庫實體定義
│   │   └── {Feature}.cs                                    # 資料庫功能實體
│   ├── EntityFrameworkCore/                                # EF Core 配置
│   │   ├── EntityFrameworkCore{ProjectName}DbSchemaMigrator.cs  # 資料庫結構遷移實作類別
│   │   ├── {ProjectName}DbContext.cs                       # 資料庫上下文類別
│   │   ├── {ProjectName}DbContextFactory.cs                # DbContext 建立工廠
│   │   ├── {ProjectName}EfCoreEntityExtensionMappings.cs   # 實體擴充映射設定
│   │   └── {ProjectName}EntityFrameworkCoreModule.cs       # EF Core 模組
│   ├── Migrations/                                         # 資料庫遷移
│   └── Repositories/                                       # 儲存庫實作資料夾
│       ├── {Feature}/                                      # 功能儲存庫資料夾
│       │   ├── {Feature}Repository.cs                      # 功能儲存庫實作
│       │   └── I{Feature}Repository.cs                     # 功能儲存庫實作介面
│       └── {ProjectName}DbContextBase.css                  # 泛型化資料庫
│
├── Hamastar.{ProjectName}.HttpApi/                         # Web API 控制器層
│   ├── Models/                                             # API 模型
│   │   └── Test/                                           # 測試模型
│   └── {ProjectName}HttpApiModule.cs                       # API 模組
│
├── Hamastar.{ProjectName}.HttpApi.Client/                  # API 客戶端
│   └── {ProjectName}HttpApiClientModule.cs                 # HTTP API 客戶端代理模組
│
├── Hamastar.{ProjectName}.Web/                             # Web 應用程式層
│   ├── Controllers/                                        # MVC 控制器
│   ├── Properties/                                         # 專案屬性
│   │   └── PublishProfiles/                               # 發佈設定檔
│   ├── wwwroot/                                            # 靜態資源(Angular打包後放置位置)
│   ├── appsettings.json                                    # 應用程式設定
│   ├── appsettings.Development.json                        # 開發環境設定
│   ├── appsettings.Production.json                         # 正式環境設定
│   ├── appsettings.secrets.json                            # 機密設定
│   ├── Program.cs                                          # 程式進入點
│   ├── web.config                                          # IIS 配置
│   ├── {ProjectName}BrandingProvider.cs                    # 品牌提供者
│   ├── {ProjectName}WebAutoMapperProfile.cs                # Web AutoMapper 設定
│   └── {ProjectName}WebModule.cs                           # Web 模組

└── Hamastar.DataStorage.Client/                            # 自訂資料儲存客戶端 (可選)
    ├── OldAuth/                                           # 舊版認證
    ├── DataInfo.cs                                        # 資料資訊
    ├── DataStorageClient.cs                              # 儲存客戶端
    ├── HamastarStorageHelper.cs                          # 儲存輔助工具
    └── IHamastarStorageHelper.cs                         # 儲存輔助介面

```

### 功能模組化組織原則

#### **1. 每個功能都要有完整的資料夾結構**
```
{Feature}/
├── Application/{Feature}/                    # 應用服務實作
├── Application.Contracts/IApplication/{Feature}/  # 應用服務介面
├── Domain.Shared/Dto/Backend/{Feature}/     # DTO 定義
│   ├── Request/                             # 請求 DTO
│   ├── Response/                            # 回應 DTO
│   ├── {Feature}Item.cs                     # 基本項目
│   ├── {Feature}ItemForList.cs              # 列表項目
│   └── {Feature}ItemForListByPage.cs        # 分頁列表項目
├── EntityFrameworkCore/Entities/            # 實體定義
└── EntityFrameworkCore/Repositories/{Feature}/  # 儲存庫實作
```

#### **2. DTO 命名規範**
- `{Feature}Item.cs` - 基本資料項目
- `{Feature}ItemForList.cs` - 列表顯示項目
- `{Feature}ItemForListByPage.cs` - 分頁列表項目
- `Request/{Feature}ListRequest.cs` - 列表查詢請求
- `Request/{Feature}Request.cs` - 單筆查詢請求
- `Request/Create{Feature}Request.cs` - 新增請求
- `Request/Update{Feature}Request.cs` - 更新請求
- `Response/{Feature}ListResponse.cs` - 列表查詢回應
- `Response/{Feature}Response.cs` - 單筆查詢回應

#### **3. 標準 ABP 模組檔案**
每個專案層都要包含：
- `{ProjectName}{Layer}Module.cs` - 模組定義檔案
- `Properties/` - 專案屬性資料夾
- 適當的 AutoMapper Profile 檔案

### 命名規範
- **公司前綴**: `Hamastar`
- **專案名稱**: `{ProjectName}`
- **命名空間**: `Hamastar.{ProjectName}.{Layer}`
- **檔案名稱**: PascalCase
- **類別名稱**: PascalCase
- **方法名稱**: PascalCase
- **屬性名稱**: PascalCase
- **欄位名稱**: camelCase with underscore prefix `_fieldName`
- **常數名稱**: UPPER_CASE

## 📝 根據文件類型的程式碼撰寫規範

### 🗂️ 根據檔案路徑自動識別文件類型

#### **當文件路徑包含 `/EntityFrameworkCore/` 且檔名包含 `DbContext` 時 - DbContext**
```csharp
//-----------------------------------------------------------------------
// <copyright file="{ProjectName}DbContext.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> {AuthorName} </author>
//-----------------------------------------------------------------------

using Hamastar.{ProjectName}.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Hamastar.{ProjectName}.EntityFrameworkCore;

/// <summary>
/// {ProjectName} 資料庫上下文實作
/// </summary>
[ConnectionStringName("Default")]
public class {ProjectName}DbContext : AbpDbContext<{ProjectName}DbContext>, I{ProjectName}DbContext
{
    #region DbSets

    // ========= 核心實體 =========

    /// <summary>
    /// {實體名稱}
    /// </summary>
    public DbSet<{Entity}> {Entities} { get; set; }

    // ========= 系統管理 =========

    /// <summary>
    /// 審計軌跡
    /// </summary>
    public DbSet<AuditTrail> AuditTrails { get; set; }

    #endregion DbSets

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="{ProjectName}DbContext" /> class
    /// </summary>
    /// <param name="options"> 資料庫上下文選項 </param>
    public {ProjectName}DbContext(DbContextOptions<{ProjectName}DbContext> options)
        : base(options)
    {
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// 配置模型建立
    /// </summary>
    /// <param name="builder"> 模型建構器 </param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */
        builder.Configure{ProjectName}();
    }

    #endregion Methods
}

/// <summary>
/// {ProjectName} 實體配置擴展方法
/// </summary>
public static class {ProjectName}DbContextModelCreatingExtensions
{
    #region Configuration Methods

    /// <summary>
    /// 配置 {ProjectName} 實體
    /// </summary>
    /// <param name="builder"> 模型建構器 </param>
    public static void Configure{ProjectName}(this ModelBuilder builder)
    {
        // 設定資料表前綴
        const string tablePrefix = "App";

        // ========= 核心實體配置 =========
        Configure{Entity}(builder, tablePrefix);

        // ========= 系統管理配置 =========
        ConfigureAuditTrail(builder, tablePrefix);
    }

    #endregion Configuration Methods

    #region Entity Configuration Methods

    /// <summary>
    /// 配置 {Entity} 實體
    /// </summary>
    /// <param name="builder"> 模型建構器 </param>
    /// <param name="tablePrefix"> 資料表前綴 </param>
    private static void Configure{Entity}(ModelBuilder builder, string tablePrefix)
    {
        builder.Entity<{Entity}>(b =>
        {
            b.ToTable(tablePrefix + "{Entities}");
            b.ConfigureByConvention();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength({Entity}Consts.MaxNameLength);

            b.Property(x => x.Description)
                .HasMaxLength({Entity}Consts.MaxDescriptionLength);

            b.HasIndex(x => x.Name);
            b.HasIndex(x => x.CreatorUserId);
        });
    }

    #endregion Entity Configuration Methods
}

#region Constants

/// <summary>
/// 實體常數定義
/// </summary>
public static class {Entity}Consts
{
    public const int MaxNameLength = 255;
    public const int MaxDescriptionLength = 1000;
}

#endregion Constants
```

#### **當文件路徑包含 `/Application/` 時 - Application Service**
```csharp
//-----------------------------------------------------------------------
// <copyright file="{FileName}.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> {AuthorName} </author>
//-----------------------------------------------------------------------

using Ganss.Xss;
using Hamastar.{ProjectName}.Dto.Backend;
using Hamastar.{ProjectName}.Dto.Backend.{Feature};
using Hamastar.{ProjectName}.Dto.Backend.{Feature}.Request;
using Hamastar.{ProjectName}.Dto.Backend.{Feature}.Response;
using Hamastar.{ProjectName}.IApplication.{Feature};
using Hamastar.{ProjectName}.IApplication.AuditTrails;
using Hamastar.{ProjectName}.Permissions;
using Hamastar.{ProjectName}.Repositories.{Feature};
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.{ProjectName}.{Feature}
{
    /// <summary>
    /// {功能名稱} App
    /// </summary>
    public class {Feature}AppService : ApplicationService, I{Feature}AppService
    {
        #region Fields

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _log = Log.ForContext<{Feature}AppService>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// {功能名稱} 儲存庫
        /// </summary>
        private readonly I{Feature}Repository _{featureLower}Repository;

        /// <summary>
        /// 審計軌跡資料 APP
        /// </summary>
        private readonly IAuditTrailService _auditTrailService;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="{Feature}AppService" /> class
        /// </summary>
        /// <param name="currentUser"> 目前使用者 </param>
        /// <param name="{featureLower}Repository"> {功能名稱} 儲存庫 </param>
        /// <param name="auditTrailService"> 審計軌跡資料 APP </param>
        public {Feature}AppService(
            ICurrentUser currentUser,
            I{Feature}Repository {featureLower}Repository,
            IAuditTrailService auditTrailService)
        {
            _currentUser = currentUser;
            _{featureLower}Repository = {featureLower}Repository;
            _auditTrailService = auditTrailService;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 查詢：{功能名稱} 列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = {ProjectName}Permission.{Feature}Mgmt)]
        public async Task<BusinessLogicResponse<{Feature}ListResponse>> Get{Feature}ListByPage({Feature}ListRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<{Feature}ListResponse> response = new();
            try
            {
                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Keyword = sanitizer.Sanitize(request.Keyword);
                request.Keyword = HttpUtility.UrlDecode(request.Keyword);

                // ========= 業務邏輯執行 =========
                {Feature}ListResponse {featureLower}List = await _{featureLower}Repository.GetListByPage(request);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("{功能名稱}查詢", "DATA_READ", "查詢了" + {featureLower}List.ItemTotalCount + "筆{功能名稱}資料");
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "查詢完成";
                response.Data = {featureLower}List;
            }
            catch (BusinessException be)
            {
                response.Status = "error";
                response.Message = be.Message;
                await uow.RollbackAsync();
            }
            catch (Exception e)
            {
                _log.Error("An unexpected error occurred: {StackTrace}", e.StackTrace);
                await uow.RollbackAsync();
                throw new UserFriendlyException(e.Message);
            }

            return response;
        }

        /// <summary>
        /// 新增：{功能名稱}
        /// </summary>
        /// <param name="createRequest"> 新增資訊 </param>
        /// <returns> 成功與否 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = {ProjectName}Permission.{Feature}Mgmt)]
        public async Task<BusinessLogicResponse<Create{Feature}Response>> Create{Feature}(Create{Feature}Request createRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<Create{Feature}Response> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (createRequest.Name.Length > 200) throw new BusinessException(message: "名稱過長 Max 200");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();

                // ========= 建立實體 =========
                Entities.{Feature} create{Feature} = new()
                {
                    Name = sanitizer.Sanitize(createRequest.Name),
                    Description = sanitizer.Sanitize(createRequest.Description),
                    Enable = createRequest.Enable,
                    CreationTime = DateTime.Now,
                    CreatorUserId = CurrentUser.Id ?? Guid.Empty,
                    LastModificationTime = DateTime.Now,
                    LastModifierUserId = CurrentUser.Id ?? Guid.Empty
                };

                // ========= 執行新增 =========
                Create{Feature}Response create{Feature}Response = new();
                var result = await _{featureLower}Repository.InsertAsync(create{Feature});
                if (result != null)
                {
                    await CreateAuditTrail("{功能名稱}新增", "DATA_CREATE", "新增了一筆{功能名稱}資料且 成功");
                    await CurrentUnitOfWork.SaveChangesAsync();
                    response.Status = "success";
                    response.Message = "新增成功";
                    create{Feature}Response.Result = true;
                    create{Feature}Response.{Feature}Id = result.Id;
                }
                else
                {
                    await CreateAuditTrail("{功能名稱}新增", "DATA_CREATE", "新增了一筆{功能名稱}資料且 失敗");
                    response.Status = "error";
                    response.Message = "新增失敗";
                    create{Feature}Response.Result = false;
                    await uow.RollbackAsync();
                }
                await uow.CompleteAsync();
                response.Data = create{Feature}Response;
            }
            catch (BusinessException be)
            {
                response.Status = "error";
                response.Message = be.Message;
                await uow.RollbackAsync();
            }
            catch (Exception e)
            {
                _log.Error("An unexpected error occurred: {StackTrace}", e.StackTrace);
                await uow.RollbackAsync();
                throw new UserFriendlyException(e.Message);
            }

            return response;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 儲存操作紀錄至審計軌跡
        /// </summary>
        /// <param name="target"> 操作目標 </param>
        /// <param name="type"> 操作類型 </param>
        /// <param name="description"> 操作描述 </param>
        /// <returns> Task </returns>
        private async Task CreateAuditTrail(string target, string type, string description)
        {
            await _auditTrailService.CreateAsync(
                target: "{專案名稱}-系統管理-" + target,
                type: type,
                description: description
                );
        }

        #endregion Private Methods
    }
}
```

#### **當文件路徑包含 `/Repositories/` 時 - Repository**
```csharp
//-----------------------------------------------------------------------
// <copyright file="{Feature}Repository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> {AuthorName} </author>
//-----------------------------------------------------------------------

using Hamastar.{ProjectName}.Dto.Backend.{Feature};
using Hamastar.{ProjectName}.Dto.Backend.{Feature}.Request;
using Hamastar.{ProjectName}.Dto.Backend.{Feature}.Response;
using Hamastar.{ProjectName}.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.{ProjectName}.Repositories.{Feature}
{
    /// <summary>
    /// {功能名稱} 儲存庫
    /// </summary>
    public class {Feature}Repository : EfCoreRepository<{ProjectName}DbContext, Entities.{Feature}, Guid>, I{Feature}Repository
    {
        #region Fields

        /// <summary>
        /// SettingProvider
        /// </summary>
        private readonly IConfiguration _appConfiguration;

        /// <summary>
        /// UnitOfWorkManager
        /// </summary>
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger log = Log.ForContext<{Feature}Repository>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="{Feature}Repository" /> class
        /// </summary>
        /// <param name="appConfiguration"> SettingProvider </param>
        /// <param name="contextProvider"> dbContext </param>
        /// <param name="unitOfWorkManager"> Unit of Work Manager </param>
        /// <param name="currentUser"> 目前登入的使用者 </param>
        public {Feature}Repository(IConfiguration appConfiguration,
            IDbContextProvider<{ProjectName}DbContext> contextProvider,
            IUnitOfWorkManager unitOfWorkManager,
            ICurrentUser currentUser) : base(contextProvider)
        {
            _appConfiguration = appConfiguration;
            _unitOfWorkManager = unitOfWorkManager;
            _currentUser = currentUser;
        }

        #endregion Constructor

        #region Public Methods

        // Repository 方法模式：
        // 1. 取得 DbContext: var dbContext = await GetDbContextAsync();
        // 2. 建立查詢: IQueryable<Entities.{Feature}> query = dbContext.{Feature};
        // 3. 條件篩選: if (!string.IsNullOrWhiteSpace(request.Keyword)) query = query.Where(...)
        // 4. 分頁處理: Skip((page - 1) * pageSize).Take(pageSize)
        // 5. 執行查詢: await query.ToListAsync()

        #endregion Public Methods

        #region Private Methods

        // 私有輔助方法

        #endregion Private Methods
    }
}
```

#### **當文件路徑包含 `/Entities/` 時 - Entity**
```csharp
//-----------------------------------------------------------------------
// <copyright file="{Feature}.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> {AuthorName} </author>
//-----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hamastar.{ProjectName}.Entities;

/// <summary>
/// {功能名稱}主表
/// </summary>
[Table("{TableName}")]
[Comment("{功能名稱}主表")]
public class {Feature} : Volo.Abp.Domain.Entities.BasicAggregateRoot<Guid>
{
    #region Properties

    /// <summary>
    /// {功能名稱} ID
    /// </summary>
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Comment("{功能名稱} ID")]
    public override Guid Id { get; protected set; }

    /// <summary>
    /// 標題
    /// </summary>
    [MaxLength(200)]
    [Column("Title", TypeName = "nvarchar")]
    [Comment("標題")]
    [Required]
    public string Title { get; set; }

    /// <summary>
    /// 內容
    /// </summary>
    [Column("Content", TypeName = "nvarchar(MAX)")]
    [Comment("內容")]
    [Required]
    public string Content { get; set; }

    /// <summary>
    /// 順序
    /// </summary>
    [Column("Sort", TypeName = "int")]
    [Comment("順序")]
    [Required]
    public int Sort { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    [Column("Enable", TypeName = "int")]
    [Comment("是否啟用")]
    [Required]
    public int Enable { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    [Column("CreationTime", TypeName = "datetime2(7)")]
    [Comment("建立時間")]
    [Required]
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    [Column("CreatorUserId", TypeName = "uniqueidentifier")]
    [Comment("建立者")]
    [Required]
    public Guid CreatorUserId { get; set; }

    /// <summary>
    /// 最後修改時間
    /// </summary>
    [Column("LastModificationTime", TypeName = "datetime2(7)")]
    [Comment("最後修改時間")]
    [Required]
    public DateTime LastModificationTime { get; set; }

    /// <summary>
    /// 最後修改者
    /// </summary>
    [Column("LastModifierUserId", TypeName = "uniqueidentifier")]
    [Comment("最後修改者")]
    [Required]
    public Guid LastModifierUserId { get; set; }

    #endregion Properties

    // Entity 規範：
    // 1. 必須繼承 BasicAggregateRoot<Guid>
    // 2. 所有屬性都要有 XML 註解
    // 3. 使用 [Column] 指定欄位名稱和類型
    // 4. 使用 [Comment] 為欄位加註解
    // 5. 必須包含審計欄位 (CreationTime, CreatorUserId, LastModificationTime, LastModifierUserId)
    // 6. 使用 #region Properties 組織屬性
}
```

#### **當文件路徑包含 `/Dto/` 或 `/Request/` 時 - Request DTO**
```csharp
//-----------------------------------------------------------------------
// <copyright file="{Feature}ListRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> {AuthorName} </author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;

namespace Hamastar.{ProjectName}.Dto.Backend.{Feature}.Request
{
    /// <summary>
    /// {功能名稱}列表 Request
    /// </summary>
    public class {Feature}ListRequest : PageRequest
    {
        #region Properties

        /// <summary>
        /// 關鍵字搜索
        /// </summary>
        [JsonProperty("keyword")]
        public string Keyword { get; set; } = "";

        #endregion Properties

        // Request DTO 規範：
        // 1. 繼承適當的基底類別 (PageRequest 用於分頁查詢)
        // 2. 使用 [JsonProperty] 指定 JSON 屬性名稱
        // 3. 提供預設值
        // 4. 所有屬性都要有 XML 註解
        // 5. 使用 #region Properties 組織屬性
    }
}
```

#### **當文件路徑包含 `/Response/` 時 - Response DTO**
```csharp
//-----------------------------------------------------------------------
// <copyright file="{Feature}ListResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> {AuthorName} </author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hamastar.{ProjectName}.Dto.Backend.{Feature}.Response
{
    /// <summary>
    /// {功能名稱}列表 Response
    /// </summary>
    public class {Feature}ListResponse : PageResponse
    {
        #region Properties

        /// <summary>
        /// {功能名稱}列表
        /// </summary>
        [JsonProperty("items")]
        public List<{Feature}ItemForListByPage> Items { get; set; } = new();

        #endregion Properties

        // Response DTO 規範：
        // 1. 繼承適當的基底類別 (PageResponse 用於分頁回應)
        // 2. 使用 [JsonProperty] 指定 JSON 屬性名稱
        // 3. 集合屬性提供預設的空集合
        // 4. 所有屬性都要有 XML 註解
        // 5. 使用 #region Properties 組織屬性
    }
}
```

#### **當文件路徑包含 `/Application.Contracts/` 或 `/IApplication/` 時 - Interface**
```csharp
//-----------------------------------------------------------------------
// <copyright file="I{Feature}AppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> {AuthorName} </author>
//-----------------------------------------------------------------------

using Hamastar.{ProjectName}.Dto.Backend;
using Hamastar.{ProjectName}.Dto.Backend.{Feature}.Request;
using Hamastar.{ProjectName}.Dto.Backend.{Feature}.Response;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Hamastar.{ProjectName}.IApplication.{Feature}
{
    /// <summary>
    /// {功能名稱} App Service 介面
    /// </summary>
    public interface I{Feature}AppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 查詢：{功能名稱} 列表(頁數)
        /// </summary>
        /// <param name="request"> 查詢條件及頁數 </param>
        /// <returns> 結果及頁數資訊 </returns>
        Task<BusinessLogicResponse<{Feature}ListResponse>> Get{Feature}ListByPage({Feature}ListRequest request);

        #endregion Methods

        // Interface 規範：
        // 1. 繼承 IApplicationService
        // 2. 所有方法都要有 XML 註解
        // 3. 使用 Task<BusinessLogicResponse<T>> 作為回傳型別
        // 4. 方法命名遵循 Get/Create/Update/Delete 模式
        // 5. 使用 #region Methods 組織方法
    }
}
```

#### **當文件路徑包含 `/Permissions/` 時 - Permission 常數類別**
```csharp
//-----------------------------------------------------------------------
// <copyright file="{ProjectName}Permission.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> {AuthorName} </author>
//-----------------------------------------------------------------------

using Hamastar.{ProjectName}.Dto.Backend.Permission;
using System.Collections.Generic;

namespace Hamastar.{ProjectName}.Permissions
{
    public static class {ProjectName}Permission
    {
        #region 權限 Fields

        /// <summary>
        /// 專案群組名稱
        /// </summary>
        public const string GroupName = "{ProjectName}";

        // 【後台管理系統】
        public const string BackendPlat = GroupName + ".BackendPlat";

        // ========= 系統管理 =========
        public const string SystemMgmt = GroupName + ".SystemMgmt";

        // ================== 角色維護 ==================
        public const string RoleMgmt = GroupName + ".RoleMgmt";

        // ================== 帳號維護 ==================
        public const string AccountMgmt = GroupName + ".AccountMgmt";

        // ================== 系統日誌 ==================
        public const string AuditTrailMgmt = GroupName + ".AuditTrailMgmt";

        #endregion 權限 Fields

        #region Public Methods

        /// <summary>
        /// 查詢：權限層級 列表 (主權限 + 子權限key值)
        /// </summary>
        /// <returns> 取得結果 </returns>
        public static List<PermissionItem> GetPermissionHierarchy()
        {
            // 實作權限階層邏輯
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 組成：一組權限物件Entity
        /// </summary>
        /// <param name="permissionKey"> 主權限 </param>
        /// <param name="childs"> 子權限key值 </param>
        /// <returns> 取得結果 </returns>
        private static PermissionItem AddPermissionItem(string permissionKey, string[] childs)
        {
            // 實作權限項目建立邏輯
        }

        #endregion Private Methods
    }
}
```

## 🎯 **重要的撰寫原則**

### **程式碼組織規範**
1. **必須使用 #region 組織程式碼區塊**：
   - `#region Fields` - 欄位宣告
   - `#region Properties` - 屬性宣告
   - `#region Constructor` - 建構函式
   - `#region Public Methods` - 公開方法
   - `#region Private Methods` - 私有方法
   - `#region Constants` - 常數定義

2. **使用註解分隔線區分功能區塊**：
   - `// ========= 核心實體 =========`
   - `// ========= 系統管理 =========`
   - `// ================== 功能名稱 ==================`

3. **DbContext 特殊規範**：
   - DbSet 屬性按功能分組並加上註解分隔線
   - 配置方法要拆分成獨立的私有方法
   - 常數定義要放在檔案末尾的 #region Constants 中
   - 每個實體配置方法都要有完整的 XML 註解

### **根據文件位置自動應用規範**
1. **Application 層**: 使用 UnitOfWork、錯誤處理、審計軌跡
2. **Repository 層**: 使用 EF Core 查詢模式、分頁處理
3. **Entity 層**: 使用 Table/Column 屬性、審計欄位
4. **DTO 層**: 使用 JsonProperty、適當的基底類別
5. **Interface 層**: 使用 BusinessLogicResponse 回傳型別
6. **DbContext 層**: 使用功能分組、方法拆分、常數組織

### **通用規範**
- 每個檔案都要有版權標頭（使用 `//-----------------------------------------------------------------------` 格式）
- 所有公開成員都要有 XML 註解
- 使用 #region 組織程式碼區塊
- 遵循命名規範
- 包含適當的 using statements
- 方法內部邏輯要有適當的註解說明

### **可開合設置 (Collapsible Regions)**
- 所有類別都必須使用 #region 讓程式碼可以折疊
- 便於程式碼審查和導航
- 提高程式碼可讀性和維護性

### **安全規範**
- Application Service 中使用 HtmlSanitizer 清理輸入
- 使用 [Authorize] 屬性進行權限控制
- Repository 層避免 SQL Injection

### **日誌規範**
- 使用 Serilog: `private readonly ILogger _log = Log.ForContext<ClassName>();`
- 錯誤日誌: `_log.Error("An unexpected error occurred: {StackTrace}", e.StackTrace);`

## 🚫 禁止事項

1. ❌ 不要直接操作 DbContext，必須透過 Repository
2. ❌ 不要在 Application 層直接注入 DbContext
3. ❌ 不要忽略例外處理
4. ❌ 不要忽略權限驗證
5. ❌ 不要忽略輸入驗證和清理
6. ❌ 不要使用硬編碼字串，使用常數或設定檔
7. ❌ 不要在 Repository 層處理業務邏輯
8. ❌ 不要忽略審計軌跡記錄
9. ❌ 不要忽略 #region 程式碼組織
10. ❌ 不要將大型方法寫在一起，要適當拆分

## 💡 最佳實踐

1. 優先使用 async/await
2. 使用 using statement 管理資源
3. 遵循 SOLID 原則
4. 使用依賴注入
5. 保持方法簡潔，單一職責
6. 使用有意義的變數和方法名稱
7. 適當的層級分離
8. 完整的單元測試覆蓋
9. **使用 #region 讓程式碼可折疊和易於導航**
10. **使用註解分隔線清楚區分功能區塊**
11. **將複雜的配置方法拆分成多個小方法**
12. **常數定義要集中管理並用 #region 包圍**

## 🏛️ **Application Service 標準實作模式**

### **Application Service 是企業級應用的核心層**
就像 DbContext 有特殊的配置規範一樣，Application Service 也有其標準的實作模式，這是確保程式碼品質和維護性的關鍵。

### **標準方法實作模式**
每個 Application Service 方法都必須遵循以下七步驟模式：

```csharp
[Authorize(AuthenticationSchemes = "Bearer", Policy = {ProjectName}Permission.{Feature}Mgmt)]
public async Task<BusinessLogicResponse<{Response}>> {MethodName}({Request} request)
{
    using IUnitOfWork uow = UnitOfWorkManager.Begin();
    BusinessLogicResponse<{Response}> response = new();
    try
    {
        // ========= 步驟1：輸入驗證 =========
        if (request.Name.Length > 200) throw new BusinessException(message: "名稱過長 Max 200");

        // ========= 步驟2：輸入清理 =========
        HtmlSanitizer sanitizer = new();
        request.Keyword = sanitizer.Sanitize(request.Keyword);
        request.Keyword = HttpUtility.UrlDecode(request.Keyword);

        // ========= 步驟3：業務邏輯執行 =========
        var result = await _{repository}.{MethodName}(request);

        // ========= 步驟4：審計軌跡記錄 =========
        await CreateAuditTrail("{操作名稱}", "{操作類型}", "{操作描述}");
        await uow.CompleteAsync();

        // ========= 步驟5：成功回應 =========
        response.Status = "success";
        response.Message = "{操作完成訊息}";
        response.Data = result;
    }
    catch (BusinessException be)
    {
        // ========= 步驟6：業務例外處理 =========
        response.Status = "error";
        response.Message = be.Message;
        await uow.RollbackAsync();
    }
    catch (Exception e)
    {
        // ========= 步驟7：系統例外處理 =========
        _log.Error("An unexpected error occurred: {StackTrace}", e.StackTrace);
        await uow.RollbackAsync();
        throw new UserFriendlyException(e.Message);
    }

    return response;
}
```

### **必要的程式碼組織**
1. **#region Fields** - 所有依賴注入的服務
2. **#region Constructor** - 依賴注入初始化
3. **#region Public Methods** - 所有公開的業務方法
4. **#region Private Methods** - 私有輔助方法（如 CreateAuditTrail）

### **審計軌跡操作類型**
- `DATA_READ` - 資料查詢操作
- `DATA_CREATE` - 資料新增操作
- `DATA_UPDATE` - 資料更新操作
- `DATA_DELETE` - 資料刪除操作

### **錯誤處理層級**
1. **BusinessException** - 業務邏輯錯誤，訊息可直接顯示給使用者
2. **Exception** - 系統錯誤，需要記錄詳細日誌並轉換為友善訊息

### **為什麼 Application Service 需要特殊規範？**
1. **一致性** - 確保所有開發者寫出相同品質的程式碼
2. **可維護性** - 標準化的結構便於後續維護和擴展
3. **安全性** - 統一的輸入清理和權限驗證
4. **可追蹤性** - 完整的審計軌跡記錄
5. **穩定性** - 標準化的錯誤處理和事務管理

### **與其他層的差異**
- **Repository 層**：專注於資料存取，不處理業務邏輯
- **Entity 層**：純資料模型，不包含業務邏輯
- **Application Service 層**：業務邏輯核心，需要完整的安全和審計機制

這種標準化確保了 Cursor 生成的程式碼具有企業級的品質和一致性。
