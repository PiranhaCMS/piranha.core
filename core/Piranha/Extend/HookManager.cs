/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections.Generic;
using Piranha.Data;

namespace Piranha.Extend
{
    public sealed class HookManager
    {
        /// <summary>
        /// The standard repository hooks for a data model.
        /// </summary>
        public sealed class RepositoryHooks<T>
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

        //
        // Private hook collections.
        //
        private readonly Dictionary<Type, object> _onLoad;
        private readonly Dictionary<Type, object> _onBeforeSave;
        private readonly Dictionary<Type, object> _onAfterSave;
        private readonly Dictionary<Type, object> _onBeforeDelete;
        private readonly Dictionary<Type, object> _onAfterDelete;

        /// <summary>
        /// Delegate for repository events.
        /// </summary>
        public delegate void ModelDelegate<T>(T model);

        /// <summary>
        /// Gets the hooks available for aliases.
        /// </summary>
        public RepositoryHooks<Alias> Alias { get; private set; }

        /// <summary>
        /// Gets the hooks available for categories.
        /// </summary>
        public RepositoryHooks<Category> Category { get; private set; }

        /// <summary>
        /// Gets the hooks available for media.
        /// </summary>
        public RepositoryHooks<Media> Media { get; private set; }

        /// <summary>
        /// Gets the hooks available for media folders.
        /// </summary>
        public RepositoryHooks<MediaFolder> MediaFolder { get; private set; }

        /// <summary>
        /// Gets the hooks available for params.
        /// </summary>
        public RepositoryHooks<Param> Param { get; private set; }

        /// <summary>
        /// Gets the hooks available for sites.
        /// </summary>
        public RepositoryHooks<Site> Site { get; private set; }

        /// <summary>
        /// Gets the hooks available for tags.
        /// </summary>
        public RepositoryHooks<Tag> Tag { get; private set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public HookManager()
        {
            _onLoad = new Dictionary<Type, object>();
            _onBeforeSave = new Dictionary<Type, object>();
            _onAfterSave = new Dictionary<Type, object>();
            _onBeforeDelete = new Dictionary<Type, object>();
            _onAfterDelete = new Dictionary<Type, object>();

            Alias = new RepositoryHooks<Alias>();
            Category = new RepositoryHooks<Category>();
            Media = new RepositoryHooks<Media>();
            MediaFolder = new RepositoryHooks<MediaFolder>();
            Param = new RepositoryHooks<Param>();
            Site = new RepositoryHooks<Site>();
            Tag = new RepositoryHooks<Tag>();
        }

        /// <summary>
        /// Removes all hooks for the specified model type.
        /// </summary>
        internal void Clear<T>()
        {
            if (_onLoad.ContainsKey(typeof(T)))
                _onLoad.Remove(typeof(T));
            if (_onBeforeSave.ContainsKey(typeof(T)))
                _onBeforeSave.Remove(typeof(T));
            if (_onAfterSave.ContainsKey(typeof(T)))
                _onAfterSave.Remove(typeof(T));
            if (_onBeforeDelete.ContainsKey(typeof(T)))
                _onBeforeDelete.Remove(typeof(T));
            if (_onAfterDelete.ContainsKey(typeof(T)))
                _onAfterDelete.Remove(typeof(T));
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
                _onLoad[typeof(T)] = new List<ModelDelegate<T>>();
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
                _onBeforeSave[typeof(T)] = new List<ModelDelegate<T>>();
            ((List<ModelDelegate<T>>)_onBeforeSave[typeof(T)]).Add(hook);
        }

        /// <summary>
        /// Registers a new hook to be executed after the model
        /// is saved to the database.
        /// </summary>
        /// <param name="hook">The hook</param>
        internal void RegisterOnAfterSave<T>(ModelDelegate<T> hook)
        {
            if (!_onAfterSave.ContainsKey(typeof(T)))
                _onAfterSave[typeof(T)] = new List<ModelDelegate<T>>();
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
                _onBeforeDelete[typeof(T)] = new List<ModelDelegate<T>>();
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
                _onAfterDelete[typeof(T)] = new List<ModelDelegate<T>>();
            ((List<ModelDelegate<T>>)_onAfterDelete[typeof(T)]).Add(hook);
        }

        /// <summary>
        /// Executes the registered hooks on the given model.
        /// </summary>
        /// <param name="model">The model</param>
        internal void OnLoad<T>(T model)
        {
            if (_onLoad.ContainsKey(typeof(T)))
            {
                var hooks = (List<ModelDelegate<T>>)_onLoad[typeof(T)];

                foreach (var hook in hooks)
                    hook.Invoke(model);
            }
        }

        /// <summary>
        /// Executes the registered hooks on the given model.
        /// </summary>
        /// <param name="model">The model</param>
        internal void OnBeforeSave<T>(T model)
        {
            if (_onBeforeSave.ContainsKey(typeof(T)))
            {
                var hooks = (List<ModelDelegate<T>>)_onBeforeSave[typeof(T)];

                foreach (var hook in hooks)
                    hook.Invoke(model);
            }
        }

        /// <summary>
        /// Executes the registered hooks on the given model.
        /// </summary>
        /// <param name="model">The model</param>
        internal void OnAfterSave<T>(T model)
        {
            if (_onAfterSave.ContainsKey(typeof(T)))
            {
                var hooks = (List<ModelDelegate<T>>)_onAfterSave[typeof(T)];

                foreach (var hook in hooks)
                    hook.Invoke(model);
            }
        }

        /// <summary>
        /// Executes the registered hooks on the given model.
        /// </summary>
        /// <param name="model">The model</param>
        internal void OnBeforeDelete<T>(T model)
        {
            if (_onBeforeDelete.ContainsKey(typeof(T)))
            {
                var hooks = (List<ModelDelegate<T>>)_onBeforeDelete[typeof(T)];

                foreach (var hook in hooks)
                    hook.Invoke(model);
            }
        }

        /// <summary>
        /// Executes the registered hooks on the given model.
        /// </summary>
        /// <param name="model">The model</param>
        internal void OnAfterDelete<T>(T model)
        {
            if (_onAfterDelete.ContainsKey(typeof(T)))
            {
                var hooks = (List<ModelDelegate<T>>)_onAfterDelete[typeof(T)];

                // Execute all of the available hooks.
                foreach (var hook in hooks)
                    hook.Invoke(model);
            }
        }
    }
}
