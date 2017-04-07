--
-- Copyright (c) 2017 Håkan Edling
--
-- This software may be modified and distributed under the terms
-- of the MIT license.  See the LICENSE file for details.
-- 
-- http://github.com/piranhacms/piranha
-- 

CREATE TABLE [Piranha_MediaFolders] (
	[Id] NVARCHAR(36) NOT NULL,
	[ParentId] NVARCHAR(36)  NULL,
    [Name] NVARCHAR(128) NOT NULL,
	[Created] DATETIME NOT NULL,
	CONSTRAINT PK_MediaFolder_Id PRIMARY KEY ([Id])        
);

CREATE TABLE [Piranha_Media] (
	[Id] NVARCHAR(36) NOT NULL,
	[FolderId] NVARCHAR(36) NULL,
	[Filename] NVARCHAR(128) NOT NULL,
	[ContentType] NVARCHAR(255) NOT NULL,
	[Size] BIGINT NOT NULL,
	[PublicUrl] NVARCHAR(255) NOT NULL,
	[Created] DATETIME NOT NULL,
    [LastModified] DATETIME NOT NULL,
	CONSTRAINT PK_Media_Id PRIMARY KEY ([Id]),
	CONSTRAINT FK_Media_FolderId FOREIGN KEY ([FolderId]) REFERENCES [Piranha_MediaFolders] ([Id])
);