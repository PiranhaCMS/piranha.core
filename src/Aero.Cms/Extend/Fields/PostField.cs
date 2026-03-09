

namespace Aero.Cms.Extend.Fields;

/// <summary>
/// Field for referencing a post.
/// </summary>
[FieldType(Name = "Post", Shorthand = "Post", Component = "post-field")]
public class PostField : IField, IEquatable<PostField>
{
    /// <summary>
    /// Gets/sets the media id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets/sets the related post object.
    /// </summary>
    public Models.PostInfo Post { get; set; }

    /// <summary>
    /// Gets if the field has a post object available.
    /// </summary>
    public bool HasValue => Post != null;

    /// <inheritdoc />
    public virtual string GetTitle()
    {
        return Post?.Title;
    }

    /// <summary>
    /// Initializes the field for client use.
    /// </summary>
    /// <param name="api">The current api</param>
    public virtual async Task Init(IApi api)
    {
        if (!string.IsNullOrEmpty(Id))
        {
            Post = await api.Posts
                .GetByIdAsync<Models.PostInfo>(Id)
                .ConfigureAwait(false);

            if (Post == null)
            {
                // The post has been removed, remove the
                // missing id.
                Id = null;
            }
        }
    }

    /// <summary>
    /// Gets the referenced post.
    /// </summary>
    /// <param name="api">The current api</param>
    /// <returns>The referenced post</returns>
    public virtual Task<T> GetPostAsync<T>(IApi api) where T : Models.Post<T>
    {
        if (!string.IsNullOrEmpty(Id))
        {
            return api.Posts.GetByIdAsync<T>(Id);
        }
        return null;
    }

    /// <summary>
    /// Implicit operator for converting a string id to a field.
    /// </summary>
    /// <param name="id">The id value</param>
    public static implicit operator PostField(string id)
    {
        return new PostField { Id = id };
    }

    /// <summary>
    /// Implicit operator for converting a Guid id to a field.
    /// </summary>
    /// <param name="guid">The guid value</param>
    public static implicit operator PostField(Guid guid)
    {
        return new PostField
        {
            Id = guid.ToString()
        };
    }

    /// <summary>
    /// Implicit operator for converting a post object to a field.
    /// </summary>
    /// <param name="post">The post object</param>
    public static implicit operator PostField(Models.PostBase post)
    {
        return new PostField
        {
            Id = post.Id
        };
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return !string.IsNullOrEmpty(Id) ? Id.GetHashCode() : 0;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (obj is PostField field)
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
    public virtual bool Equals(PostField obj)
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
    public static bool operator ==(PostField field1, PostField field2)
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
    public static bool operator !=(PostField field1, PostField field2)
    {
        return !(field1 == field2);
    }
}
