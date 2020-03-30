/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using Piranha.Models;

namespace Piranha.Runtime
{
    public sealed class HookManager
    {
        /// <summary>
        /// The standard repository hooks for a data model.
        /// </summary>
        public class ServiceHooks<T>
        {
            /// <summary>
            /// Registers a new hook to be executed after the model
            /// has been loaded but BEFORE it has been added into
            /// the cache.
            /// </summary>
            /// <param name="hook">The hook</param>
            public void RegisterOnLoad(ModelDelegate<T> hook)
            {
                App.Hooks.RegisterOnLoad(hook);
            }

            /// <summary>
            /// Registers a new hook to be executed before the model
            /// is saved to the database.
            /// </summary>
            /// <param name="hook">The hook</param>
            public void RegisterOnBeforeSave(ModelDelegate<T> hook)
            {
                App.Hooks.RegisterOnBeforeSave(hook);
            }

            /// <summary>
            /// Registers a new hook to be executed after the model
            /// is saved to the database.
            /// </summary>
            /// <param name="hook">The hook</param>
            public void RegisterOnAfterSave(ModelDelegate<T> hook)
            {
                App.Hooks.RegisterOnAfterSave(hook);
            }

            /// <summary>
            /// Registers a new hook to be executed before the model
            /// is deleted from the database.
            /// </summary>
            /// <param name="hook">The hook</param>
            public void RegisterOnBeforeDelete(ModelDelegate<T> hook)
            {
                App.Hooks.RegisterOnBeforeDelete(hook);
            }

            /// <summary>
            /// Registers a new hook to be executed after the model
            /// is deleted from the database.
            /// </summary>
            /// <param name="hook">The hook</param>
            public void RegisterOnAfterDelete(ModelDelegate<T> hook)
            {
                App.Hooks.RegisterOnAfterDelete(hook);
            }

            /// <summary>
            /// Removes all registered hooks.
            /// </summary>
            public void Clear()
            {
                App.Hooks.Clear<T>();
            }
        }

        public class ValidationServiceHooks<T> : ServiceHooks<T>
        {
            /// <summary>
            /// Registers a new hook to be executed after the OnBeforeSave
            /// hook as been executed but before the model is saved to the
            /// database. This hook should be used to extend default model
            /// validation.
            /// </summary>
            /// <param name="hook">The hook</param>
            public void RegisterOnValidate(ModelDelegate<T> hook)
            {
                App.Hooks.RegisterOnLoad<T>(hook);
            }
        }

        //
        // Private hook collections.
        //
        private readonly Dictionary<Type, object> _onLoad = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _onBeforeSave = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _onValidate = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _onAfterSave = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _onBeforeDelete = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _onAfterDelete = new Dictionary<Type, object>();

        /// <summary>
        /// Delegate for repository events.
        /// </summary>
        public delegate void ModelDelegate<T>(T model);

        /// <summary>
        /// Delegate for generating a slug.
        /// </summary>
        /// <param name="str">The input string</param>
        /// <returns>The generated slug</returns>
        public delegate string SlugDelegate(string str);

        /// <summary>
        /// Delegate for generating the public sitemap.
        /// </summary>
        /// <param name="sitemap">The default sitemap</param>
        /// <returns>The updated sitemap</returns>
        public delegate Sitemap SitemapDelegate(Sitemap sitemap);

        /// <summary>
        /// Gets the hooks available for aliases.
        /// </summary>
        public ServiceHooks<Alias> Alias { get; } = new ServiceHooks<Alias>();

        /// <summary>
        /// Gets the hooks available for comments.
        /// </summary>
        public ValidationServiceHooks<Comment> Comments { get; } = new ValidationServiceHooks<Comment>();

        /// <summary>
        /// Gets the hooks available for media.
        /// </summary>
        public ServiceHooks<Media> Media { get; } = new ServiceHooks<Media>();

