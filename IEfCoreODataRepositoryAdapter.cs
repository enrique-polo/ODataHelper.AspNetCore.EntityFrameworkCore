using Microsoft.EntityFrameworkCore;
using ODataHelper.AspNetCore.Abstractions;

namespace ODataHelper.AspNetCore.EntityFrameworkCore
{
    public interface IEfCoreODataRepositoryAdapter<out TDbContext, TEntity> : IODataHelperRepositoryAdapter<TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        TDbContext Context { get; }
    }
}
