﻿/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/tidyui/coreweb
 * 
 */

using Piranha.AttributeBuilder;

namespace CoreWeb.Models
{
    /// <summary>
    /// Basic page with main content in markdown.
    /// </summary>
    [PageType(Title = "Standard page")]
    public class StandardPage : BasePage<StandardPage>
    {
    }
}
