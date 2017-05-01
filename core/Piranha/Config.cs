/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha
{
    /// <summary>
    /// Class for easy access to built-in config parameters.
    /// </summary>
    public sealed class Config : IDisposable
    {
        #region Members
        /// <summary>
        /// The private api.
        /// </summary>
        private readonly Api api;

        /// <summary>
        /// The system config keys.
        /// </summary>
        public static readonly string CACHE_EXPIRES_MEDIA = "CacheExpiresMedia";
        public static readonly string CACHE_EXPIRES_PAGES = "CacheExpiresPages";
        #endregion

        #region Properties
        /// <summary>
        /// Gets the currently configured cache expiration
        /// in minutes for media.
        /// </summary>
        public int CacheExpiresMedia {
            get {
                var param = api.Params.GetByKey(CACHE_EXPIRES_MEDIA);
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 0;
            }
            set {
                var param = api.Params.GetByKey(CACHE_EXPIRES_MEDIA);
                if (param == null) {
                    param = new Data.Param() {
                        Key = CACHE_EXPIRES_MEDIA
                    };
                }
                param.Value = value.ToString();
                api.Params.Save(param);
            }
        }

        /// <summary>
        /// Gets the currently configured cache expiration
        /// in minutes for pages.
        /// </summary>
        public int CacheExpiresPages {
            get {
                var param = api.Params.GetByKey(CACHE_EXPIRES_PAGES);
                if (param != null)
                    return Convert.ToInt32(param.Value);
                return 0;
            }
            set {
                var param = api.Params.GetByKey(CACHE_EXPIRES_PAGES);
                if (param == null) {
                    param = new Data.Param() {
                        Key = CACHE_EXPIRES_PAGES
                    };
                }
                param.Value = value.ToString();
                api.Params.Save(param);
            }
        }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public Config(Api api) {
            this.api = api;
        }

        /// <summary>
        /// Disposes the config.
        /// </summary>
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}