#region

using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Common.Core.Enums;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;
using MockSite.Core.Factories;
using MockSite.Core.Interfaces;

#endregion

namespace MockSite.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public UserService(
            IConfiguration config,
            UserRepositoryFactory userRepositoryFactory
        )
        {
            var useDb = (ConnectDb) int.Parse(config[DbConnectionConst.UserUseDbKey]);
            _userRepository = userRepositoryFactory.Produce(useDb);
            _mapper = new MapperConfiguration(cfg => { cfg.CreateMap<UserDto, UserEntity>(); }).CreateMapper();
        }

        public Task<int> Create(UserDto user)
        {
            var userEntity = _mapper.Map<UserEntity>(user);

            return _userRepository.Create(userEntity);
        }

        public Task Update(UserDto user)
        {
            var userEntity = _mapper.Map<UserEntity>(user);

            return _userRepository.Update(userEntity);
        }

        public Task Delete(int id)
        {
            return _userRepository.Delete(id);
        }

        public Task<UserEntity> GetById(int id)
        {
            return _userRepository.GetById(id);
        }

        public Task<UserEntity[]> GetAll()
        {
            return _userRepository.GetAll();
        }

        public Task<UserEntity[]> GetByCondition(string code = null, string name = null,
            string email = null)
        {
            return _userRepository.GetByCondition(code, name, email);
        }

        public async Task<UserEntity> Authenticate(string name, string password)
        {
            var userEntities = await _userRepository.GetByCondition(name: name);
            var userEntity =
                userEntities.FirstOrDefault(x => x.Password.Equals(password));

            return userEntity;
        }
    }
}