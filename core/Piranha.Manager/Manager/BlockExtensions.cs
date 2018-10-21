using System.Collections.Generic;
using System.Linq;
using Piranha.Extend;
using Piranha.Extend.Fields;

namespace Piranha.Manager.Manager
{
    public static class BlockExtensions
    {
        public static IEnumerable<string> GetFieldNames(this Block block){
            return block.GetType()
                .GetProperties()
                .Where(p => typeof(IField).IsAssignableFrom(p.PropertyType))
                .Select(p => p.Name);
        }
    }
}