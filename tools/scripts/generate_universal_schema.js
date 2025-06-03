const fs = require('fs');
const path = require('path');

// 資料庫提供者配置
const DatabaseProviders = {
    POSTGRESQL: 'postgresql',
    SQLSERVER: 'sqlserver'
};

// 資料類型映射
const TypeMappings = {
    [DatabaseProviders.POSTGRESQL]: {
        identity: 'SERIAL PRIMARY KEY',
        text: 'TEXT',
        varcharMax: 'TEXT',
        varchar: (length) => `VARCHAR(${length})`,
        json: 'JSONB',
        boolean: 'BOOLEAN',
        timestamp: 'TIMESTAMP WITH TIME ZONE',
        integer: 'INTEGER',
        decimal: 'DECIMAL'
    },
    [DatabaseProviders.SQLSERVER]: {
        identity: 'INT IDENTITY(1,1) PRIMARY KEY',
        text: 'NVARCHAR(MAX)',
        varcharMax: 'NVARCHAR(MAX)',
        varchar: (length) => `NVARCHAR(${length})`,
        json: 'NVARCHAR(MAX)',
        boolean: 'BIT',
        timestamp: 'DATETIME2',
        integer: 'INT',
        decimal: 'DECIMAL'
    }
};

// 表格結構定義
const TableDefinitions = {
    Users: {
        columns: [
            { name: 'Id', type: 'identity' },
            { name: 'UserName', type: 'varchar', length: 100, required: true },
            { name: 'Email', type: 'varchar', length: 255, required: true },
            { name: 'PasswordHash', type: 'varchar', length: 500, required: true },
            { name: 'Role', type: 'varchar', length: 50, required: true },
            { name: 'IsActive', type: 'boolean', required: true, default: true },
            { name: 'CreatedAt', type: 'timestamp', required: true },
            { name: 'UpdatedAt', type: 'timestamp', required: true }
        ],
        indexes: [
            { name: 'IX_Users_UserName', columns: ['UserName'], unique: true },
            { name: 'IX_Users_Email', columns: ['Email'], unique: true },
            { name: 'IX_Users_Role', columns: ['Role'] }
        ]
    },
    Groups: {
        columns: [
            { name: 'Id', type: 'identity' },
            { name: 'Name', type: 'varchar', length: 100, required: true },
            { name: 'Description', type: 'varchar', length: 500 },
            { name: 'Color', type: 'varchar', length: 20 },
            { name: 'CreatedAt', type: 'timestamp', required: true },
            { name: 'UpdatedAt', type: 'timestamp', required: true }
        ],
        indexes: [
            { name: 'IX_Groups_Name', columns: ['Name'] },
            { name: 'IX_Groups_CreatedAt', columns: ['CreatedAt'] }
        ]
    },
    Cards: {
        columns: [
            { name: 'Id', type: 'identity' },
            { name: 'Name', type: 'varchar', length: 100, required: true },
            { name: 'Description', type: 'varchar', length: 500 },
            { name: 'Status', type: 'varchar', length: 50, required: true },
            { name: 'ContentA', type: 'json' },
            { name: 'ContentB', type: 'json' },
            { name: 'CreatedAt', type: 'timestamp', required: true },
            { name: 'UpdatedAt', type: 'timestamp', required: true }
        ],
        indexes: [
            { name: 'IX_Cards_Status', columns: ['Status'] },
            { name: 'IX_Cards_CreatedAt', columns: ['CreatedAt'] }
        ]
    },
    Templates: {
        columns: [
            { name: 'Id', type: 'identity' },
            { name: 'Name', type: 'varchar', length: 255, required: true },
            { name: 'Description', type: 'varchar', length: 500 },
            { name: 'Category', type: 'varchar', length: 100, default: "'general'" },
            { name: 'LayoutDataA', type: 'json' },
            { name: 'LayoutDataB', type: 'json' },
            { name: 'Dimensions', type: 'json' },
            { name: 'IsPublic', type: 'boolean', required: true, default: false },
            { name: 'IsActive', type: 'boolean', required: true, default: true },
            { name: 'CreatedBy', type: 'integer' },
            { name: 'CreatedAt', type: 'timestamp', required: true },
            { name: 'UpdatedAt', type: 'timestamp', required: true }
        ],
        foreignKeys: [
            { name: 'FK_Templates_Users_CreatedBy', column: 'CreatedBy', referencedTable: 'Users', referencedColumn: 'Id', onDelete: 'SET NULL' }
        ],
        indexes: [
            { name: 'IX_Templates_Category', columns: ['Category'] },
            { name: 'IX_Templates_IsPublic', columns: ['IsPublic'] },
            { name: 'IX_Templates_CreatedAt', columns: ['CreatedAt'] }
        ]
    },
    Devices: {
        columns: [
            { name: 'Id', type: 'identity' },
            { name: 'Name', type: 'varchar', length: 100, required: true },
            { name: 'BluetoothAddress', type: 'varchar', length: 200, required: true },
            { name: 'Status', type: 'varchar', length: 50, required: true },
            { name: 'CurrentCardId', type: 'integer' },
            { name: 'GroupId', type: 'integer' },
            { name: 'LastConnected', type: 'timestamp' },
            { name: 'CreatedAt', type: 'timestamp', required: true },
            { name: 'UpdatedAt', type: 'timestamp', required: true }
        ],
        foreignKeys: [
            { name: 'FK_Devices_Cards_CurrentCardId', column: 'CurrentCardId', referencedTable: 'Cards', referencedColumn: 'Id', onDelete: 'SET NULL' },
            { name: 'FK_Devices_Groups_GroupId', column: 'GroupId', referencedTable: 'Groups', referencedColumn: 'Id', onDelete: 'SET NULL' }
        ],
        indexes: [
            { name: 'IX_Devices_BluetoothAddress', columns: ['BluetoothAddress'], unique: true },
            { name: 'IX_Devices_Status', columns: ['Status'] },
            { name: 'IX_Devices_LastConnected', columns: ['LastConnected'] }
        ]
    }
};

