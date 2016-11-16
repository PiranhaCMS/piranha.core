/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Piranha.EF
{
    public class DbFactory : IDbContextFactory<Db>
    {
        public Db Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<Db>();
            builder.UseSqlite("Filename=./piranha-dev.db");
            return new Db(builder.Options);
        }
    }
}