        /// <summary>
        /// Gets the hooks available for media folders.
        /// </summary>
        public ServiceHooks<MediaFolder> MediaFolder { get; } = new ServiceHooks<MediaFolder>();

        /// <summary>
        /// Gets the hooks available for pages.
        /// </summary>
        public ServiceHooks<PageBase> Pages { get; } = new ServiceHooks<PageBase>();

        /// <summary>
        /// Gets the hooks available for params.
        /// </summary>
        public ServiceHooks<Param> Param { get; } = new ServiceHooks<Param>();

        /// <summary>
        /// Gets the hooks available for posts.
        /// </summary>
        public ServiceHooks<PostBase> Posts { get; } = new ServiceHooks<PostBase>();

        /// <summary>
        /// Gets the hooks available for sites.
        /// </summary>
        public ServiceHooks<Site> Site { get; } = new ServiceHooks<Site>();

        /// <summary>
        /// Gets the hooks available for sites.
        /// </summary>
        public ServiceHooks<SiteContentBase> SiteContent { get; } = new ServiceHooks<SiteContentBase>();

        /// <summary>
        /// Gets the hook for slug generation.
        /// </summary>
        public SlugDelegate OnGenerateSlug { get; set; }

        /// <summary>
        /// Gets the hook for generating the public sitemap.
        /// </summary>
        public SitemapDelegate OnGenerateSitemap { get; set; }

        /// <summary>
        /// Removes all hooks for the specified model type.
        /// </summary>
        internal void Clear<T>()
        {
            if (_onLoad.ContainsKey(typeof(T)))
            {
                _onLoad.Remove(typeof(T));
            }
            if (_onBeforeSave.ContainsKey(typeof(T)))
            {
                _onBeforeSave.Remove(typeof(T));
            }
            if (_onValidate.ContainsKey(typeof(T)))
            {
                _onValidate.Remove(typeof(T));
            }
            if (_onAfterSave.ContainsKey(typeof(T)))
            {
                _onAfterSave.Remove(typeof(T));
            }
            if (_onBeforeDelete.ContainsKey(typeof(T)))
            {
                _onBeforeDelete.Remove(typeof(T));
            }
            if (_onAfterDelete.ContainsKey(typeof(T)))
            {
                _onAfterDelete.Remove(typeof(T));
            }
        }

        /// <summary>
        /// Registers a new hook to be executed after the model
        /// has been loaded but BEFORE it has been added into
        /// the cache.
        /// </summary>
        /// <param name="hook">The hook</param>
        internal void RegisterOnLoad<T>(ModelDelegate<T> hook)
        {
            if (!_onLoad.ContainsKey(typeof(T)))
            {
                _onLoad[typeof(T)] = new List<ModelDelegate<T>>();
            }
            ((List<ModelDelegate<T>>)_onLoad[typeof(T)]).Add(hook);
        }

        /// <summary>
        /// Registers a new hook to be executed before the model
        /// is saved to the database.
        /// </summary>
        /// <param name="hook">The hook</param>
        internal void RegisterOnBeforeSave<T>(ModelDelegate<T> hook)
        {
            if (!_onBeforeSave.ContainsKey(typeof(T)))
            {
                _onBeforeSave[typeof(T)] = new List<ModelDelegate<T>>();
            }
            ((List<ModelDelegate<T>>)_onBeforeSave[typeof(T)]).Add(hook);
        }

        /// <summary>
        /// Registers a new hook to be executed before the model
        /// is saved to the database.
        /// </summary>
        /// <param name="hook">The hook</param>
        internal void RegisterOnValidate<T>(ModelDelegate<T> hook)
        {
            if (!_onValidate.ContainsKey(typeof(T)))
            {
                _onValidate[typeof(T)] = new List<ModelDelegate<T>>();
            }
            ((List<ModelDelegate<T>>)_onValidate[typeof(T)]).Add(hook);
        }

