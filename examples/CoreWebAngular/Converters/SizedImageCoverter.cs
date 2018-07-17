using System;
using CoreWebAngular.Models.Fields;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Piranha;
using Piranha.Extend.Fields;

namespace CoreWebAngular.Converters
{
    public class SizedImageCoverter : JsonConverter
    {
        private IApi Api;
        public SizedImageCoverter(IApi api)
        {
            Api = api;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var img = (SizedImageField)value;

            JObject jo = new JObject();

            jo.Add(new JProperty("HasValue", img.HasValue));

            if (img.HasValue)
            {
                var url = "";
                if(img.Width != null && img.Width.Value > 0)
                {
                    url = img.Resize(Api, img.Width.Value, img.Height).Substring(1);
                }
                else
                {
                    url = ((string)img).Substring(1);
                }
                
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
            return typeof(SizedImageField).IsAssignableFrom(objectType);
        }
    }
}
