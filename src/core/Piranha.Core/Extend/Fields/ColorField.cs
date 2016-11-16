/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Extend.Fields
{
    [Field(Name = "Color", Shorthand = "Color")]
    public class ColorField : SimpleField<string>
    {
        public static implicit operator ColorField(string str) {
            return new ColorField() { Value = str };
        } 
    }
}
