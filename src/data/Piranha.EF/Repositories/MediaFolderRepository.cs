using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Repositories;

namespace Piranha.EF.Repositories
{
    public class MediaFolderRepository : RepositoryBase<Data.MediaFolder, Models.MediaFolder>, IMediaFolderRepository
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        internal MediaFolderRepository(IDb db) : base(db) { }

        /// <summary>
        /// Gets the available folders.
        /// </summary>
        /// <param name="parentId">The optional parent folder id</param>
        /// <returns>The available folders</returns>
        public IList<Models.MediaFolder> Get(Guid? parentId = null) {
            var folders = Query()
                .Where(f => f.ParentId == parentId)
                .OrderBy(f => f.Title)
                .ToList();
            var result = new List<Models.MediaFolder>();

            foreach (var folder in folders)
                result.Add(Map(folder));
            return result;
        }

        /// <summary>
        /// Moves the folder with the given id to the
        /// specified parent.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="parentId">The parent folder id</param>
        public void Move(Guid id, Guid? parentId) {
            var folder = Query().FirstOrDefault(f => f.Id == id);

            if (folder != null) {
                folder.ParentId = parentId;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Saves the given media folder.
        /// </summary>
        /// <param name="folder">The media folder</param>
        public void Save(Models.MediaFolder folder) {
            var item = Query().FirstOrDefault(f => f.Id == folder.Id);

            if (item == null) {
                item = new Data.MediaFolder() {
                    Id = folder.Id != Guid.Empty ? folder.Id : Guid.NewGuid()
                };
                db.MediaFolders.Add(item);
            }
            Module.Mapper.Map<Models.MediaFolder, Data.MediaFolder>(folder, item);
            db.SaveChanges();
        }

        /// <summary>
        /// Deletes the given media folder
        /// </summary>
        /// <param name="folder">The media folder</param>
        public void Delete(Models.MediaFolder folder) {
            Delete(folder.Id);
        }

        /// <summary>
        /// Deletes the media folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public void Delete(Guid id) {
            var folder = Query().FirstOrDefault(f => f.Id == id);

            if (folder != null) {
                db.MediaFolders.Remove(folder);
                db.SaveChanges();
            }
        }        
    }
}
