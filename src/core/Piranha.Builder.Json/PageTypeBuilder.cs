/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Piranha.Builder.Json
{
    public class PageTypeBuilder
    {
        #region Members
        private readonly List<ConfigFile> files = new List<ConfigFile>();
        private readonly IApi api;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public PageTypeBuilder(IApi api) {
            this.api = api;
        }

        /// <summary>
        /// Adds a new json file to build page types from
        /// </summary>
        /// <param name="filename">The filename</param>
        /// <param name="optional">If the file is optional</param>
        /// <returns>The builder</returns>
        public PageTypeBuilder AddJsonFile(string filename, bool optional = false) {
            files.Add(new ConfigFile() {
                Filename = filename,
                Optional = optional
            });
            return this;
        }

        /// <summary>
        /// Builds the page types.
        /// </summary>
        public void Build() {
            foreach (var file in files) {
                if (File.Exists(file.Filename)) {
                    using (var json = File.OpenRead(file.Filename)) {
                        using (var reader = new StreamReader(json)) {
                            var import = JsonConvert.DeserializeObject<PageTypeConfig>(reader.ReadToEnd());

                            import.AssertConfigIsValid();

                            // Update page types
                            foreach (var type in import.PageTypes)
                                api.PageTypes.Save(type);
                        }
                    }
                } else if (!file.Optional) {
                    throw new FileNotFoundException($"Specified file {file.Filename} not found!");
                }
            }
        }
    }
}
