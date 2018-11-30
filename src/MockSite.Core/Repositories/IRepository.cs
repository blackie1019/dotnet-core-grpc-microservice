using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace MockSite.Core.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task Create(T instance);
        Task Update(T instance);
        Task Delete(T instance);
        Task<IEnumerable<T>> GetAll();
        Task<T> GetByPk(object obj);
    }
}