using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Piranha.EF.Data;

namespace Piranha.EF
{
    public interface IDb
    {
        DbSet<Block> Blocks { get; set; }
        DbSet<BlockField> BlockFields { get; set; }

        DbSet<BlockType> BlockTypes { get; set; }

        DbSet<Category> Categories { get; set; }

        DbSet<Page> Pages { get; set; }

        DbSet<PageType> PageTypes { get; set; }

        DbSet<PageField> PageFields { get; set; }

        DbSet<Post> Posts { get; set; }

        DbSet<Tag> Tags { get; set; }

        DatabaseFacade Database { get; }

        DbSet<T> Set<T>() where T : class;

        int SaveChanges();

        int SaveChanges(bool acceptAllChangesOnSuccess);

        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}