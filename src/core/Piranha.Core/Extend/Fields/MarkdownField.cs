/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using HeyRed.MarkdownSharp;

namespace Piranha.Extend.Fields
{
    [Field(Name = "Markdown", Shorthand = "Markdown")]
    public class MarkdownField : SimpleField<string>
    {
        public static implicit operator MarkdownField(string str) {
            return new MarkdownField() { Value = str };
        } 

        public string ToHtml() {
            var converter = new Markdown();
            return converter.Transform(Value);
        }
    }
}
