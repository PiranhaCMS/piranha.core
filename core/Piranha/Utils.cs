/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Piranha
{
    /// <summary>
    /// Utility methods.
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
        /// Generates a slug from the given string.
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns>The slug</returns>
        public static string GenerateSlug(string str) {
            // Trim & make lower case
            var slug = str.Trim().ToLower();

            // Remove whitespaces
            slug = Regex.Replace(slug.Replace("-", " "), @"\s+", " ").Replace(" ", "-");

            // Convert culture specific characters
            slug = slug
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
                .Replace("ì", "i");

            // Remove special characters
            slug = Regex.Replace(slug, @"[^a-z0-9-/]", "");

            if (slug.EndsWith("-"))
                slug = slug.Substring(0, slug.LastIndexOf("-"));
            if (slug.StartsWith("-"))
                slug = slug.Substring(Math.Min(slug.IndexOf("-") + 1, slug.Length));
            return slug;
        }


        /// <summary>
        /// Generates a ETag from the given name and date.
        /// </summary>
        /// <param name="name">The resource name</param>
        /// <param name="date">The modification date</param>
        /// <returns>The etag</returns>
        public static string GenerateETag(string name, DateTime date) {
            var encoding = new UTF8Encoding();

            using (var crypto = MD5.Create()) {
                var str = name + date.ToString("yyyy-MM-dd HH:mm:ss");
                var bytes = crypto.ComputeHash(encoding.GetBytes(str));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
