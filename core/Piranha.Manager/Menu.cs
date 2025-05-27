/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Manager;

/// <summary>
/// Static class for defining the manager menu.
/// </summary>
public static class Menu
{
    /// <summary>
    /// The basic manager menu.
    /// </summary>
    public static MenuItemList Items { get; } = new MenuItemList
    {
        new MenuItem
        {
            InternalId = "Content",
            Name = "Content",
            Css = "fas fa-pencil-alt",
            Items = new MenuItemList
            {
                new MenuItem
                {
                    InternalId = "Pages",
                    Name = "Pages",
                    Route = "~/manager/pages",
                    Policy = Permission.Pages,
                    Css = "fas fa-copy"
                },
                new MenuItem
                {
                    InternalId = "Media",
                    Name = "Media",
                    Route = "~/manager/media",
                    Policy = Permission.Media,
                    Css = "fas fa-images"
                },
                new MenuItem
                {
                    InternalId = "Comments",
                    Name = "Comments",
                    Route = "~/manager/comments",
                    Policy = Permission.Comments,
                    Css = "fas fa-comments"
                }
            }
        },
        new MenuItem
            {
                InternalId = "Workflows",
                Name       = "Workflows",
                Css        = "fas fa-tasks",
                Items      = new MenuItemList
                    {
                       new MenuItem
                        {
                            InternalId = "WorkflowsMy",
                            Name       = "My Content",
                            Route      = "~/manager/workflows/my",
                            Policy     = Permission.Content,
                            Css        = "fas fa-file-alt"
                        },
                        new MenuItem
                        {
                            InternalId = "WorkflowsOthers",
                            Name       = "Published Content",
                            Route      = "~/manager/workflows/others",
                            Policy     = Permission.Content,
                            Css        = "fas fa-globe"
                        },
                        new MenuItem
                        {
                            InternalId = "WorkflowsReview",
                            Name       = "Review Content",
                            Route      = "~/manager/workflows/review",
                            Policy     = Permission.ContentReview,
                            Css        = "fas fa-clipboard-check"
                        }
                    }
            },
        new MenuItem
        {
            InternalId = "Settings",
            Name = "Settings",
            Css = "fas fa-wrench",
            Items = new MenuItemList
            {
                new MenuItem
                {
                    InternalId = "Aliases", Name = "Aliases", Route = "~/manager/aliases",
                    Policy = Permission.Aliases, Css = "fas fa-random"
                }
            }
        },
        new MenuItem
        {
            InternalId = "System",
            Name = "System",
            Css = "fas fa-cog",
            Items = new MenuItemList
            {
                new MenuItem
                {
                    InternalId = "Config",
                    Name = "Config",
                    Route = "~/manager/config",
                    Policy = Permission.Config,
                    Css = "fas fa-cogs"
                },
                new MenuItem
                {
                    InternalId = "Modules",
                    Name = "Modules",
                    Route = "~/manager/modules",
                    Policy = Permission.Modules,
                    Css = "fas fa-code-branch"
                }
            }
        }
    };
}
