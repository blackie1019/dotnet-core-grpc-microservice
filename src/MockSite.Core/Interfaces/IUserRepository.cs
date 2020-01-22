#region

using System.Threading.Tasks;
using MockSite.Core.Entities;

#endregion

namespace MockSite.Core.Interfaces
{
    public interface IUserRepository : IRepository<UserEntity,int>
    {
        Task<UserEntity[]> GetByCondition(string code = null, string name = null, string email = null);
    }
}