// 生成 SQL 的函數
function generateSQL(provider) {
    const types = TypeMappings[provider];
    let sql = '';
    
    // 添加標題註釋
    sql += `-- SmartNameplate 通用資料庫架構\n`;
    sql += `-- 資料庫提供者: ${provider.toUpperCase()}\n`;
    sql += `-- 生成時間: ${new Date().toISOString()}\n\n`;
    
    // 生成表格
    for (const [tableName, tableDefinition] of Object.entries(TableDefinitions)) {
        sql += generateTable(tableName, tableDefinition, types, provider);
        sql += '\n';
    }
    
    return sql;
}

function generateTable(tableName, definition, types, provider) {
    let sql = `-- 創建表格: ${tableName}\n`;
    sql += `CREATE TABLE ${tableName} (\n`;
    
    // 生成欄位
    const columns = definition.columns.map(col => {
        let columnSQL = `    ${col.name} `;
        
        // 資料類型
        if (col.type === 'varchar' && col.length) {
            columnSQL += types.varchar(col.length);
        } else {
            columnSQL += types[col.type];
        }
        
        // 必填
        if (col.required) {
            columnSQL += ' NOT NULL';
        }
        
        // 預設值
        if (col.default !== undefined) {
            if (provider === DatabaseProviders.POSTGRESQL) {
                columnSQL += ` DEFAULT ${col.default}`;
            } else {
                columnSQL += ` DEFAULT ${col.default}`;
            }
        }
        
        return columnSQL;
    });
    
    sql += columns.join(',\n');
    sql += '\n);\n\n';
    
    // 生成索引
    if (definition.indexes) {
        definition.indexes.forEach(index => {
            sql += `CREATE ${index.unique ? 'UNIQUE ' : ''}INDEX ${index.name} ON ${tableName} (${index.columns.join(', ')});\n`;
        });
        sql += '\n';
    }
    
    // 生成外鍵約束
    if (definition.foreignKeys) {
        definition.foreignKeys.forEach(fk => {
            sql += `ALTER TABLE ${tableName} ADD CONSTRAINT ${fk.name} FOREIGN KEY (${fk.column}) REFERENCES ${fk.referencedTable}(${fk.referencedColumn})`;
            if (fk.onDelete) {
                sql += ` ON DELETE ${fk.onDelete}`;
            }
            sql += ';\n';
        });
        sql += '\n';
    }
    
    return sql;
}

