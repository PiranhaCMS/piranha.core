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
/// Base class for all asset reference fields.
/// </summary>
/// <typeparam name="T">The field type</typeparam>
public class MediaFieldBase<T> : IField, IEquatable<T> where T : MediaFieldBase<T>
{
    /// <inheritdoc />
    public virtual string GetTitle()
    {
        return Media != null ? (!string.IsNullOrWhiteSpace(Media.Title)? string.Format("{0} ({1})", Media.Title, Media.Filename) : Media.Filename) : null;
    }

    /// <summary>
    /// Gets/sets the media id.
    /// </summary>
    /// <returns></returns>
    public Guid? Id { get; set; }

    /// <summary>
    /// Gets/sets the related media object.
    /// </summary>
    /// [JsonIgnore]
    public Models.Media Media { get; set; }

    /// <summary>
    /// Gets if the field has a media object available.
    /// </summary>
    public bool HasValue => Media != null;

    /// <summary>
    /// Initializes the field for client use.
    /// </summary>
    /// <param name="api">The current api</param>
    public virtual async Task Init(IApi api)
    {
        if (Id.HasValue)
        {
            Media = await api.Media
                .GetByIdAsync(Id.Value)
                .ConfigureAwait(false);

            if (Media == null)
            {
                // The image has been removed, remove the
                // missing id.
                Id = null;
            }
        }
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Id.HasValue ? Id.GetHashCode() : 0;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (obj is T field)
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
    public virtual bool Equals(T obj)
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
    public static bool operator ==(MediaFieldBase<T> field1, MediaFieldBase<T> field2)
    {
        if ((object)field1 != null && (object)field2 != null)
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
    public static bool operator !=(MediaFieldBase<T> field1, MediaFieldBase<T> field2)
    {
        return !(field1 == field2);
    }
}
