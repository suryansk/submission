-- Create Database
CREATE DATABASE Training;
GO

USE Training;
GO

-- Create Users table (Table-per-hierarchy)
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(200) NOT NULL UNIQUE,
    PhoneNumber NVARCHAR(15) NOT NULL,
    DateOfBirth DATETIME2 NOT NULL,
    Address NVARCHAR(500) NOT NULL,
    IdentificationNumber NVARCHAR(20) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1,
    UserType NVARCHAR(50) NOT NULL, -- Discriminator: NormalUser, BankUser
    -- BankUser specific fields
    EmployeeId NVARCHAR(50) NULL,
    Department NVARCHAR(100) NULL,
    Designation NVARCHAR(100) NULL,
    PrimaryBankId INT NULL,
    
    CONSTRAINT CK_Users_UserType CHECK (UserType IN ('NormalUser', 'BankUser')),
    CONSTRAINT CK_Users_BankUser_Fields CHECK (
        (UserType = 'BankUser' AND EmployeeId IS NOT NULL AND Department IS NOT NULL AND Designation IS NOT NULL AND PrimaryBankId IS NOT NULL) OR
        (UserType = 'NormalUser' AND EmployeeId IS NULL AND Department IS NULL AND Designation IS NULL AND PrimaryBankId IS NULL)
    )
);

-- Create Banks table
CREATE TABLE Banks (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    BankCode NVARCHAR(20) NOT NULL UNIQUE,
    SwiftCode NVARCHAR(11) NOT NULL,
    HeadOfficeAddress NVARCHAR(500) NOT NULL,
    ContactNumber NVARCHAR(15) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    EstablishedDate DATETIME2 NOT NULL
);

-- Create Branches table
CREATE TABLE Branches (
    Id INT PRIMARY KEY IDENTITY(1,1),
    BankId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    BranchCode NVARCHAR(20) NOT NULL,
    Address NVARCHAR(500) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(100) NOT NULL,
    PostalCode NVARCHAR(6) NOT NULL,
    ContactNumber NVARCHAR(15) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT FK_Branches_Banks FOREIGN KEY (BankId) REFERENCES Banks(Id),
    CONSTRAINT UQ_Branches_BankCode UNIQUE (BankId, BranchCode)
);

-- Create Currencies table
CREATE TABLE Currencies (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(3) NOT NULL UNIQUE,
    Name NVARCHAR(100) NOT NULL,
    Symbol NVARCHAR(5) NOT NULL,
    ExchangeRateToINR DECIMAL(10,6) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    LastUpdated DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Create Accounts table (Table-per-hierarchy)
CREATE TABLE Accounts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AccountNumber NVARCHAR(20) NOT NULL UNIQUE,
    UserId INT NOT NULL,
    BankId INT NOT NULL,
    BranchId INT NOT NULL,
    CurrencyId INT NOT NULL,
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    MinimumBalance DECIMAL(18,2) NOT NULL DEFAULT 0,
    OpenedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    ClosedDate DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsNRIAccount BIT NOT NULL DEFAULT 0,
    AccountType NVARCHAR(50) NOT NULL, -- Discriminator: SavingAccount, CurrentAccount, TermDepositAccount
    -- SavingAccount specific fields
    InterestRate DECIMAL(5,2) NULL,
    WithdrawalLimit DECIMAL(18,2) NULL,
    MaxWithdrawalsPerMonth INT NULL,
    -- CurrentAccount specific fields
    OverdraftLimit DECIMAL(18,2) NULL,
    OverdraftInterestRate DECIMAL(5,2) NULL,
    -- TermDepositAccount specific fields
    MaturityDate DATETIME2 NULL,
    TermInMonths INT NULL,
    MaturityInterestRate DECIMAL(5,2) NULL,
    IsAutoRenewal BIT NULL,
    
    CONSTRAINT FK_Accounts_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_Accounts_Banks FOREIGN KEY (BankId) REFERENCES Banks(Id),
    CONSTRAINT FK_Accounts_Branches FOREIGN KEY (BranchId) REFERENCES Branches(Id),
    CONSTRAINT FK_Accounts_Currencies FOREIGN KEY (CurrencyId) REFERENCES Currencies(Id),
    CONSTRAINT CK_Accounts_AccountType CHECK (AccountType IN ('SavingAccount', 'CurrentAccount', 'TermDepositAccount')),
    CONSTRAINT CK_Accounts_SavingAccount_Fields CHECK (
        (AccountType IN ('SavingAccount', 'TermDepositAccount') AND InterestRate IS NOT NULL AND WithdrawalLimit IS NOT NULL AND MaxWithdrawalsPerMonth IS NOT NULL) OR
        (AccountType = 'CurrentAccount' AND InterestRate IS NULL AND WithdrawalLimit IS NULL AND MaxWithdrawalsPerMonth IS NULL)
    ),
    CONSTRAINT CK_Accounts_CurrentAccount_Fields CHECK (
        (AccountType = 'CurrentAccount' AND OverdraftLimit IS NOT NULL AND OverdraftInterestRate IS NOT NULL) OR
        (AccountType IN ('SavingAccount', 'TermDepositAccount') AND OverdraftLimit IS NULL AND OverdraftInterestRate IS NULL)
    ),
    CONSTRAINT CK_Accounts_TermDeposit_Fields CHECK (
        (AccountType = 'TermDepositAccount' AND MaturityDate IS NOT NULL AND TermInMonths IS NOT NULL AND MaturityInterestRate IS NOT NULL AND IsAutoRenewal IS NOT NULL) OR
        (AccountType IN ('SavingAccount', 'CurrentAccount') AND MaturityDate IS NULL AND TermInMonths IS NULL AND MaturityInterestRate IS NULL AND IsAutoRenewal IS NULL)
    )
);