// 生成配置檔案
function generateConfigFile() {
    const config = {
        database: {
            providers: {
                postgresql: {
                    description: "PostgreSQL 資料庫配置",
                    connectionString: "Host=localhost;Database=smart_nameplate;Username=postgres;Password=password",
                    features: ["jsonb", "arrays", "full_text_search"]
                },
                sqlserver: {
                    description: "SQL Server 資料庫配置", 
                    connectionString: "Server=localhost;Database=SmartNameplateDB;Trusted_Connection=true;TrustServerCertificate=true;",
                    features: ["json_functions", "full_text_search", "spatial_data"]
                }
            },
            settings: {
                useNeon: false,
                useSqlServer: false,
                autoMigration: true,
                seedData: true
            }
        }
    };
    
    return JSON.stringify(config, null, 2);
}

// 主執行函數
function main() {
    const outputDir = path.join(__dirname, '../../database-schemas');
    
    // 確保輸出目錄存在
    if (!fs.existsSync(outputDir)) {
        fs.mkdirSync(outputDir, { recursive: true });
    }
    
    // 生成 PostgreSQL 腳本
    const postgresqlSQL = generateSQL(DatabaseProviders.POSTGRESQL);
    fs.writeFileSync(path.join(outputDir, 'postgresql_schema.sql'), postgresqlSQL);
    
    // 生成 SQL Server 腳本
    const sqlserverSQL = generateSQL(DatabaseProviders.SQLSERVER);
    fs.writeFileSync(path.join(outputDir, 'sqlserver_schema.sql'), sqlserverSQL);
    
    // 生成配置檔案
    const configContent = generateConfigFile();
    fs.writeFileSync(path.join(outputDir, 'database_config.json'), configContent);
    
    // 生成說明文件
    const readme = `# SmartNameplate 通用資料庫架構

## 🎯 概述

本資料夾包含 SmartNameplate 系統的通用資料庫架構，支援 PostgreSQL 和 SQL Server。

## 📁 檔案說明

- \`postgresql_schema.sql\` - PostgreSQL 資料庫架構
- \`sqlserver_schema.sql\` - SQL Server 資料庫架構  
- \`database_config.json\` - 資料庫配置範例
- \`README.md\` - 本說明文件

## 🚀 使用方式

### PostgreSQL
\`\`\`bash
psql -U postgres -d smart_nameplate < postgresql_schema.sql
\`\`\`

### SQL Server
\`\`\`bash
sqlcmd -S localhost -i sqlserver_schema.sql
\`\`\`

## ⚙️ 配置

在 \`appsettings.json\` 中設定：

\`\`\`json
{
  "Database": {
    "UseSqlServer": false,  // 設為 true 使用 SQL Server
    "UseNeon": false        // 設為 true 使用 Neon 雲端 PostgreSQL
  }
}
\`\`\`

## 🗂️ 資料表結構

系統包含以下核心表格：
- Users - 用戶管理
- Groups - 群組管理
- Cards - 卡片管理
- Templates - 模板管理
- Devices - 設備管理

所有表格都包含適當的索引和外鍵約束。
`;
    
    fs.writeFileSync(path.join(outputDir, 'README.md'), readme);
    
    console.log('✅ 通用資料庫架構生成完成！');
    console.log(`📁 輸出目錄: ${outputDir}`);
    console.log('📋 生成的檔案:');
    console.log('   - postgresql_schema.sql');
    console.log('   - sqlserver_schema.sql');
    console.log('   - database_config.json');
    console.log('   - README.md');
}

// 執行腳本
if (require.main === module) {
    main();
}

module.exports = { generateSQL, DatabaseProviders, TypeMappings }; 