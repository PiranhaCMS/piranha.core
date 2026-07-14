/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 */

namespace Piranha.Manager.Models;

/// <summary>
/// A portable translation document for one page and language pair.
/// </summary>
public class PageTranslationExchangeModel
{
    /// <summary>
    /// Gets or sets the exchange format version.
    /// </summary>
    public string FormatVersion { get; set; } = "1.0";

    /// <summary>
    /// Gets or sets the page identity.
    /// </summary>
    public Guid PageId { get; set; }

    /// <summary>
    /// Gets or sets the site identity.
    /// </summary>
    public Guid SiteId { get; set; }

    /// <summary>
    /// Gets or sets the source language.
    /// </summary>
    public PageTranslationExchangeLanguage SourceLanguage { get; set; }

    /// <summary>
    /// Gets or sets the language that receives the imported text.
    /// </summary>
    public PageTranslationExchangeLanguage TargetLanguage { get; set; }

    /// <summary>
    /// Gets or sets the UTC export timestamp.
    /// </summary>
    public DateTime ExportedAt { get; set; }

    /// <summary>
    /// Gets or sets the canonical page structure fingerprint.
    /// </summary>
    public string StructureHash { get; set; }

    /// <summary>
    /// Gets or sets the translatable units.
    /// </summary>
    public IList<PageTranslationExchangeUnit> Units { get; set; } = new List<PageTranslationExchangeUnit>();
}

/// <summary>
/// Identifies a language in a translation exchange document.
/// </summary>
public class PageTranslationExchangeLanguage
{
    /// <summary>
    /// Gets or sets the language identity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the display title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the language culture.
    /// </summary>
    public string Culture { get; set; }
}

/// <summary>
/// A single page metadata, region, or block text value.
/// </summary>
public class PageTranslationExchangeUnit
{
    /// <summary>
    /// Gets or sets the stable unit key.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets a human-readable translation context.
    /// </summary>
    public string Context { get; set; }

    /// <summary>
    /// Gets or sets the source field type.
    /// </summary>
    public string FieldType { get; set; }

    /// <summary>
    /// Gets or sets the source-language text.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the text to import into the target language.
    /// A null value leaves the target unchanged; an empty value clears it.
    /// </summary>
    public string Target { get; set; }
}

/// <summary>
/// Describes the result of importing one page translation document.
/// </summary>
public class PageTranslationExchangeImportResult
{
    /// <summary>
    /// Gets or sets the operation status.
    /// </summary>
    public StatusMessage Status { get; set; }

    /// <summary>
    /// Gets or sets the number of overwritten text values.
    /// </summary>
    public int Replaced { get; set; }

    /// <summary>
    /// Gets or sets the number of values explicitly cleared.
    /// </summary>
    public int Cleared { get; set; }

    /// <summary>
    /// Gets or sets the target language id.
    /// </summary>
    public Guid TargetLanguageId { get; set; }
}
