using System.Collections.Generic;
using System.Threading.Tasks;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;

namespace MockSite.Core.Repositories
{
    public interface IRepository
    {
        Task Create(UserDTO userDto);
        Task Update(UserDTO userDto);
        Task Delete(UserDTO userDto);
        Task<IEnumerable<UserEntity>> GetAll();
        Task<UserEntity> GetByCode(object obj);
    }
}