CREATE DATABASE HealthQuest
GO

USE HealthQuest
GO

CREATE SCHEMA auth;
GO

CREATE TABLE [dbo].[Doctor] (
    ID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Specialization NVARCHAR(50) NOT NULL
);

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

CREATE TABLE [dbo].[Guardian] (
    ID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    UserID NVARCHAR(450) NOT NULL,
    FOREIGN KEY (UserID) REFERENCES auth.AspNetUsers(Id)
);

CREATE TABLE [dbo].[Treatment] (
    ID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(50) NOT NULL
);

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

CREATE TABLE [dbo].[JournalEntry] (
    ID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientID UNIQUEIDENTIFIER NOT NULL,
    GuardianID UNIQUEIDENTIFIER NOT NULL,
    Date DATETIME NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (PatientID) REFERENCES Patient(ID),
    FOREIGN KEY (GuardianID) REFERENCES Guardian(ID)
);

CREATE TABLE [dbo].[Appoinment] (
    ID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(50) NOT NULL,
    Url NVARCHAR(256),
    Image VARBINARY(MAX),
    DurationInMinutes INT NOT NULL
);

CREATE TABLE [dbo].[Treatment_Appoinment] (
    TreatmentID UNIQUEIDENTIFIER NOT NULL,
    AppoinmentID UNIQUEIDENTIFIER NOT NULL,
    Sequence INT NOT NULL,
    PRIMARY KEY (TreatmentID, AppoinmentID),
    FOREIGN KEY (TreatmentID) REFERENCES Treatment(ID),
    FOREIGN KEY (AppoinmentID) REFERENCES Appoinment(ID)
);

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