USE Training;
GO

-- Create UserCredentials table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserCredentials')
BEGIN
    CREATE TABLE UserCredentials (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL,
        PasswordHash NVARCHAR(500) NOT NULL,
        PasswordSalt NVARCHAR(100) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        LastPasswordChange DATETIME2 NULL,
        FailedLoginAttempts INT NOT NULL DEFAULT 0,
        LockedUntil DATETIME2 NULL,
        IsLocked BIT NOT NULL DEFAULT 0,
        
        CONSTRAINT FK_UserCredentials_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
        CONSTRAINT UQ_UserCredentials_UserId UNIQUE (UserId)
    );
    
    CREATE INDEX IX_UserCredentials_UserId ON UserCredentials(UserId);
    
    PRINT 'UserCredentials table created successfully!';
END
ELSE
BEGIN
    PRINT 'UserCredentials table already exists.';
END
GO
