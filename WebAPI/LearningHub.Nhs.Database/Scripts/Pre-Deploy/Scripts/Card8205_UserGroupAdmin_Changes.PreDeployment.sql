/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
    Killian davies 10-03-21
	Card 8205 - User Group Admin
    - Attribute required for Restricted Catalogues user groups
    - Drop existing Attribute table as not in use
--------------------------------------------------------------------------------------
*/

--IF (NOT EXISTS (SELECT 'X' 
--					FROM INFORMATION_SCHEMA.TABLES 
--					WHERE TABLE_SCHEMA = 'hub' 
--					AND  TABLE_NAME = 'AttributeType'))
--BEGIN
--	CREATE TABLE [hub].[AttributeType](
--	[Id] [int] NOT NULL,
--	[Name] [nvarchar](50) NOT NULL,
--	[Deleted] [bit] NOT NULL,
--	[CreateUserId] [int] NOT NULL,
--	[CreateDate] [datetimeoffset](7) NOT NULL,
--	[AmendUserId] [int] NOT NULL,
--	[AmendDate] [datetimeoffset](7) NOT NULL,
--		CONSTRAINT [PK_attributeTypeTBL] PRIMARY KEY CLUSTERED 
--	(
--		[Id] ASC
--	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
--	) ON [PRIMARY]

--	ALTER TABLE [hub].[AttributeType] ADD  DEFAULT ((0)) FOR [Deleted]

--	ALTER TABLE [hub].[AttributeType] ADD  DEFAULT (sysdatetimeoffset()) FOR [AmendDate]
--END
--GO

--IF NOT EXISTS(SELECT 1 FROM sys.columns 
--		WHERE Name = N'AttributeTypeId'
--		AND Object_ID = Object_ID(N'hub.Attribute'))
--BEGIN
--	DROP TABLE [hub].[Attribute]

--	CREATE TABLE [hub].[Attribute](
--		[Id] [int] IDENTITY(1,1) NOT NULL,
--		[AttributeTypeId] [int] NOT NULL,
--		[Name] [nvarchar](50) NOT NULL,
--		[Description] [nvarchar](400) NULL,
--		[Deleted] [bit] NOT NULL,
--		[CreateUserId] [int] NOT NULL,
--		[CreateDate] [datetimeoffset](7) NOT NULL,
--		[AmendUserId] [int] NOT NULL,
--		[AmendDate] [datetimeoffset](7) NOT NULL,
--	 CONSTRAINT [PK_Attribute] PRIMARY KEY CLUSTERED 
--	(
--		[Id] ASC
--	)
--	) 

--	ALTER TABLE [hub].[Attribute] ADD  DEFAULT ((0)) FOR [Deleted]

--	ALTER TABLE [hub].[Attribute] ADD  DEFAULT (sysdatetimeoffset()) FOR [AmendDate]

--	ALTER TABLE [hub].[Attribute]  WITH CHECK ADD  CONSTRAINT [FK_Attribute_AttributeType] FOREIGN KEY([AttributeTypeId])
--	REFERENCES [hub].[AttributeType] ([Id])

--	ALTER TABLE [hub].[Attribute] CHECK CONSTRAINT [FK_Attribute_AttributeType]

--END
--GO

-- Attribute table updates - Start
GO
ALTER TABLE [hub].[Attribute] DROP CONSTRAINT [FK_Attribute_AmendUser];


GO
ALTER TABLE [hub].[Attribute] DROP CONSTRAINT [FK_Attribute_CreateUser];


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [hub].[tmp_ms_xx_Attribute] (
    [Id]              INT                IDENTITY (1, 1) NOT NULL,
    [AttributeTypeId] INT                DEFAULT (1) NOT NULL,
    [Name]            NVARCHAR (50)      NOT NULL,
    [Description]     NVARCHAR (400)     NULL,
    [Deleted]         BIT                DEFAULT ((0)) NOT NULL,
    [CreateUserId]    INT                NOT NULL,
    [CreateDate]      DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]     INT                NOT NULL,
    [AmendDate]       DATETIMEOFFSET (7) DEFAULT (sysdatetimeoffset()) NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_Attribute1] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [hub].[Attribute])
    BEGIN
        SET IDENTITY_INSERT [hub].[tmp_ms_xx_Attribute] ON;
        INSERT INTO [hub].[tmp_ms_xx_Attribute] ([Id], [Name], [Description], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate])
        SELECT   [Id],
                 [Name],
                 [Description],
                 [Deleted],
                 [CreateUserId],
                 [CreateDate],
                 [AmendUserId],
                 [AmendDate]
        FROM     [hub].[Attribute]
        ORDER BY [Id] ASC;
        SET IDENTITY_INSERT [hub].[tmp_ms_xx_Attribute] OFF;
    END

DROP TABLE [hub].[Attribute];

EXECUTE sp_rename N'[hub].[tmp_ms_xx_Attribute]', N'Attribute';

EXECUTE sp_rename N'[hub].[tmp_ms_xx_constraint_PK_Attribute1]', N'PK_Attribute', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO

