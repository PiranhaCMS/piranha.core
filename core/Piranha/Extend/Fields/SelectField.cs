/*
 * Copyright (c) 2018 Håkan Edling
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
    [FieldType(Name = "Select", Shorthand = "Select")]
    public class SelectField<T> : SelectFieldBase where T : struct
    {
        /// <summary>
        /// The static list of available items.
        /// </summary>
        private static List<SelectFieldItem> items = new List<SelectFieldItem>();

        /// <summary>
        /// Initialization mutex.
        /// </summary>
        private static object mutex = new object();

        /// <summary>
        /// The initialization state.
        /// </summary>
        private static bool isInitialized = false;

        /// <summary>
        /// Gets/sets the selected value.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets the current enum type.
        /// </summary>
        public override Type EnumType {
            get { return typeof(T); }
        }

        /// <summary>
        /// Gets/sets the selected value as a string.
        /// </summary>
        public override string EnumValue {
            get { return Value.ToString(); }
            set { Value = (T)Enum.Parse(typeof(T), value); }
        }

        /// <summary>
        /// Gets the available items to choose from. Primarily used 
        /// from the manager interface.
        /// </summary>
        public override List<SelectFieldItem> Items {
            get { 
                InitMetaData();
                
                return items; 
            }
        }

        /// <summary>
        /// Gets the list item title if this field is used in
        /// a collection regions.
        /// </summary>
        public override string GetTitle() {
            return GetEnumTitle((Enum)(object)Value);
        }

        /// <summary>
        /// Initializes the field for client use.
        /// </summary>
        /// <param name="api">The current api</param>
        public override void Init(IApi api) {
            InitMetaData();
        }

        /// <summary>
        /// Gets the display title for the given enum. If the DisplayAttribute
        /// is present it's description is returned, otherwise the string r
        /// epresentation of the enum.
        /// </summary>
        /// <param name="val">The enum value</param>
        /// <returns>The display title</returns>
        private string GetEnumTitle(Enum val) {
            var members = typeof(T).GetMember(val.ToString());

            if (members != null && members.Length > 0) {
                var attrs = members[0].GetCustomAttributes(false);
                
                foreach (var attr in attrs) {
                    if (attr is DisplayAttribute) {
                        return  ((DisplayAttribute)attr).Description;
                    }
                }
            }
            return val.ToString();
        }

        /// <summary>
        /// Initializes the meta data needed in the manager interface.
        /// </summary>
        private void InitMetaData() {
            if (isInitialized)
                return;

            lock (mutex) {
                if (isInitialized)
                    return;

                foreach (var val in Enum.GetValues(typeof(T))) {
                    items.Add(new SelectFieldItem() {
                        Title = GetEnumTitle((Enum)val),
                        Value = (Enum)val
                    });
                }
                isInitialized = true;
            }
        }
    }
}