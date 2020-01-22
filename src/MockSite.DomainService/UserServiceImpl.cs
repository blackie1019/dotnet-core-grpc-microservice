#region

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
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
        private readonly IMapper _mapper;

        public UserServiceImpl(IUserService service)
        {
            _coreService = service;
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CreateUserMessage, UserDto>()
                        .ForCtorParam("password", opt => opt.MapFrom(src => EncodePassword(src.Password)));
                    cfg.CreateMap<CreateUserMessage, User>();
                    cfg.CreateMap<UserEntity, User>();
                })
                .CreateMapper();
        }

        public override async Task<User> Create(CreateUserMessage message, ServerCallContext context)
        {
            int resultId;
            var userDto = _mapper.Map<UserDto>(message);
            try
            {
                resultId = await _coreService.Create(userDto);
            }
            catch (Exception)
            {
                return null;
            }

            var result = _mapper.Map<User>(message);
            result.Id = resultId;

            return result;
        }

        public override async Task<BaseResponse> Update(UpdateUserMessage message, ServerCallContext context)
        {
            var result = new BaseResponse();
            try
            {
                await _coreService.Update(new UserDto
                (
                    message.Id,
                    message.Name,
                    message.Email
                ));
                result.Code = ResponseCode.Success;
            }
            catch (Exception)
            {
                result.Code = ResponseCode.GeneralError;
            }

            return result;
        }

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


        public override async Task<AuthenticateResponse> Authenticate(AuthenticateMessage request,
            ServerCallContext context)
        {
            try
            {
                var userEntity = await _coreService.Authenticate(request.Name, EncodePassword(request.Password));

                if (userEntity != null)
                {
                    return new AuthenticateResponse
                    {
                        Code = ResponseCode.Success,
                        Data = _mapper.Map<User>(userEntity)
                    };
                }

                return new AuthenticateResponse
                {
                    Code = ResponseCode.GeneralError,
                    Message = "Invalid username or password."
                };
            }
            catch (Exception e)
            {
                return new AuthenticateResponse
                {
                    Code = ResponseCode.GeneralError,
                    Message = e.Message
                };
            }
        }

        public override async Task<UserResponse> Get(QueryUserMessage message, ServerCallContext context)
        {
            User user = null;
            try
            {
                var entity = await _coreService.GetById(message.Id);
                if(entity != null)
                {
                    user = _mapper.Map<User>(entity);
                }

                var userResponse = new UserResponse
                {
                    Code = user == null ? ResponseCode.NotFound : ResponseCode.Success,
                    Data = user
                };

                return userResponse;
            }
            catch (Exception ex)
            {
                return new UserResponse
                {
                    Code = ResponseCode.GeneralError,
                    Message = ex.Message
                };
            }
        }

        public override async Task<UsersResponse> GetAll(QueryUsersMessage request, ServerCallContext context)
        {
            try
            {
                var entities = (await _coreService.GetByCondition(request.Code, request.Name, request.Email)).ToArray();
                var usersResponse = new UsersResponse
                {
                    Code = ResponseCode.Success
                };
                usersResponse.Data.AddRange(entities.Select(u => _mapper.Map<User>(u)));

                return usersResponse;
            }
            catch (Exception ex)
            {
                return new UsersResponse
                {
                    Code = ResponseCode.GeneralError,
                    Message = ex.Message
                };
            }
        }

        private string EncodePassword(string rawPassword)
        {
            using (var cryptoMd5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(rawPassword);
                var hash = cryptoMd5.ComputeHash(bytes);
                var md5 = BitConverter.ToString(hash)
                    .Replace("-", string.Empty)
                    .ToUpper();

                return md5;
            }
        }
    }
}