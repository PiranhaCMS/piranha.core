/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Piranha.Manager.Editor
{
    /// <summary>
    /// Static class for configuring the Tiny MCE editor.
    /// </summary>
    public class EditorConfig
    {
        /// <summary>
        /// Gets/sets the current editor config.
        /// </summary>
        public static EditorConfig Current { get; set; } = new EditorConfig
        {
            Plugins = "autoresize autolink code hr paste lists piranhalink piranhaimage",
            Toolbar = "bold italic | bullist numlist hr | alignleft aligncenter alignright | formatselect | piranhalink piranhaimage",
            BlockFormats = "Paragraph=p;Header 1=h1;Header 2=h2;Header 3=h3;Header 4=h4;Code=pre;Quote=blockquote"
        };

        /// <summary>
        /// Gets/sets the configured editor plugins.
        /// </summary>
        public string Plugins { get; set; }

        /// <summary>
        /// Gets/sets the configured editor toolbar.
        /// </summary>
        public string Toolbar { get; set; }

        /// <summary>
        /// Gets/sets the configured block formats.
        /// </summary>
        public string BlockFormats { get; set; }

        /// <summary>
        /// Gets/sets the configured editor styles.
        /// </summary>
        public IList<EditorStyle> StyleFormats { get; set; } = new List<EditorStyle>();

        /// <summary>
        /// Configures the editor from the given json file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        public static void FromFile(string path)
        {
            if (File.Exists(path)) {
                using (var json = File.OpenRead(path)) {
                    using (var reader = new StreamReader(json)) {
                        Current = JsonConvert.DeserializeObject<EditorConfig>(reader.ReadToEnd());
                    }
                }
            } else {
                throw new FileNotFoundException($"Editor config { path } not found!");
            }
        }
    }
}