using System;
using CoreWebAngular.Models.Fields;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Piranha;
using Piranha.Extend.Fields;

namespace CoreWebAngular.Converters
{
    public class ImageCoverter : JsonConverter
    {
        private IApi Api;
        public ImageCoverter(IApi api)
        {
            Api = api;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var img = (ImageField)value;

            JObject jo = new JObject();

            jo.Add(new JProperty("HasValue", img.HasValue));

            if (img.HasValue)
            {
                var url = ((string)img).Substring(1);                
                jo.Add(new JProperty("Url", url));
            }

            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ImageField).IsAssignableFrom(objectType);
        }
    }
}
