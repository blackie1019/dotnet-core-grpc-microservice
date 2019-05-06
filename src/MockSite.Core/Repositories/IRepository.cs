using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;

namespace MockSite.Core.Repositories
{
    public interface IRepository
    {
        Task Create(UserDTO userDTO);
        Task Update(UserDTO userDTO);
        Task Delete(UserDTO userDTO);
        Task<IEnumerable<UserEntity>> GetAll();
        Task<UserEntity> GetByCode(object obj);
    }
}