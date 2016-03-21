/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.dnx
 * 
 */

using System;
using System.Text.RegularExpressions;

namespace Piranha.Data
{
	/// <summary>
	/// Utility methods
	/// </summary>
	public static class Utils
    {
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
