using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Piranha.Manager
{

    /// <summary>
    /// Defines custom script resources with sources, hashes, and other future features as needed.
    /// </summary>
    public class ManagerScriptDefinition
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
        public bool CrossOriginUseCredentials { get; }

        /// <summary>
        /// The script type.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Get the hash code for this script.
        /// </summary>
        /// <remarks>The integrity hash will still be unique to the file, even moreso than the address. If the same file gets loaded with SRI hashes from two different sources they'll still be labeled the same file.</remarks>
        /// <returns></returns>
        public override int GetHashCode() => Integrity?.GetHashCode() ?? Src.GetHashCode();

        public ManagerScriptDefinition(string src, string integrity = null, bool crossOriginUseCredentials = false, string type = "text/javascript")
        {
            if (src == null) throw new ArgumentNullException(nameof(src));
            if (string.IsNullOrWhiteSpace(src)) throw new ArgumentException("Source url must not be null or whitespace.", nameof(src));
            Src = src;
            Integrity = integrity;
            CrossOriginUseCredentials = crossOriginUseCredentials;
            Type = type;
        }

        /// <summary>
        /// WARNING: DO NOT USE THIS VALUE FOR INJECTING INTO A PAGE. IT IS NOT SANITIZED. IT SHOULD ONLY BE USED FOR TESTING OR DISPLAY PURPOSES ONLY.
        /// Returns a text string of what a rendered script tag for this script would look like.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"<script type=\"{Type}\" src=\"{Src}\"{(string.IsNullOrWhiteSpace(Integrity) ? "":$" integrity=\"{Integrity}\" crossorigin=\"{(CrossOriginUseCredentials ? "use-credentials":"anonymous")}\"")}></script>";

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
    }
}
