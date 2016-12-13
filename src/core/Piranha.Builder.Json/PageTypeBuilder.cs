/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.Extensions.Logging;
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
        private readonly ILogger logger;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="logFactory">The optional log factory</param>
        public PageTypeBuilder(IApi api, ILoggerFactory logFactory = null) {
            this.api = api;
            this.logger = logFactory?.CreateLogger("Piranha.Builder.Json.PageTypeBuilder");
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
                            foreach (var type in import.PageTypes) {
                                logger?.LogInformation($"Importing PageType '{type.Id}'.");
                                api.PageTypes.Save(type);
                            }
                        }
                    }
                } else if (!file.Optional) {
                    logger?.LogError($"Specified file '{file.Filename}' not found'.");
                    throw new FileNotFoundException($"Specified file {file.Filename} not found!");
                }
            }
            // Tell the app to reload the page types
            App.ReloadPageTypes(api);
        }
    }
}
