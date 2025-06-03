-- SmartNameplate 通用資料庫架構
-- 資料庫提供者: POSTGRESQL
-- 生成時間: 2025-06-03T11:06:40.323Z

-- 創建表格: Users
CREATE TABLE Users (
    Id SERIAL PRIMARY KEY,
    UserName VARCHAR(100) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    PasswordHash VARCHAR(500) NOT NULL,
    Role VARCHAR(50) NOT NULL,
    IsActive BOOLEAN NOT NULL DEFAULT true,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL,
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL
);

CREATE UNIQUE INDEX IX_Users_UserName ON Users (UserName);
CREATE UNIQUE INDEX IX_Users_Email ON Users (Email);
CREATE INDEX IX_Users_Role ON Users (Role);


-- 創建表格: Groups
CREATE TABLE Groups (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Description VARCHAR(500),
    Color VARCHAR(20),
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL,
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL
);

CREATE INDEX IX_Groups_Name ON Groups (Name);
CREATE INDEX IX_Groups_CreatedAt ON Groups (CreatedAt);


-- 創建表格: Cards
CREATE TABLE Cards (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Description VARCHAR(500),
    Status VARCHAR(50) NOT NULL,
    ContentA JSONB,
    ContentB JSONB,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL,
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL
);

CREATE INDEX IX_Cards_Status ON Cards (Status);
CREATE INDEX IX_Cards_CreatedAt ON Cards (CreatedAt);


-- 創建表格: Templates
CREATE TABLE Templates (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description VARCHAR(500),
    Category VARCHAR(100) DEFAULT 'general',
    LayoutDataA JSONB,
    LayoutDataB JSONB,
    Dimensions JSONB,
    IsPublic BOOLEAN NOT NULL DEFAULT false,
    IsActive BOOLEAN NOT NULL DEFAULT true,
    CreatedBy INTEGER,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL,
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL
);

CREATE INDEX IX_Templates_Category ON Templates (Category);
CREATE INDEX IX_Templates_IsPublic ON Templates (IsPublic);
CREATE INDEX IX_Templates_CreatedAt ON Templates (CreatedAt);

ALTER TABLE Templates ADD CONSTRAINT FK_Templates_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(Id) ON DELETE SET NULL;


-- 創建表格: Devices
CREATE TABLE Devices (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    BluetoothAddress VARCHAR(200) NOT NULL,
    Status VARCHAR(50) NOT NULL,
    CurrentCardId INTEGER,
    GroupId INTEGER,
    LastConnected TIMESTAMP WITH TIME ZONE,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL,
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL
);

CREATE UNIQUE INDEX IX_Devices_BluetoothAddress ON Devices (BluetoothAddress);
CREATE INDEX IX_Devices_Status ON Devices (Status);
CREATE INDEX IX_Devices_LastConnected ON Devices (LastConnected);

ALTER TABLE Devices ADD CONSTRAINT FK_Devices_Cards_CurrentCardId FOREIGN KEY (CurrentCardId) REFERENCES Cards(Id) ON DELETE SET NULL;
ALTER TABLE Devices ADD CONSTRAINT FK_Devices_Groups_GroupId FOREIGN KEY (GroupId) REFERENCES Groups(Id) ON DELETE SET NULL;


