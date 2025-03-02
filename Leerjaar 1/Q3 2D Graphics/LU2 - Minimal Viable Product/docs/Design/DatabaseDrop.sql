-- Drop the foreign key constraints first
ALTER TABLE [dbo].[Object2D] DROP CONSTRAINT [FK_Object2D_Environment2D];
ALTER TABLE [dbo].[Environment2D] DROP CONSTRAINT [FK_Environment2D_AspNetUsers];
ALTER TABLE [auth].[AspNetUserRoles] DROP CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId];
ALTER TABLE [auth].[AspNetUserRoles] DROP CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId];
ALTER TABLE [auth].[AspNetUserLogins] DROP CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId];
ALTER TABLE [auth].[AspNetUserClaims] DROP CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId];
ALTER TABLE [auth].[AspNetRoleClaims] DROP CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId];

-- Drop the tables
DROP TABLE IF EXISTS [dbo].[Object2D];
DROP TABLE IF EXISTS [dbo].[Environment2D];
DROP TABLE IF EXISTS [auth].[AspNetUserTokens];
DROP TABLE IF EXISTS [auth].[AspNetUserRoles];
DROP TABLE IF EXISTS [auth].[AspNetUserLogins];
DROP TABLE IF EXISTS [auth].[AspNetUserClaims];
DROP TABLE IF EXISTS [auth].[AspNetRoles];
DROP TABLE IF EXISTS [auth].[AspNetRoleClaims];
DROP TABLE IF EXISTS [auth].[AspNetUsers];

-- Drop the indexes
DROP INDEX IF EXISTS [RoleNameIndex] ON [auth].[AspNetRoles];
DROP INDEX IF EXISTS [IX_AspNetRoleClaims_RoleId] ON [auth].[AspNetRoleClaims];
DROP INDEX IF EXISTS [UserNameIndex] ON [auth].[AspNetUsers];
DROP INDEX IF EXISTS [EmailIndex] ON [auth].[AspNetUsers];
DROP INDEX IF EXISTS [IX_AspNetUserClaims_UserId] ON [auth].[AspNetUserClaims];
DROP INDEX IF EXISTS [IX_AspNetUserLogins_UserId] ON [auth].[AspNetUserLogins];
DROP INDEX IF EXISTS [IX_AspNetUserRoles_RoleId] ON [auth].[AspNetUserRoles];

-- Drop the schema
DROP SCHEMA IF EXISTS [auth];
