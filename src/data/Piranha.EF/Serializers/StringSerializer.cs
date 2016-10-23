using System;

namespace Piranha.EF.Serializers
{
    /// <summary>
    /// Serializer for simple string based fields.
    /// </summary>
    /// <typeparam name="T">The field type</typeparam>
    public class StringSerializer<T> : ISerializer where T : Extend.Fields.SimpleField<string>
    {
        /// <summary>
        /// Serializes the given object.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>The serialized value</returns>
        public string Serialize(object obj) {
            if (obj is T)
                return ((T)obj).Value;
            throw new ArgumentException("The given object is not assignable from SimpleField<string>");
        }

        /// <summary>
        /// Deserializes the given string.
        /// </summary>
        /// <param name="str">The serialized value</param>
        /// <returns>The object</returns>
        public object Deserialize(string str) {
            var obj = Activator.CreateInstance<T>();
            obj.Value = str;

            return obj;
        }
    }
}
