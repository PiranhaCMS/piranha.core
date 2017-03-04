--
-- Copyright (c) 2017 Håkan Edling
--
-- This software may be modified and distributed under the terms
-- of the MIT license.  See the LICENSE file for details.
-- 
-- http://github.com/piranhacms/piranha
-- 

CREATE TABLE [Piranha_Migrations] (
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(64) NOT NULL,
	[Created] DATETIME NOT NULL,
	CONSTRAINT PK_Migration_Id PRIMARY KEY ([Id])
);

CREATE TABLE [Piranha_Params] (
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[Key] NVARCHAR(64) NOT NULL,
	[Value] NTEXT NULL,
	[Description] NVARCHAR(256) NULL,
	[Created] DATETIME NOT NULL,
	[LastModified] DATETIME NOT NULL,
	CONSTRAINT PK_Param_Id PRIMARY KEY ([Id])
);
CREATE UNIQUE INDEX IX_Param_Key ON [Piranha_Params] ([Key]);

CREATE TABLE [Piranha_Sites] (
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[InternalId] NVARCHAR(64) NOT NULL,
	[Title] NVARCHAR(128) NOT NULL,
	[Description] NVARCHAR(256) NULL,
	[HostNames] NVARCHAR(256) NULL,
	[IsDefault] BIT NOT NULL DEFAULT(0),
	[Created] DATETIME NOT NULL,
	[LastModified] DATETIME NOT NULL,
	CONSTRAINT PK_Site_Id PRIMARY KEY ([Id])
);
CREATE UNIQUE INDEX IX_Site_InternalId ON [Piranha_Sites] ([InternalId]);

CREATE TABLE [Piranha_PageTypes] (
    [Id] NVARCHAR(64) NOT NULL,
    [Body] NTEXT NOT NULL,
    [Created] DATETIME NOT NULL,
    [LastModified] DATETIME NOT NULL,
	CONSTRAINT PK_PageType_Id PRIMARY KEY ([Id])
);

CREATE TABLE [Piranha_Pages] (
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[PageTypeId] NVARCHAR(64) NOT NULL,
	[SiteId] UNIQUEIDENTIFIER NOT NULL,
	[ParentId] UNIQUEIDENTIFIER NULL,
	[SortOrder] INT NOT NULL DEFAULT(0),
	[Title] NVARCHAR(128) NOT NULL,
	[NavigationTitle] NVARCHAR(128) NULL,
	[Slug] NVARCHAR(128) NOT NULL,
	[IsHidden] BIT NOT NULL DEFAULT (0),
	[MetaKeywords] NVARCHAR(128) NULL,
	[MetaDescription] NVARCHAR(256) NULL,
	[Route] NVARCHAR(256) NULL,
	[Published] DATETIME NULL,
	[Created] DATETIME NOT NULL,
	[LastModified] DATETIME NOT NULL,
	CONSTRAINT PK_Page_Id PRIMARY KEY ([Id]),
	CONSTRAINT FK_Page_PageTypeId FOREIGN KEY ([PageTypeId]) REFERENCES [Piranha_PageTypes] ([Id]),
	CONSTRAINT FK_Page_SiteId FOREIGN KEY ([SiteId]) REFERENCES [Piranha_Sites] ([Id]) ON DELETE CASCADE
);
CREATE UNIQUE INDEX IX_Page_Slug ON [Piranha_Pages] ([Slug]);

CREATE TABLE [Piranha_PageFields] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [PageId] UNIQUEIDENTIFIER NOT NULL,
    [RegionId] NVARCHAR(64) NOT NULL,
    [FieldId] NVARCHAR(64) NOT NULL,
    [SortOrder] INT NOT NULL DEFAULT(0),
    [CLRType] NVARCHAR(256) NOT NULL,
    [Value] NTEXT NULL,
	CONSTRAINT PK_PageField_Id PRIMARY KEY ([Id]),
	CONSTRAINT FK_PageField_PageId FOREIGN KEY ([PageId]) REFERENCES [Piranha_Pages] ([Id]) ON DELETE CASCADE
);
CREATE UNIQUE INDEX IX_PageField_FieldId ON [Piranha_PageFields] ([PageId], [RegionId], [FieldId], [SortOrder]);
