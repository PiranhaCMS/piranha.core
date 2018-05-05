/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System.Collections.Generic;
using System.Linq;

namespace Piranha.Security
{
    /// <summary>
    /// An item in the permission manager.
    /// </summary>
    public class PermissionItem
    {
        /// <summary>
        /// The name of the claim.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The display title.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Gets/sets the optional category for grouping.
        /// </summary>
        public string Category { get; set; }
    }

    /// <summary>
    /// The permission manager.
    /// </summary>
    public class PermissionManager 
    {
        private readonly Dictionary<string, IList<PermissionItem>> _modules;

        public IList<PermissionItem> this[string module]
        {
            get
            {
                if (_modules.TryGetValue(module, out var items))
                    return items;
                
                _modules[module] = items = new List<PermissionItem>();

                return items;
            }
        }

        public PermissionManager()
        {
            _modules = new Dictionary<string, IList<PermissionItem>>();
        }

        public IList<string> GetModules()
        {
            return _modules.Keys.OrderBy(k => k).ToList();
        }

        public IList<PermissionItem> GetPermissions(string module) 
        {
            return this[module].OrderBy(p => p.Name).ToList();
        }

        public IList<PermissionItem> GetPermissions() 
        {
            var all = new Dictionary<string, PermissionItem>();

            foreach (var module in GetModules())
            {
                foreach (var permission in GetPermissions(module))
                {
                    all[permission.Name] = permission;
                }
            }
            return all.Values.OrderBy(k => k.Name).ToList();
        }
    }
}