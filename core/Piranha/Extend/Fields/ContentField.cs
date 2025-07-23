/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Extend.Fields;

/// <summary>
/// Field for referencing a content model.
/// </summary>
[FieldType(Name = "Content", Shorthand = "Content", Component = "content-field")]
public class ContentField : IField, IEquatable<ContentField>
{
    /// <summary>
    /// Gets/sets the content id.
    /// </summary>
    /// <returns></returns>
    public Guid? Id { get; set; }

    /// <summary>
    /// Gets/sets the related content object.
    /// </summary>
    public Models.ContentInfo Content { get; set; }

    /// <summary>
    /// Gets if the field has a content object available.
    /// </summary>
    public bool HasValue => Content != null;

    /// <inheritdoc />
    public virtual string GetTitle()
    {
        return Content?.Title;
    }

    /// <summary>
    /// Initializes the field for client use.
    /// </summary>
    /// <param name="api">The current api</param>
    public virtual async Task Init(IApi api)
    {
        if (Id.HasValue)
        {
            Content = await api.Content
                .GetByIdAsync<Models.ContentInfo>(Id.Value)
                .ConfigureAwait(false);

            if (Content == null)
            {
                // The content has been removed, remove the
                // missing id.
                Id = null;
            }
        }
    }

    /// <summary>
    /// Implicit operator for converting a Guid id to a field.
    /// </summary>
    /// <param name="guid">The guid value</param>
    public static implicit operator ContentField(Guid guid)
    {
        return new ContentField { Id = guid };
    }

    /// <summary>
    /// Implicit operator for converting a content object to a field.
    /// </summary>
    /// <param name="content">The content object</param>
    public static implicit operator ContentField(Models.GenericContent content)
    {
        return new ContentField { Id = content.Id };
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Id.HasValue ? Id.GetHashCode() : 0;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (obj is ContentField field)
        {
            return Equals(field);
        }
        return false;
    }

    /// <summary>
    /// Checks if the given field is equal to the field.
    /// </summary>
    /// <param name="obj">The field</param>
    /// <returns>True if the fields are equal</returns>
    public virtual bool Equals(ContentField obj)
    {
        if (obj == null)
        {
            return false;
        }
        return Id == obj.Id;
    }

    /// <summary>
    /// Checks if the fields are equal.
    /// </summary>
    /// <param name="field1">The first field</param>
    /// <param name="field2">The second field</param>
    /// <returns>True if the fields are equal</returns>
    public static bool operator ==(ContentField field1, ContentField field2)
    {
        if ((object) field1 != null && (object) field2 != null)
        {
            return field1.Equals(field2);
        }

        if ((object)field1 == null && (object)field2 == null)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the fields are not equal.
    /// </summary>
    /// <param name="field1">The first field</param>
    /// <param name="field2">The second field</param>
    /// <returns>True if the fields are equal</returns>
    public static bool operator !=(ContentField field1, ContentField field2)
    {
        return !(field1 == field2);
    }
}
