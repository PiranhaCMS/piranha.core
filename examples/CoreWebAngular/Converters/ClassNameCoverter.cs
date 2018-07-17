using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoreWebAngular.Converters
{
    public class ClassNameCoverter<T> : JsonConverter<T>
    {
        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            JProperty NameProperty = new JProperty("Type", value.GetType().Name);

            JObject jo = new JObject();
            if (NameProperty != null)
            {
                jo.Add(NameProperty);
            }
            foreach (PropertyInfo prop in value.GetType().GetProperties())
            {
                if (prop.CanRead && !prop.CustomAttributes.Any(a => a.AttributeType == typeof(JsonIgnoreAttribute)))
                {
                    object propValue = prop.GetValue(value);
                    if (propValue != null)
                    {
                        jo.Add(prop.Name, JToken.FromObject(propValue, serializer));
                    }
                }
            }
            jo.WriteTo(writer);
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
