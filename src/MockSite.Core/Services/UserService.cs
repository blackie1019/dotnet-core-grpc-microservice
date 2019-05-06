using System.Collections.Generic;
using System.Threading.Tasks;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;
using MockSite.Core.Repositories;

namespace MockSite.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository _userRepository;

        public UserService(IRepository repository)
        {
            _userRepository = repository;
        }
        
        public async Task Create(UserDTO user)
        {
            await _userRepository.Create(user);
        }

        public async Task Update(UserDTO user)
        {
            await _userRepository.Update(user);
        }

        public async Task Delete(UserDTO user)
        {
            await _userRepository.Delete(user);
        }

        public async Task<UserEntity> GetByCode(int code)
        {
            return await _userRepository.GetByCode(code);
        }

        public async Task<IEnumerable<UserEntity>> GetAll()
        {
            return await _userRepository.GetAll();
        }

    }
}