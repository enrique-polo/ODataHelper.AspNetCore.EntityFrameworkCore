using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ODataHelper.AspNetCore.EntityFrameworkCore
{
    public class EfCoreODataRepositoryAdapter<TDbContext, TEntity> 
        : ODataHelperRepositoryAdapter<TEntity>, 
            IEfCoreODataRepositoryAdapter<TDbContext, TEntity>, 
            IDisposable
        where TDbContext : DbContext
        where TEntity : class
    {
        public TDbContext Context { get; }
        protected EfCoreODataRepositoryAdapter(TDbContext dbContext)
        {
            Context = dbContext;
        }
        public override IQueryable<TEntity> AsQueryable()
        {
            var query = Context.Set<TEntity>();
            return query;
        }
        protected override async ValueTask<TEntity> GetEntityAsync(object key, IQueryable<TEntity> query)
        {
            var result = await Context.Set<TEntity>().FindAsync(key).ConfigureAwait(false);
            return result;
        }
        protected override ValueTask<TEntity> InsertEntityAsync(TEntity entity)
        {
            var result = Context.Set<TEntity>().Add(entity);
            return new ValueTask<TEntity>(result.Entity);
        }
        protected override ValueTask<TEntity> UpdateEntityAsync(TEntity entity)
        {
            var result = Context.Set<TEntity>().Update(entity);
            return new ValueTask<TEntity>(result.Entity);
        }
        protected override ValueTask<int> RemoveEntityAsync(TEntity entity)
        {
            var entry = Context.Set<TEntity>().Remove(entity);
            var result = entry.State == EntityState.Deleted ? 1 : 0;
            return new ValueTask<int>(result);
        }
        public override async ValueTask<int> PersistChangesAsync()
        {
            var result = await Context.SaveChangesAsync().ConfigureAwait(false);
            return result;
        }
        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
