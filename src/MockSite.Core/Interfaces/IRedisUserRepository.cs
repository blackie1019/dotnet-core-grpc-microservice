using System.Threading.Tasks;
using MockSite.Core.Entities;

namespace MockSite.Core.Interfaces
{
    public interface IRedisUserRepository : IRepository<UserEntity,int>
    {
        Task DeleteAll();
    }
}