-- Create Roles table
CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Create Permissions table
CREATE TABLE Permissions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(200) NULL,
    Module NVARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Create UserRoles table
CREATE TABLE UserRoles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    BankId INT NULL,
    AccountId INT NULL,
    AssignedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    ExpiryDate DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id),
    CONSTRAINT FK_UserRoles_Banks FOREIGN KEY (BankId) REFERENCES Banks(Id),
    CONSTRAINT FK_UserRoles_Accounts FOREIGN KEY (AccountId) REFERENCES Accounts(Id)
);

-- Create RolePermissions table
CREATE TABLE RolePermissions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT FK_RolePermissions_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id),
    CONSTRAINT FK_RolePermissions_Permissions FOREIGN KEY (PermissionId) REFERENCES Permissions(Id),
    CONSTRAINT UQ_RolePermissions UNIQUE (RoleId, PermissionId)
);

-- Create GuardianRelationships table
CREATE TABLE GuardianRelationships (
    Id INT PRIMARY KEY IDENTITY(1,1),
    GuardianUserId INT NOT NULL,
    MinorUserId INT NOT NULL,
    RelationshipType NVARCHAR(50) NOT NULL,
    EstablishedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    ExpiryDate DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT FK_GuardianRelationships_Guardian FOREIGN KEY (GuardianUserId) REFERENCES Users(Id),
    CONSTRAINT FK_GuardianRelationships_Minor FOREIGN KEY (MinorUserId) REFERENCES Users(Id),
    CONSTRAINT CK_GuardianRelationships_DifferentUsers CHECK (GuardianUserId != MinorUserId)
);

-- Add foreign key constraint for Users.PrimaryBankId (must be added after Banks table is created)
ALTER TABLE Users
ADD CONSTRAINT FK_Users_PrimaryBank FOREIGN KEY (PrimaryBankId) REFERENCES Banks(Id);

-- Create Indexes for better performance
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_UserType ON Users(UserType);
CREATE INDEX IX_Accounts_AccountNumber ON Accounts(AccountNumber);
CREATE INDEX IX_Accounts_UserId ON Accounts(UserId);
CREATE INDEX IX_Accounts_BankId ON Accounts(BankId);
CREATE INDEX IX_UserRoles_UserId ON UserRoles(UserId);
CREATE INDEX IX_UserRoles_BankId ON UserRoles(BankId);

