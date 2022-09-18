
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 09/15/2022 10:13:47
-- Generated from EDMX file: D:\cagatay\CRMProject2\CRMProject.Data\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [CrmDbTest];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Step_TaskSet1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Step] DROP CONSTRAINT [FK_Step_TaskSet1];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskSet_Personels1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskSet] DROP CONSTRAINT [FK_TaskSet_Personels1];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskSet_ProblemSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskSet] DROP CONSTRAINT [FK_TaskSet_ProblemSet];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskSet_Roles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskSet] DROP CONSTRAINT [FK_TaskSet_Roles];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskUser_TaskSet]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskUser] DROP CONSTRAINT [FK_TaskUser_TaskSet];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskUser_Users]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskUser] DROP CONSTRAINT [FK_TaskUser_Users];
GO
IF OBJECT_ID(N'[dbo].[FK_UserRole_Roles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserRole] DROP CONSTRAINT [FK_UserRole_Roles];
GO
IF OBJECT_ID(N'[dbo].[FK_UserRole_Users]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserRole] DROP CONSTRAINT [FK_UserRole_Users];
GO
IF OBJECT_ID(N'[dbo].[FK_Users_Roles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_Users_Roles];
GO
IF OBJECT_ID(N'[dbo].[FK_Users_Roles1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_Users_Roles1];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Personels]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Personels];
GO
IF OBJECT_ID(N'[dbo].[ProblemSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProblemSet];
GO
IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roles];
GO
IF OBJECT_ID(N'[dbo].[Step]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Step];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO
IF OBJECT_ID(N'[dbo].[TaskSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TaskSet];
GO
IF OBJECT_ID(N'[dbo].[TaskUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TaskUser];
GO
IF OBJECT_ID(N'[dbo].[UserRole]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserRole];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Personels'
CREATE TABLE [dbo].[Personels] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100)  NOT NULL
);
GO

-- Creating table 'ProblemSet'
CREATE TABLE [dbo].[ProblemSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Step'
CREATE TABLE [dbo].[Step] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [StepDetail] nvarchar(50)  NULL,
    [taskId] int  NULL,
    [IsDone] bit  NULL
);
GO

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- Creating table 'TaskSet'
CREATE TABLE [dbo].[TaskSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProblemId] int  NULL,
    [PersonelId] int  NULL,
    [RoleId] int  NULL,
    [IsSelected] bit  NULL,
    [Deadline] datetime  NULL,
    [AtayanKullanici] nvarchar(50)  NULL,
    [GirenKullaniciRolu] nvarchar(50)  NULL
);
GO

-- Creating table 'TaskUser'
CREATE TABLE [dbo].[TaskUser] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [taskId] int  NOT NULL,
    [userId] int  NULL
);
GO

-- Creating table 'UserRole'
CREATE TABLE [dbo].[UserRole] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [userId] int  NULL,
    [roleId] int  NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Role] nvarchar(50)  NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserName] nvarchar(250)  NULL,
    [Password] nvarchar(250)  NULL,
    [RoleId] int  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Personels'
ALTER TABLE [dbo].[Personels]
ADD CONSTRAINT [PK_Personels]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProblemSet'
ALTER TABLE [dbo].[ProblemSet]
ADD CONSTRAINT [PK_ProblemSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Step'
ALTER TABLE [dbo].[Step]
ADD CONSTRAINT [PK_Step]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [Id] in table 'TaskSet'
ALTER TABLE [dbo].[TaskSet]
ADD CONSTRAINT [PK_TaskSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TaskUser'
ALTER TABLE [dbo].[TaskUser]
ADD CONSTRAINT [PK_TaskUser]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserRole'
ALTER TABLE [dbo].[UserRole]
ADD CONSTRAINT [PK_UserRole]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [PersonelId] in table 'TaskSet'
ALTER TABLE [dbo].[TaskSet]
ADD CONSTRAINT [FK_TaskSet_Personels1]
    FOREIGN KEY ([PersonelId])
    REFERENCES [dbo].[Personels]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskSet_Personels1'
CREATE INDEX [IX_FK_TaskSet_Personels1]
ON [dbo].[TaskSet]
    ([PersonelId]);
GO

-- Creating foreign key on [ProblemId] in table 'TaskSet'
ALTER TABLE [dbo].[TaskSet]
ADD CONSTRAINT [FK_TaskSet_ProblemSet]
    FOREIGN KEY ([ProblemId])
    REFERENCES [dbo].[ProblemSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskSet_ProblemSet'
CREATE INDEX [IX_FK_TaskSet_ProblemSet]
ON [dbo].[TaskSet]
    ([ProblemId]);
GO

-- Creating foreign key on [taskId] in table 'Step'
ALTER TABLE [dbo].[Step]
ADD CONSTRAINT [FK_Step_TaskSet1]
    FOREIGN KEY ([taskId])
    REFERENCES [dbo].[TaskSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Step_TaskSet1'
CREATE INDEX [IX_FK_Step_TaskSet1]
ON [dbo].[Step]
    ([taskId]);
GO

-- Creating foreign key on [taskId] in table 'TaskUser'
ALTER TABLE [dbo].[TaskUser]
ADD CONSTRAINT [FK_TaskUser_TaskSet]
    FOREIGN KEY ([taskId])
    REFERENCES [dbo].[TaskSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskUser_TaskSet'
CREATE INDEX [IX_FK_TaskUser_TaskSet]
ON [dbo].[TaskUser]
    ([taskId]);
GO

-- Creating foreign key on [RoleId] in table 'TaskSet'
ALTER TABLE [dbo].[TaskSet]
ADD CONSTRAINT [FK_TaskSet_Roles]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskSet_Roles'
CREATE INDEX [IX_FK_TaskSet_Roles]
ON [dbo].[TaskSet]
    ([RoleId]);
GO

-- Creating foreign key on [roleId] in table 'UserRole'
ALTER TABLE [dbo].[UserRole]
ADD CONSTRAINT [FK_UserRole_Roles]
    FOREIGN KEY ([roleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserRole_Roles'
CREATE INDEX [IX_FK_UserRole_Roles]
ON [dbo].[UserRole]
    ([roleId]);
GO

-- Creating foreign key on [RoleId] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_Users_Roles]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Users_Roles'
CREATE INDEX [IX_FK_Users_Roles]
ON [dbo].[Users]
    ([RoleId]);
GO

-- Creating foreign key on [RoleId] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_Users_Roles1]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Users_Roles1'
CREATE INDEX [IX_FK_Users_Roles1]
ON [dbo].[Users]
    ([RoleId]);
GO

-- Creating foreign key on [userId] in table 'TaskUser'
ALTER TABLE [dbo].[TaskUser]
ADD CONSTRAINT [FK_TaskUser_Users]
    FOREIGN KEY ([userId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskUser_Users'
CREATE INDEX [IX_FK_TaskUser_Users]
ON [dbo].[TaskUser]
    ([userId]);
GO

-- Creating foreign key on [userId] in table 'UserRole'
ALTER TABLE [dbo].[UserRole]
ADD CONSTRAINT [FK_UserRole_Users]
    FOREIGN KEY ([userId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserRole_Users'
CREATE INDEX [IX_FK_UserRole_Users]
ON [dbo].[UserRole]
    ([userId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------