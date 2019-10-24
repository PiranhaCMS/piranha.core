/*
 * Copyright (c) 2019 @121GWJolt
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

 using System;

namespace Piranha.Manager
{
    public enum ECrossOriginPolicy
    {
        None,
        Anonymous,
        UseCredentials
    }

    /// <summary>
    /// Defines custom script resources with sources, hashes, and other future features as needed.
    /// </summary>
    public class ManagerScriptDefinition : IEquatable<ManagerScriptDefinition>
    {
        /// <summary>
        /// The script source.
        /// </summary>
        public string Src { get; }

        /// <summary>
        /// The file hash.
        /// </summary>
        public string Integrity { get; }

        /// <summary>
        /// If true, set crossorigin to "use-credentials". Otherwise, set to "anonymous".
        /// </summary>
        public ECrossOriginPolicy CrossOriginValue { get; }

        public bool RenderBeforeCoreScripts { get; set; } = false;

        public string GetCrossOriginValueStrValue(bool includeAttributeName = false)
        {
            switch (CrossOriginValue)
            {
                case ECrossOriginPolicy.None:
                    return "";
                case ECrossOriginPolicy.Anonymous:
                    return includeAttributeName ? "crossorigin=\"anonymous\"": "anonymous";
                case ECrossOriginPolicy.UseCredentials:
                    return includeAttributeName ? "crossorigin=\"use-credentials\"" : "use-credentials";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// The script type.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Get the hash code for this script.
        /// </summary>
        /// <remarks>The integrity hash will still be unique to the file, even moreso than the address. If the same file gets loaded with SRI hashes from two different sources they'll still be labeled the same file.</remarks>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Src != null ? Src.GetHashCode() : 0) * 397) ^ (Integrity != null ? Integrity.GetHashCode() : 0);
            }
        }

        public ManagerScriptDefinition(string src, string integrity = null, ECrossOriginPolicy crossOriginValue = ECrossOriginPolicy.Anonymous, string type = "text/javascript")
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            if (string.IsNullOrWhiteSpace(src))
            {
                throw new ArgumentException("Source url must not be null or whitespace.", nameof(src));
            }

            Src = src;
            Integrity = integrity;
            CrossOriginValue = crossOriginValue;
            Type = type;
        }

        /// <summary>
        /// WARNING: DO NOT USE THIS VALUE FOR INJECTING INTO A PAGE. IT IS NOT SANITIZED. IT SHOULD ONLY BE USED FOR TESTING OR DISPLAY PURPOSES ONLY.
        /// Returns a text string of what a rendered script tag for this script would look like.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"<script type=\"{Type}\" src=\"{Src}\"{(string.IsNullOrWhiteSpace(Integrity) ? "":$" integrity=\"{Integrity}\" {GetCrossOriginValueStrValue(true)}")}></script>";

        /// <summary>
        /// Backwards compatibility for the original string list.
        /// </summary>
        /// <param name="src"></param>
        public static implicit operator ManagerScriptDefinition(string src) => new ManagerScriptDefinition(src);

        /// <summary>
        /// Enables KVP-like list insertion.
        /// </summary>
        /// <param name="valTup"></param>
        public static implicit operator ManagerScriptDefinition((string src, string integrity) valTup) => new ManagerScriptDefinition(valTup.src, valTup.integrity);

        public bool Equals(ManagerScriptDefinition other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Integrity == null ? Src == other.Src : Integrity == other.Integrity;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == this.GetType() && Equals((ManagerScriptDefinition) obj);
        }
    }
}
