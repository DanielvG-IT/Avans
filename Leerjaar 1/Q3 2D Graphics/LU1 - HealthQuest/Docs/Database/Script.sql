/****** Object:  Schema [auth]    Script Date: 17/04/2025 17:53:20 ******/
CREATE SCHEMA [auth]
GO
/****** Object:  Table [auth].[AspNetRoleClaims]    Script Date: 17/04/2025 17:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [auth].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [auth].[AspNetRoles]    Script Date: 17/04/2025 17:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [auth].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [auth].[AspNetUserClaims]    Script Date: 17/04/2025 17:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [auth].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [auth].[AspNetUserRoles]    Script Date: 17/04/2025 17:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [auth].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [auth].[AspNetUsers]    Script Date: 17/04/2025 17:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [auth].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Appointment]    Script Date: 17/04/2025 17:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Appointment](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Url] [nvarchar](256) NULL,
	[Image] [varbinary](max) NULL,
	[DurationInMinutes] [int] NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[TreatmentID] [uniqueidentifier] NULL,
	[Sequence] [int] NULL,
 CONSTRAINT [PK__Appoinme__3214EC272822E26A] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Doctor]    Script Date: 17/04/2025 17:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Doctor](
	[ID] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Specialization] [nvarchar](50) NOT NULL,
	[UserID] [nvarchar](450) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Guardian]    Script Date: 17/04/2025 17:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Guardian](
	[ID] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[UserID] [nvarchar](450) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[JournalEntry]    Script Date: 17/04/2025 17:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JournalEntry](
	[ID] [uniqueidentifier] NOT NULL,
	[PatientID] [uniqueidentifier] NOT NULL,
	[Date] [varchar](50) NOT NULL,
	[Content] [text] NOT NULL,
	[Title] [nvarchar](25) NOT NULL,
	[Rating] [int] NOT NULL,
 CONSTRAINT [PK__JournalE__3214EC27AE57AFB6] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Patient]    Script Date: 17/04/2025 17:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Patient](
	[ID] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[GuardianID] [uniqueidentifier] NOT NULL,
	[TreatmentID] [uniqueidentifier] NOT NULL,
	[DoctorID] [uniqueidentifier] NULL,
	[Avatar] [nvarchar](100) NOT NULL,
	[DoctorAccessJournal] [bit] NOT NULL,
	[GuardianAccessJournal] [bit] NOT NULL,
 CONSTRAINT [PK__Patient__3214EC276CCB6B86] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PersonalAppointments]    Script Date: 17/04/2025 17:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PersonalAppointments](
	[ID] [uniqueidentifier] NOT NULL,
	[PatientID] [uniqueidentifier] NOT NULL,
	[AppointmentID] [uniqueidentifier] NOT NULL,
	[AppointmentDate] [varchar](50) NULL,
	[CompletedDate] [varchar](50) NULL,
	[CompletedQuestion] [bit] NOT NULL,
	[Sequence] [int] NOT NULL,
 CONSTRAINT [PK__Complete__3214EC2714067360] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sticker]    Script Date: 17/04/2025 17:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sticker](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StickerCollection]    Script Date: 17/04/2025 17:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StickerCollection](
	[ID] [uniqueidentifier] NOT NULL,
	[PatientID] [uniqueidentifier] NOT NULL,
	[StickerID] [uniqueidentifier] NOT NULL,
	[UnlockedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Treatment]    Script Date: 17/04/2025 17:53:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Treatment](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Appointment] ADD  CONSTRAINT [DF__Appoinment__ID__3F115E1A]  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[Doctor] ADD  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[Guardian] ADD  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[JournalEntry] ADD  CONSTRAINT [DF__JournalEntry__ID__3A4CA8FD]  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[Patient] ADD  CONSTRAINT [DF__Patient__ID__3493CFA7]  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[Patient] ADD  CONSTRAINT [DF_Patient_DoctorAccessJournal]  DEFAULT ((0)) FOR [DoctorAccessJournal]
GO
ALTER TABLE [dbo].[Patient] ADD  CONSTRAINT [DF_Patient_GuardianAccessJournal]  DEFAULT ((0)) FOR [GuardianAccessJournal]
GO
ALTER TABLE [dbo].[PersonalAppointments] ADD  CONSTRAINT [DF__CompletedApp__ID__4E53A1AA]  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[PersonalAppointments] ADD  CONSTRAINT [DF__Completed__Compl__51300E55]  DEFAULT (getdate()) FOR [CompletedDate]
GO
ALTER TABLE [dbo].[PersonalAppointments] ADD  CONSTRAINT [DF_CompletedAppointments_completedQuestion]  DEFAULT ((0)) FOR [CompletedQuestion]
GO
ALTER TABLE [dbo].[Sticker] ADD  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[StickerCollection] ADD  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[StickerCollection] ADD  DEFAULT (getdate()) FOR [UnlockedDate]
GO
ALTER TABLE [dbo].[Treatment] ADD  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [auth].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [auth].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [auth].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [auth].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [auth].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [auth].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [auth].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [auth].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [auth].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [auth].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [auth].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [auth].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Appointment]  WITH CHECK ADD  CONSTRAINT [FK_Appointment_Treatment] FOREIGN KEY([TreatmentID])
REFERENCES [dbo].[Treatment] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Appointment] CHECK CONSTRAINT [FK_Appointment_Treatment]
GO
ALTER TABLE [dbo].[Doctor]  WITH CHECK ADD  CONSTRAINT [FK_Doctor_AspNetUsers] FOREIGN KEY([UserID])
REFERENCES [auth].[AspNetUsers] ([Id])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Doctor] CHECK CONSTRAINT [FK_Doctor_AspNetUsers]
GO
ALTER TABLE [dbo].[Guardian]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [auth].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[JournalEntry]  WITH CHECK ADD  CONSTRAINT [FK__JournalEn__Patie__3B40CD36] FOREIGN KEY([PatientID])
REFERENCES [dbo].[Patient] ([ID])
GO
ALTER TABLE [dbo].[JournalEntry] CHECK CONSTRAINT [FK__JournalEn__Patie__3B40CD36]
GO
ALTER TABLE [dbo].[Patient]  WITH CHECK ADD  CONSTRAINT [FK__Patient__DoctorI__37703C52] FOREIGN KEY([DoctorID])
REFERENCES [dbo].[Doctor] ([ID])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Patient] CHECK CONSTRAINT [FK__Patient__DoctorI__37703C52]
GO
ALTER TABLE [dbo].[Patient]  WITH CHECK ADD  CONSTRAINT [FK__Patient__Guardia__3587F3E0] FOREIGN KEY([GuardianID])
REFERENCES [dbo].[Guardian] ([ID])
GO
ALTER TABLE [dbo].[Patient] CHECK CONSTRAINT [FK__Patient__Guardia__3587F3E0]
GO
ALTER TABLE [dbo].[Patient]  WITH CHECK ADD  CONSTRAINT [FK__Patient__Treatme__367C1819] FOREIGN KEY([TreatmentID])
REFERENCES [dbo].[Treatment] ([ID])
GO
ALTER TABLE [dbo].[Patient] CHECK CONSTRAINT [FK__Patient__Treatme__367C1819]
GO
ALTER TABLE [dbo].[PersonalAppointments]  WITH CHECK ADD  CONSTRAINT [FK_PersonalAppointments_Appointment] FOREIGN KEY([AppointmentID])
REFERENCES [dbo].[Appointment] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PersonalAppointments] CHECK CONSTRAINT [FK_PersonalAppointments_Appointment]
GO
ALTER TABLE [dbo].[PersonalAppointments]  WITH CHECK ADD  CONSTRAINT [FK_PersonalAppointments_Patient] FOREIGN KEY([PatientID])
REFERENCES [dbo].[Patient] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PersonalAppointments] CHECK CONSTRAINT [FK_PersonalAppointments_Patient]
GO
ALTER TABLE [dbo].[StickerCollection]  WITH CHECK ADD  CONSTRAINT [FK__StickerCo__Patie__498EEC8D] FOREIGN KEY([PatientID])
REFERENCES [dbo].[Patient] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StickerCollection] CHECK CONSTRAINT [FK__StickerCo__Patie__498EEC8D]
GO
ALTER TABLE [dbo].[StickerCollection]  WITH CHECK ADD FOREIGN KEY([StickerID])
REFERENCES [dbo].[Sticker] ([ID])
GO
