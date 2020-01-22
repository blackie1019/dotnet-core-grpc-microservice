#region

using System.Threading.Tasks;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;

#endregion

namespace MockSite.Core.Interfaces
{
    public interface IUserService
    {
        Task<int> Create(UserDto userDto);

        Task Update(UserDto userDto);

        Task Delete(int id);

        Task<UserEntity> GetById(int id);

        Task<UserEntity[]> GetAll();

        Task<UserEntity[]> GetByCondition(string code = null,string name = null, string email = null);

        Task<UserEntity> Authenticate(string name, string password);
    }
}