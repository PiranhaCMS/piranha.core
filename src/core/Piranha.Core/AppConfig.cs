using System.Collections.Generic;

namespace Piranha
{
    public sealed class AppConfig
    {
        public IList<Models.PageType> PageTypes { get; set; }

        public AppConfig() {
            PageTypes = new List<Models.PageType>();
        }

        internal void Ensure() {
            foreach (var pageType in PageTypes)
                pageType.Ensure();
        }
    }
}
