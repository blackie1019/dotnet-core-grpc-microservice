using System.Threading.Tasks;

namespace MockSite.Core.Interfaces
{
    public interface IRedisRepository : IRepository
    {
        Task DeleteAll();
    }
}