IF NOT EXISTS (SELECT 1 FROM [hub].[AttributeType])
BEGIN
    INSERT INTO [hub].[AttributeType] (Id, Name, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (1, 'Integer', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
    INSERT INTO [hub].[AttributeType] (Id, Name, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (2, 'String', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
    INSERT INTO [hub].[AttributeType] (Id, Name, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (3, 'Boolean', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
    INSERT INTO [hub].[AttributeType] (Id, Name, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (4, 'DateTime', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END
GO

ALTER TABLE [hub].[Attribute] WITH NOCHECK
    ADD CONSTRAINT [FK_Attribute_AttributeType] FOREIGN KEY ([AttributeTypeId]) REFERENCES [hub].[AttributeType] ([Id]);

GO
ALTER TABLE [hub].[Attribute] WITH CHECK CHECK CONSTRAINT [FK_Attribute_AttributeType];


GO


IF EXISTS(SELECT 1 FROM [hub].[Attribute] WHERE Name = 'Resource MigrationInputRecordId' AND Id = 1)
BEGIN
    SET IDENTITY_INSERT [hub].[Attribute] ON;

    ALTER TABLE [resources].[ResourceAttribute] DROP CONSTRAINT [FK_ResourceAttribute_Attribute];

    INSERT INTO [hub].[Attribute] ([Id],[AttributeTypeId],[Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		VALUES (3, 1, 'Resource MigrationInputRecordId','If a resource was created during a migration, this attribute defines the Id of the MigrationInputRecord that it was originally created from.',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())

    UPDATE [resources].[ResourceAttribute] SET AttributeId = 3 where AttributeId = 1;

    -- Cannot update idenity column, just delete the row and let the post-deploy script add back the migration attribute with the correct id
    --UPDATE [hub].[Attribute] SET Id = 3 WHERE Name = 'Resource MigrationInputRecordId';
    
    DELETE FROM [hub].[Attribute] where Id = 1;

    INSERT INTO [hub].[Attribute] ([Id],[AttributeTypeId],[Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		VALUES (1, 1,'Local Admin User Group','Local Admin User Group',0,4, SYSDATETIMEOFFSET(),4, SYSDATETIMEOFFSET());

	INSERT INTO [hub].[Attribute] ([Id],[AttributeTypeId],[Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		VALUES (2, 2,'Restricted Access User Group','Local Admin User Group',0,4, SYSDATETIMEOFFSET(),4, SYSDATETIMEOFFSET());
    SET IDENTITY_INSERT [hub].[Attribute] OFF;
END
GO
-- Attribute table updates - End




IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Code'
          AND Object_ID = Object_ID(N'hub.UserGroup'))
BEGIN

    -- Remove any default constraints on Code
	WHILE 0=0
	BEGIN
	DECLARE @constraintName varchar(128)
	SET @constraintName = (	select top 1 con.name
							from sys.default_constraints con
							left outer join sys.objects t
								on con.parent_object_id = t.object_id
							left outer join sys.all_columns col
								on con.parent_column_id = col.column_id
								and con.parent_object_id = col.object_id
							where t.name = 'UserGroup' and col.name = 'Code' and schema_name(t.schema_id) = 'hub')
	IF @constraintName is null break
	EXEC ('alter table hub.UserGroup drop constraint "'+@constraintName+'"')
	END

    ALTER TABLE hub.UserGroup DROP COLUMN [Code]
END
GO

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'UserGroupTypeId'
          AND Object_ID = Object_ID(N'hub.UserGroup'))
BEGIN

    -- Remove any default constraints on UserGroupTypeId
	WHILE 0=0
	BEGIN
	DECLARE @constraintName varchar(128)
	SET @constraintName = (	select top 1 con.name
							from sys.default_constraints con
							left outer join sys.objects t
								on con.parent_object_id = t.object_id
							left outer join sys.all_columns col
								on con.parent_column_id = col.column_id
								and con.parent_object_id = col.object_id
							where t.name = 'UserGroup' and col.name = 'UserGroupTypeId' and schema_name(t.schema_id) = 'hub')
	IF @constraintName is null break
	EXEC ('alter table hub.UserGroup drop constraint "'+@constraintName+'"')
	END

	ALTER TABLE [hub].[UserGroup] DROP CONSTRAINT [FK_userGroup_userGroupType]

    ALTER TABLE hub.UserGroup DROP COLUMN UserGroupTypeId
END
GO

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'hub' 
                 AND  TABLE_NAME = 'TenantUserType'))
BEGIN
    DROP TABLE hub.TenantUserType
END
GO

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'hub' 
                 AND  TABLE_NAME = 'UserUserType'))
BEGIN

	ALTER TABLE hub.UserUserType SET (SYSTEM_VERSIONING = OFF)
 
    DROP TABLE hub.UserUserType

    DROP TABLE hub.UserUserTypeHistory
 
END
GO

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'hub' 
                 AND  TABLE_NAME = 'UserType'))
BEGIN
    DROP TABLE hub.UserType
END
GO

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'hub' 
                 AND  TABLE_NAME = 'UserGroupType'))
BEGIN
    DROP TABLE hub.UserGroupType
END
GO