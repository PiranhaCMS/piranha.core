/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/tidyui/coreweb
 * 
 */

using CoreWebAngular.Models.Fields;
using Piranha.AttributeBuilder;
using Piranha.Extend.Fields;

namespace CoreWebAngular.Models.Regions
{
    /// <summary>
    /// Simple region for some optional page heading information.
    /// </summary>
    public class PageHeading
    {
        /// <summary>
        /// Gets/sets the optional primary image.
        /// </summary>
        [Field(Title = "Primary Image")]
        public SizedImageField PrimaryImage { get; set; }



        /// <summary>
        /// Gets/sets the optional ingress.
        /// </summary>
        [Field]
        public TextField Ingress { get; set; }
    }
}
