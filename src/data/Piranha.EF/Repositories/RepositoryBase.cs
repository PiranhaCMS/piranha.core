using System;
using System.Linq;

namespace Piranha.EF.Repositories
{
    public abstract class RepositoryBase<T, TModel> where T : class, Data.IModel where TModel : class
    {
		protected Db db;

		protected RepositoryBase(Db db) {
			this.db = db;
		}

		public virtual TModel GetById(Guid id) {
			var result = Query().SingleOrDefault(e => e.Id == id);

			if (result != null)
				return Map(result);
			return null;
		}

		protected virtual IQueryable<T> Query() {
			return db.Set<T>();
		}

		protected virtual TModel Map(T result) {
			return Module.Mapper.Map<T, TModel>(result);
		}
    }
}
