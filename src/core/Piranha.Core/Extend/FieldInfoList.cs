using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Piranha.Extend
{
    public class FieldInfoList : IEnumerable<FieldInfo>
    {
		internal readonly IList<FieldInfo> items = new List<FieldInfo>();

		public void Register<T>() where T : IField {
			var type = typeof(T);
			var field = new FieldInfo() {
				CLRType = type.FullName,
				Type = type
			};

			var attr = type.GetTypeInfo().GetCustomAttribute<FieldAttribute>();
			if (attr != null) {
				field.Name = attr.Name;
				field.Shorthand = attr.Shorthand;
			}
			items.Add(field);
		}

		public void UnRegister<T>() where T : IField {
			var item = items.SingleOrDefault(i => i.Type == typeof(T));
			if (item != null)
				items.Remove(item);
		}

		public FieldInfo GetByType(Type type) {
			return items.SingleOrDefault(i => i.Type == type);
		}

		public FieldInfo GetByType(string typeName) {
			return items.SingleOrDefault(i => i.CLRType == typeName);
		}

		public FieldInfo GetByShorthand(string shorthand) {
			return items.SingleOrDefault(i => i.Shorthand == shorthand);
		}

		public IEnumerator<FieldInfo> GetEnumerator() {
			return items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return items.GetEnumerator();
		}
	}
}
