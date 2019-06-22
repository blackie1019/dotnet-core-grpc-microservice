#region

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using MethodTimer;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;
using MockSite.Core.Interfaces;
using MockSite.Message;

#endregion

namespace MockSite.DomainService
{
    public class UserServiceImpl : UserService.UserServiceBase
    {
        private readonly IUserService _coreService;

        public UserServiceImpl(IUserService service)
        {
            _coreService = service;
        }

        [Time]
        public override async Task<BaseResponse> Create(CreateUserMessage message, ServerCallContext context)
        {
            var result = new BaseResponse();
            try
            {
                await _coreService.Create(new UserDto
                {
                    Code = message.Code,
                    Name = message.Name,
                    Email = message.Email,
                    Password = EncodePassword(message.Password)
                });
                result.Code = ResponseCode.Success;
            }
            catch (Exception)
            {
                result.Code = ResponseCode.GeneralError;
            }
            return result;
        }

        [Time]
        public override async Task<BaseResponse> Update(UpdateUserMessage message, ServerCallContext context)
        {
            var result = new BaseResponse();
            try
            {
                await _coreService.Update(new UserDto
                {
                    Id = message.Id,
                    Name = message.Name,
                    Email = message.Email
                });
                result.Code = ResponseCode.Success;
            }
            catch (Exception)
            {
                result.Code = ResponseCode.GeneralError;
            }
            return result;
        }

        [Time]
        public override async Task<BaseResponse> Delete(QueryUserMessage message, ServerCallContext context)
        {
            var result = new BaseResponse();
            try
            {
                await _coreService.Delete(message.Id);
                result.Code = ResponseCode.Success;
            }
            catch (Exception)
            {
                result.Code = ResponseCode.GeneralError;
            }
            return result;
        }

        [Time]
        public override async Task<User> Get(QueryUserMessage message, ServerCallContext context)
        {
            try
            {
                var entity = await _coreService.GetById(message.Id);
                return ConvertEntityToMessage(entity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Time]
        public override async Task<Users> GetAll(Empty request, ServerCallContext context)
        {
            try
            {
                var entities = await _coreService.GetAll();
                var result = new Users();
                result.Value.AddRange(entities.Select(ConvertEntityToMessage));
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string EncodePassword(string rawPassword)
        {
            using (var cryptoMd5 = System.Security.Cryptography.MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(rawPassword);
                var hash = cryptoMd5.ComputeHash(bytes);
                var md5 = BitConverter.ToString(hash)
                    .Replace("-", String.Empty)
                    .ToUpper();
                return md5;
            }
        }

        private User ConvertEntityToMessage(UserEntity entity)
        {
            return new User
            {
                Id = entity.Id,
                Code = entity.Code,
                Email = entity.Email,
                Name = entity.Name
            };
        }
    }
}