        /// <summary>
        /// Registers a new hook to be executed after the model
        /// is saved to the database.
        /// </summary>
        /// <param name="hook">The hook</param>
        internal void RegisterOnAfterSave<T>(ModelDelegate<T> hook)
        {
            if (!_onAfterSave.ContainsKey(typeof(T)))
            {
                _onAfterSave[typeof(T)] = new List<ModelDelegate<T>>();
            }
            ((List<ModelDelegate<T>>)_onAfterSave[typeof(T)]).Add(hook);
        }

        /// <summary>
        /// Registers a new hook to be executed before the model
        /// is deleted from the database.
        /// </summary>
        /// <param name="hook">The hook</param>
        internal void RegisterOnBeforeDelete<T>(ModelDelegate<T> hook)
        {
            if (!_onBeforeDelete.ContainsKey(typeof(T)))
            {
                _onBeforeDelete[typeof(T)] = new List<ModelDelegate<T>>();
            }
            ((List<ModelDelegate<T>>)_onBeforeDelete[typeof(T)]).Add(hook);
        }

        /// <summary>
        /// Registers a new hook to be executed after the model
        /// is deleted from the database.
        /// </summary>
        /// <param name="hook">The hook</param>
        internal void RegisterOnAfterDelete<T>(ModelDelegate<T> hook)
        {
            if (!_onAfterDelete.ContainsKey(typeof(T)))
            {
                _onAfterDelete[typeof(T)] = new List<ModelDelegate<T>>();
            }
            ((List<ModelDelegate<T>>)_onAfterDelete[typeof(T)]).Add(hook);
        }

        /// <summary>
        /// Executes the registered hooks on the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public void OnLoad<T>(T model)
        {
            if (_onLoad.ContainsKey(typeof(T)))
            {
                var hooks = (List<ModelDelegate<T>>)_onLoad[typeof(T)];

                foreach (var hook in hooks)
                {
                    hook.Invoke(model);
                }
            }
        }

        /// <summary>
        /// Executes the registered hooks on the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public void OnBeforeSave<T>(T model)
        {
            if (_onBeforeSave.ContainsKey(typeof(T)))
            {
                var hooks = (List<ModelDelegate<T>>)_onBeforeSave[typeof(T)];

                foreach (var hook in hooks)
                {
                    hook.Invoke(model);
                }
            }
        }

        /// <summary>
        /// Executes the registered hooks on the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public void OnValidate<T>(T model)
        {
            if (_onValidate.ContainsKey(typeof(T)))
            {
                var hooks = (List<ModelDelegate<T>>)_onValidate[typeof(T)];

                foreach (var hook in hooks)
                {
                    hook.Invoke(model);
                }
            }
        }

        /// <summary>
        /// Executes the registered hooks on the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public void OnAfterSave<T>(T model)
        {
            if (_onAfterSave.ContainsKey(typeof(T)))
            {
                var hooks = (List<ModelDelegate<T>>)_onAfterSave[typeof(T)];

                foreach (var hook in hooks)
                {
                    hook.Invoke(model);
                }
            }
        }

        /// <summary>
        /// Executes the registered hooks on the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public void OnBeforeDelete<T>(T model)
        {
            if (_onBeforeDelete.ContainsKey(typeof(T)))
            {
                var hooks = (List<ModelDelegate<T>>)_onBeforeDelete[typeof(T)];

                foreach (var hook in hooks)
                {
                    hook.Invoke(model);
                }
            }
        }

        /// <summary>
        /// Executes the registered hooks on the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public void OnAfterDelete<T>(T model)
        {
            if (_onAfterDelete.ContainsKey(typeof(T)))
            {
                var hooks = (List<ModelDelegate<T>>)_onAfterDelete[typeof(T)];

                // Execute all of the available hooks.
                foreach (var hook in hooks)
                {
                    hook.Invoke(model);
                }
            }
        }
    }
}
