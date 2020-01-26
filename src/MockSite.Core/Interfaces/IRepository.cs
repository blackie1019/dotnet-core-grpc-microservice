using System.Threading.Tasks;

namespace MockSite.Core.Interfaces
{
    public interface IRepository<TEntity, in TKey>
    where TEntity: class
    {
        Task<int> Create(TEntity entity);

        Task Update(TEntity entity);

        Task Delete(TKey key);

        Task<TEntity[]> GetAll();

        Task<TEntity> GetById(TKey id);
    }
}