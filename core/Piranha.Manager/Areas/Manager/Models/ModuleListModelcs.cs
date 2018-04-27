using System.Linq;
using System.Collections.Generic;

namespace Piranha.Areas.Manager.Models
{
    public class ModuleListModel
    {
        /// <summary>
        /// Gets/sets the dictionary with the author as the key
        /// and matching modules as the value
        /// </summary>
        public Dictionary<string, List<Extend.IModule>> Modules { get; set; }

        /// <summary>
        /// Get the model with all the modules loaded into it
        /// </summary>
        /// <returns></returns>
        public static ModuleListModel Get() {
            return new ModuleListModel() {
                Modules = App.Modules
                    .Select(m => m.Instance)
                    .GroupBy(m => m.Author)
                    .ToDictionary(m => m.Key, m => m.ToList())
            };
        }
    }
}
