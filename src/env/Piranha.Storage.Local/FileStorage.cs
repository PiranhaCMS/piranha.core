/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.IO;

namespace Piranha.Storage.Local
{
    public class FileStorage : IStorage
    {
        public FileStorage() {
            if (!Directory.Exists("wwwroot/uploads"))
                Directory.CreateDirectory("wwwroot/uploads");
        }

        /// <summary>
        /// Opens a new storage session.
        /// </summary>
        /// <returns>A new open session</returns>
        public IStorageSession Open() {
            return new FileStorageSession();
        }
    }
}
