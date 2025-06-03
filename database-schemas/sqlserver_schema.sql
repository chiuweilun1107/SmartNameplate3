-- SmartNameplate 通用資料庫架構
-- 資料庫提供者: SQLSERVER
-- 生成時間: 2025-06-03T11:06:40.328Z

-- 創建表格: Users
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    Role NVARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL DEFAULT true,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL
);

CREATE UNIQUE INDEX IX_Users_UserName ON Users (UserName);
CREATE UNIQUE INDEX IX_Users_Email ON Users (Email);
CREATE INDEX IX_Users_Role ON Users (Role);


-- 創建表格: Groups
CREATE TABLE Groups (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Color NVARCHAR(20),
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL
);

CREATE INDEX IX_Groups_Name ON Groups (Name);
CREATE INDEX IX_Groups_CreatedAt ON Groups (CreatedAt);


-- 創建表格: Cards
CREATE TABLE Cards (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Status NVARCHAR(50) NOT NULL,
    ContentA NVARCHAR(MAX),
    ContentB NVARCHAR(MAX),
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL
);

CREATE INDEX IX_Cards_Status ON Cards (Status);
CREATE INDEX IX_Cards_CreatedAt ON Cards (CreatedAt);


-- 創建表格: Templates
CREATE TABLE Templates (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(500),
    Category NVARCHAR(100) DEFAULT 'general',
    LayoutDataA NVARCHAR(MAX),
    LayoutDataB NVARCHAR(MAX),
    Dimensions NVARCHAR(MAX),
    IsPublic BIT NOT NULL DEFAULT false,
    IsActive BIT NOT NULL DEFAULT true,
    CreatedBy INT,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL
);

CREATE INDEX IX_Templates_Category ON Templates (Category);
CREATE INDEX IX_Templates_IsPublic ON Templates (IsPublic);
CREATE INDEX IX_Templates_CreatedAt ON Templates (CreatedAt);

ALTER TABLE Templates ADD CONSTRAINT FK_Templates_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(Id) ON DELETE SET NULL;


-- 創建表格: Devices
CREATE TABLE Devices (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    BluetoothAddress NVARCHAR(200) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CurrentCardId INT,
    GroupId INT,
    LastConnected DATETIME2,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL
);

CREATE UNIQUE INDEX IX_Devices_BluetoothAddress ON Devices (BluetoothAddress);
CREATE INDEX IX_Devices_Status ON Devices (Status);
CREATE INDEX IX_Devices_LastConnected ON Devices (LastConnected);

ALTER TABLE Devices ADD CONSTRAINT FK_Devices_Cards_CurrentCardId FOREIGN KEY (CurrentCardId) REFERENCES Cards(Id) ON DELETE SET NULL;
ALTER TABLE Devices ADD CONSTRAINT FK_Devices_Groups_GroupId FOREIGN KEY (GroupId) REFERENCES Groups(Id) ON DELETE SET NULL;