-- Insert default currencies
INSERT INTO Currencies (Code, Name, Symbol, ExchangeRateToINR) VALUES
('INR', 'Indian Rupee', '₹', 1.000000),
('USD', 'US Dollar', '$', 83.250000),
('EUR', 'Euro', '€', 90.500000),
('GBP', 'British Pound', '£', 105.750000),
('AED', 'UAE Dirham', 'د.إ', 22.650000),
('SGD', 'Singapore Dollar', 'S$', 61.800000);

-- Insert default roles
INSERT INTO Roles (Name, Description) VALUES
('ACCOUNT_HOLDER', 'Primary account holder with full access'),
('POA', 'Power of Attorney with limited access'),
('GUARDIAN', 'Guardian for minor accounts'),
('BANK_MANAGER', 'Bank manager with administrative access'),
('BANK_EMPLOYEE', 'Bank employee with operational access'),
('ADMIN', 'System administrator with full access');

-- Insert default permissions
INSERT INTO Permissions (Name, Description, Module) VALUES
-- User permissions
('CREATE_USER', 'Create new user accounts', 'USER'),
('UPDATE_USER', 'Update user information', 'USER'),
('DELETE_USER', 'Delete user accounts', 'USER'),
('READ_USER', 'View user information', 'USER'),
-- Account permissions
('CREATE_ACCOUNT', 'Open new bank accounts', 'ACCOUNT'),
('UPDATE_ACCOUNT', 'Modify account details', 'ACCOUNT'),
('DELETE_ACCOUNT', 'Close bank accounts', 'ACCOUNT'),
('READ_ACCOUNT', 'View account information', 'ACCOUNT'),
-- Role CRUD permissions
('CREATE_ROLE', 'Create new roles', 'ROLE'),
('READ_ROLE', 'View role information', 'ROLE'),
('UPDATE_ROLE', 'Update role information', 'ROLE'),
('DELETE_ROLE', 'Delete roles', 'ROLE'),
('ASSIGN_ROLE', 'Assign roles to users', 'ROLE'),
('REVOKE_ROLE', 'Revoke roles from users', 'ROLE'),
-- Permission CRUD permissions
('CREATE_PERMISSION', 'Create new permissions', 'PERMISSION'),
('READ_PERMISSION', 'View permission information', 'PERMISSION'),
('UPDATE_PERMISSION', 'Update permission information', 'PERMISSION'),
('DELETE_PERMISSION', 'Delete permissions', 'PERMISSION'),
('ASSIGN_PERMISSION', 'Assign permissions to roles', 'PERMISSION'),
('REVOKE_PERMISSION', 'Revoke permissions from roles', 'PERMISSION'),
-- Administrative permissions
('MANAGE_ROLES', 'Manage user roles and permissions', 'ADMIN'),
('MANAGE_BANKS', 'Manage bank and branch information', 'ADMIN'),
('MANAGE_CURRENCIES', 'Manage currencies and exchange rates', 'ADMIN'),
('MANAGE_SYSTEM', 'Full system administration access', 'ADMIN');

-- Insert default role-permission mappings
-- ACCOUNT_HOLDER permissions
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id FROM Roles r, Permissions p 
WHERE r.Name = 'ACCOUNT_HOLDER' AND p.Name IN ('READ_USER', 'UPDATE_USER', 'READ_ACCOUNT');

-- GUARDIAN permissions
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id FROM Roles r, Permissions p 
WHERE r.Name = 'GUARDIAN' AND p.Name IN ('READ_USER', 'UPDATE_USER', 'READ_ACCOUNT');

-- BANK_EMPLOYEE permissions
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id FROM Roles r, Permissions p 
WHERE r.Name = 'BANK_EMPLOYEE' AND p.Name IN ('CREATE_USER', 'READ_USER', 'UPDATE_USER', 'CREATE_ACCOUNT', 'READ_ACCOUNT', 'UPDATE_ACCOUNT', 'READ_ROLE', 'READ_PERMISSION');

