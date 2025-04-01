-- Drop tables in reverse order
DROP TABLE IF EXISTS [dbo].[Treatment_Appoinment];
DROP TABLE IF EXISTS [dbo].[Appoinment];
DROP TABLE IF EXISTS [dbo].[JournalEntry];
DROP TABLE IF EXISTS [dbo].[Patient];
DROP TABLE IF EXISTS [dbo].[Treatment];
DROP TABLE IF EXISTS [dbo].[Guardian];
DROP TABLE IF EXISTS [dbo].[Doctor];
DROP TABLE IF EXISTS [auth].[AspNetUserRoles];
DROP TABLE IF EXISTS [auth].[AspNetRoleClaims];
DROP TABLE IF EXISTS [auth].[AspNetUserClaims];
DROP TABLE IF EXISTS [auth].[AspNetRoles];
DROP TABLE IF EXISTS [auth].[AspNetUsers];

-- Drop schema
DROP SCHEMA IF EXISTS auth;

-- Ensure the auth schema exists
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'auth')
BEGIN
    EXEC('CREATE SCHEMA auth')
END;
GO

-- Doctor Table
CREATE TABLE [dbo].[Doctor] (
    ID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Specialization NVARCHAR(50) NOT NULL
);
GO

-- User Table
CREATE TABLE [auth].[AspNetUsers] (
    [Id]                   NVARCHAR (450)     NOT NULL,
    [UserName]             NVARCHAR (256)     NULL,
    [NormalizedUserName]   NVARCHAR (256)     NULL,
    [Email]                NVARCHAR (256)     NULL,
    [NormalizedEmail]      NVARCHAR (256)     NULL,
    [EmailConfirmed]       BIT                NOT NULL,
    [PasswordHash]         NVARCHAR (MAX)     NULL,
    [SecurityStamp]        NVARCHAR (MAX)     NULL,
    [ConcurrencyStamp]     NVARCHAR (MAX)     NULL,
    [PhoneNumber]          NVARCHAR (MAX)     NULL,
    [PhoneNumberConfirmed] BIT                NOT NULL,
    [TwoFactorEnabled]     BIT                NOT NULL,
    [LockoutEnd]           DATETIMEOFFSET (7) NULL,
    [LockoutEnabled]       BIT                NOT NULL,
    [AccessFailedCount]    INT                NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Guardian Table
CREATE TABLE [dbo].[Guardian] (
    ID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    UserID NVARCHAR(450) NOT NULL,
    FOREIGN KEY (UserID) REFERENCES auth.AspNetUsers(Id)
);
GO

-- Treatment Table
CREATE TABLE [dbo].[Treatment] (
    ID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(50) NOT NULL
);
GO

-- Patient Table
CREATE TABLE [dbo].[Patient] (
    ID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    GuardianID UNIQUEIDENTIFIER NOT NULL,
    TreatmentID UNIQUEIDENTIFIER NOT NULL,
    DoctorID UNIQUEIDENTIFIER NOT NULL,
    Avatar NVARCHAR(100),
    FOREIGN KEY (GuardianID) REFERENCES Guardian(ID),
    FOREIGN KEY (TreatmentID) REFERENCES Treatment(ID),
    FOREIGN KEY (DoctorID) REFERENCES Doctor(ID)
);
GO

-- JournalEntry Table
CREATE TABLE [dbo].[JournalEntry] (
    ID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientID UNIQUEIDENTIFIER NOT NULL,
    GuardianID UNIQUEIDENTIFIER NOT NULL,
    Date DATETIME NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (PatientID) REFERENCES Patient(ID),
    FOREIGN KEY (GuardianID) REFERENCES Guardian(ID)
);
GO

-- Appoinment Table
CREATE TABLE [dbo].[Appoinment] (
    ID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Url NVARCHAR(256),
    Image VARBINARY(MAX),
    DurationInMinutes INT NOT NULL
);
GO

-- Treatment_Appoinment Table
CREATE TABLE [dbo].[Treatment_Appoinment] (
    TreatmentID UNIQUEIDENTIFIER NOT NULL,
    AppoinmentID UNIQUEIDENTIFIER NOT NULL,
    Sequence INT NOT NULL,
    PRIMARY KEY (TreatmentID, AppoinmentID),
    FOREIGN KEY (TreatmentID) REFERENCES Treatment(ID),
    FOREIGN KEY (AppoinmentID) REFERENCES Appoinment(ID)
);
GO

-- Sticker Table
CREATE TABLE [dbo].[Sticker] (
    ID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(50) NOT NULL,
    );
GO

-- StickerCollection Table
CREATE TABLE [dbo].[StickerCollection] (
    ID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Patient(ID),
    StickerID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Sticker(ID),
    UnlockedDate DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- CompletedAppointments Table
CREATE TABLE [dbo].[CompletedAppointments] (
    ID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Patient(ID),
    AppointmentID UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Appoinment(ID),
    CompletedDate DATETIME NOT NULL DEFAULT GETDATE(),
);
GO

CREATE TABLE [auth].[AspNetRoles] (
    [Id]               NVARCHAR (450) NOT NULL,
    [Name]             NVARCHAR (256) NULL,
    [NormalizedName]   NVARCHAR (256) NULL,
    [ConcurrencyStamp] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [auth].[AspNetUserClaims] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [UserId]     NVARCHAR (450) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [auth].[AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [auth].[AspNetRoleClaims] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [RoleId]     NVARCHAR (450) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [auth].[AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [auth].[AspNetUserRoles] (
    [UserId] NVARCHAR (450) NOT NULL,
    [RoleId] NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [auth].[AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [auth].[AspNetUsers] ([Id]) ON DELETE CASCADE
);