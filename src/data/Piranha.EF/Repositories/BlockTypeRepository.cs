/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Piranha.Repositories;

namespace Piranha.EF.Repositories
{
    /// <summary>
    /// Block type repository.
    /// </summary>
    public class BlockTypeRepository : IBlockTypeRepository
    {
        #region Members
        private readonly Db db;
        #endregion

        /// <summary>
        /// Default internal constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        internal BlockTypeRepository(Db db) {
            this.db = db;
        }

        /// <summary>
        /// Gets the block type with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The block type</returns>
        public Extend.BlockType GetById(string id) {
            var type = db.BlockTypes
                .FirstOrDefault(t => t.Id == id);

            if (type != null)
                return JsonConvert.DeserializeObject<Extend.BlockType>(type.Body);
            return null;
        }

        /// <summary>
        /// Gets all available block types.
        /// </summary>
        /// <returns>The block types</returns>
        public IList<Extend.BlockType> Get() {
            var result = new List<Extend.BlockType>();

            var types = db.BlockTypes.ToList();
            foreach (var type in types)
                result.Add(JsonConvert.DeserializeObject<Extend.BlockType>(type.Body));

            return result.OrderBy(t => t.Title).ToList();
        }

        /// <summary>
        /// Saves the given block type.
        /// </summary>
        /// <param name="blockType">The block type</param>
        public void Save(Extend.BlockType blockType) {
            var type = db.BlockTypes
                .FirstOrDefault(t => t.Id == blockType.Id);

            if (type == null) {
                type = new Data.BlockType() {
                    Id = blockType.Id
                };
                db.BlockTypes.Add(type);
            }
            type.Body = JsonConvert.SerializeObject(blockType);

            db.SaveChanges();
        }

        /// <summary>
        /// Deletes the given block type.
        /// </summary>
        /// <param name="blockType">The block type</param>
        public void Delete(Extend.BlockType blockType) {
            Delete(blockType.Id);
        }

        /// <summary>
        /// Deletes the block type with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public void Delete(string id) {
            var type = db.BlockTypes
                .FirstOrDefault(t => t.Id == id);

            if (type != null) {
                db.BlockTypes.Remove(type);
                db.SaveChanges();
            }
        }
    }
}