-- BANK_MANAGER permissions
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id FROM Roles r, Permissions p 
WHERE r.Name = 'BANK_MANAGER' AND p.Name IN ('CREATE_USER', 'READ_USER', 'UPDATE_USER', 'DELETE_USER', 'CREATE_ACCOUNT', 'READ_ACCOUNT', 'UPDATE_ACCOUNT', 'DELETE_ACCOUNT', 'CREATE_ROLE', 'READ_ROLE', 'UPDATE_ROLE', 'DELETE_ROLE', 'ASSIGN_ROLE', 'REVOKE_ROLE', 'READ_PERMISSION', 'ASSIGN_PERMISSION', 'REVOKE_PERMISSION', 'MANAGE_ROLES');

-- ADMIN permissions (all permissions)
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id FROM Roles r, Permissions p 
WHERE r.Name = 'ADMIN';

-- Insert sample banks for testing CRUD operations
INSERT INTO Banks (Name, BankCode, SwiftCode, HeadOfficeAddress, ContactNumber, Email, EstablishedDate) VALUES
('State Bank of India', 'SBI', 'SBININBB', 'State Bank Bhavan, Nariman Point, Mumbai 400021', '+919876543210', 'contact@sbi.co.in', '1955-07-01'),
('HDFC Bank', 'HDFC', 'HDFCINBB', 'HDFC Bank House, Senapati Bapat Marg, Lower Parel, Mumbai 400013', '+919876543211', 'customercare@hdfcbank.com', '1994-08-01'),
('ICICI Bank', 'ICICI', 'ICICINBB', 'ICICI Bank Towers, Bandra Kurla Complex, Mumbai 400051', '+919876543212', 'customer.care@icicibank.com', '1994-05-05');

-- Insert sample branches
INSERT INTO Branches (BankId, Name, BranchCode, Address, City, State, PostalCode, ContactNumber) VALUES
(1, 'Mumbai Main Branch', 'SBI001', 'Nariman Point, Mumbai', 'Mumbai', 'Maharashtra', '400021', '+912233445566'),
(1, 'Delhi Connaught Place', 'SBI002', 'Connaught Place, New Delhi', 'New Delhi', 'Delhi', '110001', '+911123445566'),
(2, 'Mumbai Bandra', 'HDFC001', 'Bandra West, Mumbai', 'Mumbai', 'Maharashtra', '400050', '+912233445567'),
(2, 'Bangalore Koramangala', 'HDFC002', 'Koramangala, Bangalore', 'Bangalore', 'Karnataka', '560034', '+918033445567'),
(3, 'Chennai Anna Nagar', 'ICICI001', 'Anna Nagar, Chennai', 'Chennai', 'Tamil Nadu', '600040', '+914433445568'),
(3, 'Pune FC Road', 'ICICI002', 'Fergusson College Road, Pune', 'Pune', 'Maharashtra', '411004', '+912033445568');

-- Create stored procedures for Role CRUD operations
GO

-- Create Role procedure
CREATE PROCEDURE sp_CreateRole
    @Name NVARCHAR(50),
    @Description NVARCHAR(200) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (SELECT 1 FROM Roles WHERE Name = @Name)
    BEGIN
        RAISERROR('Role with this name already exists', 16, 1);
        RETURN;
    END
    
    INSERT INTO Roles (Name, Description, IsActive)
    VALUES (@Name, @Description, @IsActive);
    
    SELECT * FROM Roles WHERE Id = SCOPE_IDENTITY();
END
GO

-- Read Role procedure
CREATE PROCEDURE sp_GetRole
    @Id INT = NULL,
    @Name NVARCHAR(50) = NULL,
    @IncludeInactive BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT r.*, 
           COUNT(ur.Id) as ActiveUserCount,
           COUNT(rp.Id) as PermissionCount
    FROM Roles r
    LEFT JOIN UserRoles ur ON r.Id = ur.RoleId AND ur.IsActive = 1
    LEFT JOIN RolePermissions rp ON r.Id = rp.RoleId AND rp.IsActive = 1
    WHERE (@Id IS NULL OR r.Id = @Id)
      AND (@Name IS NULL OR r.Name = @Name)
      AND (@IncludeInactive = 1 OR r.IsActive = 1)
    GROUP BY r.Id, r.Name, r.Description, r.IsActive
    ORDER BY r.Name;
