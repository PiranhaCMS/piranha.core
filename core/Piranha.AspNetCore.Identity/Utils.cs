/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.Security.Cryptography;
using System.Text;

namespace Piranha.AspNetCore.Identity
{
    public static class Utils
    {
        /// <summary>
        /// Gets the gravatar URL from the given parameters.
        /// </summary>
        /// <param name="email">The email address</param>
        /// <param name="size">The requested size</param>
        /// <returns>The gravatar URL</returns>
        public static string GetGravatarUrl(string email, int size = 0)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(email));

                var sb = new StringBuilder(bytes.Length * 2);
                for (var n = 0; n < bytes.Length; n++)
                {
                    sb.Append(bytes[n].ToString("X2"));
                }
                return "https://www.gravatar.com/avatar/" + sb.ToString().ToLower() +
                       (size > 0 ? "?s=" + size : "");
            }
        }
    }
}