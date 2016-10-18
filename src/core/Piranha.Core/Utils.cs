/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Piranha
{
    /// <summary>
    /// Utility methods
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Gets a subset of the given array as a new array.
        /// </summary>
        /// <typeparam name="T">The array type</typeparam>
        /// <param name="arr">The array</param>
        /// <param name="startpos">The startpos</param>
        /// <param name="length">The length</param>
        /// <returns>The new array</returns>
        public static T[] Subset<T>(this T[] arr, int startpos = 0, int length = 0) {
            List<T> tmp = new List<T>();

            length = length > 0 ? length : arr.Length - startpos;

            for (var i = 0; i < arr.Length; i++) {
                if (i >= startpos && i < (startpos + length))
                    tmp.Add(arr[i]);
            }
            return tmp.ToArray();
        }

        /// <summary>
        /// Implodes the string enumerable into a string.
        /// </summary>
        /// <param name="strs">The strings to implode</param>
        /// <param name="sep">The optional separator</param>
        /// <returns>The string</returns>
        public static string Implode(this IEnumerable<string> strs, string sep = "") {
            var sb = new StringBuilder();

            foreach (var str in strs) {
                if (sb.Length > 0 && sep != "")
                    sb.Append(sep);
                sb.Append(str);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generates a slug from the given string.
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns>The slug</returns>
        public static string GenerateSlug(string str) {
            // Default slug generation
            var slug = Regex.Replace(str.ToLower()
                .Replace(" ", "-")
                .Replace("å", "a")
                .Replace("ä", "a")
                .Replace("á", "a")
                .Replace("à", "a")
                .Replace("ö", "o")
                .Replace("ó", "o")
                .Replace("ò", "o")
                .Replace("é", "e")
                .Replace("è", "e")
                .Replace("í", "i")
                .Replace("ì", "i"), @"[^a-z0-9-/]", "").Replace("--", "-");

            if (slug.EndsWith("-"))
                slug = slug.Substring(0, slug.LastIndexOf("-"));
            if (slug.StartsWith("-"))
                slug = slug.Substring(Math.Min(slug.IndexOf("-") + 1, slug.Length));
            return slug;
        }
    }
}