END
GO

-- Update Role procedure
CREATE PROCEDURE sp_UpdateRole
    @Id INT,
    @Name NVARCHAR(50) = NULL,
    @Description NVARCHAR(200) = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = @Id)
    BEGIN
        RAISERROR('Role not found', 16, 1);
        RETURN;
    END
    
    IF @Name IS NOT NULL AND EXISTS (SELECT 1 FROM Roles WHERE Name = @Name AND Id != @Id)
    BEGIN
        RAISERROR('Role with this name already exists', 16, 1);
        RETURN;
    END
    
    UPDATE Roles
    SET Name = ISNULL(@Name, Name),
        Description = ISNULL(@Description, Description),
        IsActive = ISNULL(@IsActive, IsActive)
    WHERE Id = @Id;
    
    SELECT * FROM Roles WHERE Id = @Id;
END
GO

-- Delete Role procedure (soft delete)
CREATE PROCEDURE sp_DeleteRole
    @Id INT,
    @ForceDelete BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = @Id)
    BEGIN
        RAISERROR('Role not found', 16, 1);
        RETURN;
    END
    
    -- Check if role is assigned to users
    IF EXISTS (SELECT 1 FROM UserRoles WHERE RoleId = @Id AND IsActive = 1) AND @ForceDelete = 0
    BEGIN
        RAISERROR('Cannot delete role that is assigned to active users. Use ForceDelete to override.', 16, 1);
        RETURN;
    END
    
    -- Soft delete role
    UPDATE Roles SET IsActive = 0 WHERE Id = @Id;
    
    -- Deactivate all user role assignments
    UPDATE UserRoles SET IsActive = 0 WHERE RoleId = @Id;
    
    -- Deactivate all role permissions
    UPDATE RolePermissions SET IsActive = 0 WHERE RoleId = @Id;
    
    SELECT 'Role deleted successfully' as Message;
END
GO

-- Assign Permission to Role procedure
CREATE PROCEDURE sp_AssignPermissionToRole
    @RoleId INT,
    @PermissionId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = @RoleId AND IsActive = 1)
    BEGIN
        RAISERROR('Role not found or inactive', 16, 1);
        RETURN;
    END
    
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Id = @PermissionId AND IsActive = 1)
    BEGIN
        RAISERROR('Permission not found or inactive', 16, 1);
        RETURN;
    END
    
    -- Check if already assigned
    IF EXISTS (SELECT 1 FROM RolePermissions WHERE RoleId = @RoleId AND PermissionId = @PermissionId AND IsActive = 1)
    BEGIN
        SELECT 'Permission already assigned to role' as Message;
        RETURN;
    END
    
    -- Reactivate if exists but inactive
    IF EXISTS (SELECT 1 FROM RolePermissions WHERE RoleId = @RoleId AND PermissionId = @PermissionId AND IsActive = 0)
    BEGIN
        UPDATE RolePermissions SET IsActive = 1 WHERE RoleId = @RoleId AND PermissionId = @PermissionId;
    END
    ELSE
    BEGIN
        INSERT INTO RolePermissions (RoleId, PermissionId, IsActive) VALUES (@RoleId, @PermissionId, 1);
    END
    
    SELECT 'Permission assigned to role successfully' as Message;
END
GO

-- Revoke Permission from Role procedure
CREATE PROCEDURE sp_RevokePermissionFromRole
    @RoleId INT,
    @PermissionId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE RoleId = @RoleId AND PermissionId = @PermissionId AND IsActive = 1)
    BEGIN
        RAISERROR('Permission assignment not found', 16, 1);
        RETURN;
    END
    
    UPDATE RolePermissions SET IsActive = 0 WHERE RoleId = @RoleId AND PermissionId = @PermissionId;
    
    SELECT 'Permission revoked from role successfully' as Message;
END
GO

-- ...existing code...

PRINT 'Database Training created successfully with all tables, initial data, and Role CRUD procedures!';
