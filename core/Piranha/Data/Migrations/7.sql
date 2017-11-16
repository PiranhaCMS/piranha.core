--
-- Copyright (c) 2017 Håkan Edling
--
-- This software may be modified and distributed under the terms
-- of the MIT license.  See the LICENSE file for details.
-- 
-- http://github.com/piranhacms/piranha
-- 

DROP INDEX IX_Page_Slug;
CREATE UNIQUE INDEX IX_Page_Slug ON [Piranha_Pages] ([SiteId], [Slug]);
