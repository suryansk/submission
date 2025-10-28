CREATE DATABASE Training;

USE Training;

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
    UserType NVARCHAR(50) NOT NULL,
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

CREATE TABLE Currencies (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(3) NOT NULL UNIQUE,
    Name NVARCHAR(100) NOT NULL,
    Symbol NVARCHAR(5) NOT NULL,
    ExchangeRateToINR DECIMAL(10,6) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    LastUpdated DATETIME2 NOT NULL DEFAULT GETDATE()
);

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
    AccountType NVARCHAR(50) NOT NULL, 
    -- Use for Saving Account 
    InterestRate DECIMAL(5,2) NULL,
    WithdrawalLimit DECIMAL(18,2) NULL,
    MaxWithdrawalsPerMonth INT NULL,
    -- Use for Current Account
    OverdraftLimit DECIMAL(18,2) NULL,
    OverdraftInterestRate DECIMAL(5,2) NULL,
    -- Use for TermDeposit Account
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

CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE Permissions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(200) NULL,
    Module NVARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

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

CREATE TABLE RolePermissions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT FK_RolePermissions_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id),
    CONSTRAINT FK_RolePermissions_Permissions FOREIGN KEY (PermissionId) REFERENCES Permissions(Id),
    CONSTRAINT UQ_RolePermissions UNIQUE (RoleId, PermissionId)
);

CREATE TABLE PowerOfAttorneys (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AccountId INT NOT NULL,
    GrantorUserId INT NOT NULL,
    AttorneyUserId INT NOT NULL,
    GrantedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    ExpiryDate DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    LimitationsDescription NVARCHAR(500) NULL,
    
    CONSTRAINT FK_PowerOfAttorneys_Accounts FOREIGN KEY (AccountId) REFERENCES Accounts(Id),
    CONSTRAINT FK_PowerOfAttorneys_Grantor FOREIGN KEY (GrantorUserId) REFERENCES Users(Id),
    CONSTRAINT FK_PowerOfAttorneys_Attorney FOREIGN KEY (AttorneyUserId) REFERENCES Users(Id),
    CONSTRAINT CK_PowerOfAttorneys_DifferentUsers CHECK (GrantorUserId != AttorneyUserId)
);

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

CREATE TABLE Transactions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AccountId INT NOT NULL,
    TransactionType NVARCHAR(50) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    BalanceAfterTransaction DECIMAL(18,2) NOT NULL,
    Description NVARCHAR(500) NULL,
    ReferenceNumber NVARCHAR(50) NOT NULL UNIQUE,
    TransactionDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(20) NOT NULL,
    OriginalCurrencyId INT NULL,
    OriginalAmount DECIMAL(18,2) NULL,
    ExchangeRate DECIMAL(10,6) NULL,
    InitiatedByUserId INT NULL,
    
    CONSTRAINT FK_Transactions_Accounts FOREIGN KEY (AccountId) REFERENCES Accounts(Id),
    CONSTRAINT FK_Transactions_OriginalCurrency FOREIGN KEY (OriginalCurrencyId) REFERENCES Currencies(Id),
    CONSTRAINT FK_Transactions_InitiatedBy FOREIGN KEY (InitiatedByUserId) REFERENCES Users(Id)
);


ALTER TABLE Users
ADD CONSTRAINT FK_Users_PrimaryBank FOREIGN KEY (PrimaryBankId) REFERENCES Banks(Id);

CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_UserType ON Users(UserType);
CREATE INDEX IX_Accounts_AccountNumber ON Accounts(AccountNumber);
CREATE INDEX IX_Accounts_UserId ON Accounts(UserId);
CREATE INDEX IX_Accounts_BankId ON Accounts(BankId);
CREATE INDEX IX_Transactions_AccountId ON Transactions(AccountId);
CREATE INDEX IX_Transactions_TransactionDate ON Transactions(TransactionDate);
CREATE INDEX IX_UserRoles_UserId ON UserRoles(UserId);
CREATE INDEX IX_UserRoles_BankId ON UserRoles(BankId);
