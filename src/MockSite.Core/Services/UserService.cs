using System.Collections.Generic;
using System.Threading.Tasks;
using MockSite.Core.Entities;
using MockSite.Core.Repositories;

namespace MockSite.Core.Services
{
    public class UserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> repository)
        {
            _userRepository = repository;
        }
        
        public async Task Create(User user)
        {
            await _userRepository.Create(user);
        }

        public async Task Update(User user)
        {
            await _userRepository.Update(user);
        }

        public async Task Delete(User user)
        {
            await _userRepository.Delete(user);
        }

        public async Task<User> GetByCode(int code)
        {
            return await _userRepository.GetByPk(code);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _userRepository.GetAll();
        }

    }
}