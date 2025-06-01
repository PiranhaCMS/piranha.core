/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.ComponentModel.DataAnnotations;
using Piranha.Extend;

namespace Piranha.Models;

/// <summary>
/// Base class for page models.
/// </summary>
[Serializable]
public abstract class PageBase : RoutedContentBase
{
    /// <summary>
    /// Gets/sets the site id.
    /// </summary>
    public Guid SiteId { get; set; }

    /// <summary>
    /// Gets/sets the optional parent id.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Gets/sets the sort order of the page in its hierarchical position.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets/sets the navigation title.
    /// </summary>
    [StringLength(128)]
    public string NavigationTitle { get; set; }

    /// <summary>
    /// Gets/sets if the page is hidden in the navigation.
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Gets/sets the id of the page this page is a copy of
    /// </summary>
    public Guid? OriginalPageId { get; set; }

    /// <summary>
    /// Gets if this is the startpage of the site.
    /// </summary>
    public bool IsStartPage => !ParentId.HasValue && SortOrder == 0;

    /// <summary>
    /// Workflow properties.
    /// </summary>
    public Guid? WorkflowId { get; set; }
    public Workflow Workflow { get; set; }
    
    /// <summary>
    /// Gets/sets the workflow status from database
    /// </summary>
    public int? WorkflowStatusValue { get; set; }

    public enum PageWorkflowStatus
    {
        Draft = 0,
        PendingReview = 1,
        PendingLegal = 2,
        Approved = 3,
        Rejected = 4,
    }

    public PageWorkflowStatus WorkflowStatus
    {
        get
        {
            // Primeiro, tenta usar o valor da base de dados
            if (WorkflowStatusValue.HasValue)
            {
                return (PageWorkflowStatus)WorkflowStatusValue.Value;
            }

            // Fallback para a lógica anterior se não houver valor na BD
            if (Workflow == null)
                return PageWorkflowStatus.Draft;

            if (Workflow.IsApproved)
                return PageWorkflowStatus.Approved;

            if (
                Workflow.CurrentStep == 0
                && !string.IsNullOrEmpty(Workflow.GetCurrentStep()?.Reason)
            )
                return PageWorkflowStatus.Rejected;

            if (Workflow.CurrentStep == 0)
                return PageWorkflowStatus.PendingReview;

            if (Workflow.CurrentStep == 1)
                return PageWorkflowStatus.PendingLegal;

            return PageWorkflowStatus.Draft;
        }
    }

    public string WorkflowStatusDisplay
    {
        get
        {
            return WorkflowStatus switch
            {
                PageWorkflowStatus.Draft => "Draft",
                PageWorkflowStatus.PendingReview => "Pending Review",
                PageWorkflowStatus.PendingLegal => "Pending Legal",
                PageWorkflowStatus.Approved => "Approved",
                PageWorkflowStatus.Rejected => "Rejected",
                _ => "Unknown",
            };
        }
    }
}