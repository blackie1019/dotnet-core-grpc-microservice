using System.Collections.Generic;
using System.Threading.Tasks;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;

namespace MockSite.Core.Services
{
    public interface IUserService
    {
        Task Create(UserDTO userDto);
        Task Update(UserDTO userDto);
        Task Delete(UserDTO userDto);
        Task<UserEntity> GetByCode(int code);
        Task<IEnumerable<UserEntity>> GetAll();
    }
}