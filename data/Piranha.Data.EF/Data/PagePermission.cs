/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Text.Json.Serialization;


namespace Piranha.Data;

[Serializable]
public sealed class PagePermission
{
    public string PageId { get; set; }
    public string Permission { get; set; }

    [JsonIgnore]
    public Page Page { get; set; }
}
