/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Extend.Fields
{
    /// <summary>
    /// Generic select field.
    /// </summary>
    [FieldType(Name = "Select", Shorthand = "Select", Component = "select-field")]
    public class SelectField<T> : SelectFieldBase, IEquatable<SelectField<T>> where T : struct
    {
        /// <summary>
        /// The static list of available items.
        /// </summary>
        private static readonly List<SelectFieldItem> _items = new List<SelectFieldItem>();

        /// <summary>
        /// Initialization mutex.
        /// </summary>
        private static readonly object Mutex = new object();

        /// <summary>
        /// The initialization state.
        /// </summary>
        private static bool IsInitialized;

        /// <summary>
        /// Gets/sets the selected value.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets the current enum type.
        /// </summary>
        public override Type EnumType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Gets/sets the selected value as a string.
        /// </summary>
        public override string EnumValue
        {
            get { return Value.ToString(); }
            set { Value = (T)Enum.Parse(typeof(T), value); }
        }

        /// <summary>
        /// Gets the available items to choose from. Primarily used
        /// from the manager interface.
        /// </summary>
        public override List<SelectFieldItem> Items
        {
            get
            {
                InitMetaData();

                return _items;
            }
        }

        /// <summary>
        /// Gets the list item title if this field is used in
        /// a collection regions.
        /// </summary>
        public override string GetTitle()
        {
            return GetEnumTitle((Enum)(object)Value);
        }

        /// <summary>
        /// Initializes the field for client use.
        /// </summary>
        /// <param name="api">The current api</param>
        public override void Init(IApi api)
        {
            InitMetaData();
        }

        /// <summary>
        /// Gets the hash code for the field.
        /// </summary>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Checks if the given object is equal to the field.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>True if the fields are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is SelectField<T> field)
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
        public virtual bool Equals(SelectField<T> obj)
        {
            if (obj == null)
            {
                return false;
            }
            return EqualityComparer<T>.Default.Equals(Value, obj.Value);
        }

        /// <summary>
        /// Checks if the fields are equal.
        /// </summary>
        /// <param name="field1">The first field</param>
        /// <param name="field2">The second field</param>
        /// <returns>True if the fields are equal</returns>
        public static bool operator ==(SelectField<T> field1, SelectField<T> field2)
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
        public static bool operator !=(SelectField<T> field1, SelectField<T> field2)
        {
            return !(field1 == field2);
        }

        /// <summary>
        /// Gets the display title for the given enum. If the DisplayAttribute
        /// is present it's description is returned, otherwise the string
        /// representation of the enum.
        /// </summary>
        /// <param name="val">The enum value</param>
        /// <returns>The display title</returns>
        private string GetEnumTitle(Enum val)
        {
            var members = typeof(T).GetMember(val.ToString());

            if (members != null && members.Length > 0)
            {
                var attrs = members[0].GetCustomAttributes(false);

                foreach (var attr in attrs)
                {
                    if (attr is DisplayAttribute)
                    {
                        return ((DisplayAttribute)attr).Description;
                    }
                }
            }
            return val.ToString();
        }

        /// <summary>
        /// Initializes the meta data needed in the manager interface.
        /// </summary>
        private void InitMetaData()
        {
            if (IsInitialized)
                return;

            lock (Mutex)
            {
                if (IsInitialized)
                {
                    return;
                }

                foreach (var val in Enum.GetValues(typeof(T)))
                {
                    _items.Add(new SelectFieldItem
                    {
                        Title = GetEnumTitle((Enum)val),
                        Value = (Enum)val
                    });
                }
                IsInitialized = true;
            }
        }
    }
}