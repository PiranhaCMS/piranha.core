--
-- Copyright (c) 2017 Håkan Edling
--
-- This software may be modified and distributed under the terms
-- of the MIT license.  See the LICENSE file for details.
-- 
-- http://github.com/piranhacms/piranha
-- 

ALTER TABLE [Piranha_Media] ADD [Type] INT NOT NULL DEFAULT(0);

UPDATE [Piranha_Media] SET [Type] = 1 WHERE [ContentType] LIKE 'document/%' OR [ContentType] LIKE 'application/%';
UPDATE [Piranha_Media] SET [Type] = 2 WHERE [ContentType] LIKE 'image/%';
UPDATE [Piranha_Media] SET [Type] = 3 WHERE [ContentType] LIKE 'video/%';