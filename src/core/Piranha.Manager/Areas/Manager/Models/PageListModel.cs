using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piranha.Areas.Manager.Models
{
    public class PageListModel
    {
        public class PageListItem
        {
            public Guid Id { get; set; }
            public string Type { get; set; }
            public DateTime? Published { get; set; }
            public DateTime Created { get; set; }
            public DateTime LastModified { get; set; }
        }

        public IList<PageListItem> Items { get; set; }

        public PageListModel() {
            Items = new List<PageListItem>();
        }

        public static PageListModel Get(IApi api) {
            var model = new PageListModel();

            return model;
        }
    }
}
