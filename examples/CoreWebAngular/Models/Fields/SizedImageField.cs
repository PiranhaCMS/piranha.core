using Newtonsoft.Json;
using Piranha;
using Piranha.Extend;
using Piranha.Extend.Fields;
using System;

namespace CoreWebAngular.Models.Fields
{
    [FieldType(Name = "SizedImage", Shorthand = "SizedImage")]
    public class SizedImageField : MediaFieldBase<SizedImageField>
    {

        public int? Width { get; set; }

        public int? Height { get; set; }

        /// <summary>
        /// Implicit operator for converting a Guid id to a field.
        /// </summary>
        /// <param name="guid">The string value</param>
        public static implicit operator SizedImageField(Guid guid)
        {
            return new SizedImageField { Id = guid };
        }

        /// <summary>
        /// Implicit operator for converting a Guid id to a field.
        /// </summary>
        /// <param name="guid">The image Guid</param>
        /// <param name="width">The image Width</param>
        /// <param name="height">The image Height</param>
        public static SizedImageField WithSize(Guid guid, int? width = null, int? height = null)
        {
            return new SizedImageField { Id = guid, Width = width, Height = height };
        }

        /// <summary>
        /// Implicit operator for converting a Guid id to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator SizedImageField(ImageField image)
        {
            return new SizedImageField { Id = image.Id };
        }

        /// <summary>
        /// Implicit operator for converting a media object to a field.
        /// </summary>
        /// <param name="media">The media object</param>
        public static implicit operator SizedImageField(Piranha.Data.Media media)
        {
            return new SizedImageField { Id = media.Id };
        }

        /// <summary>
        /// Impicit operator for converting the field to an url string.
        /// </summary>
        /// <param name="image">The image field</param>
        public static implicit operator string(SizedImageField image)
        {
            if (image.Media != null)
                return image.Media.PublicUrl;
            return "";
        }

        /// <summary>
        /// Gets the url for a resized version of the image.
        /// </summary>
        /// <param name="api">The api</param>
        /// <param name="width">The requested width</param>
        /// <param name="height">The optional height</param>
        /// <returns>The image url</returns>
        public string Resize(IApi api, int width, int? height = null)
        {
            if (Id.HasValue)
                return api.Media.EnsureVersion(Id.Value, width, height);
            return null;
        }

    }
}
