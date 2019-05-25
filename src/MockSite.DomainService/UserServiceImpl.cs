using System;
using System.Threading.Tasks;
using Grpc.Core;
using MethodTimer;
using MockSite.Core.DTOs;
using MockSite.Core.Services;
using MockSite.Message;
using UserService = MockSite.Message.UserService;

namespace MockSite.DomainService
{
    public class UserServiceImpl:UserService.UserServiceBase
    {
        private readonly IUserService _coreService;

        public UserServiceImpl(IUserService service)
        {
            _coreService = service;
        }

        [Time]
        public override async Task<ResponseBase> Create(User request, ServerCallContext context)
        {
            var result = new ResponseBase();
            try
            {
                await _coreService.Create(new UserDTO
                {
                    Code = request.Code,
                    DisplayKey = request.DisplayKey,
                    OrderNo = request.OrderNo
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
        public override async Task<ResponseBase> Update(User request, ServerCallContext context)
        {
            var result = new ResponseBase();
            try
            {
                await _coreService.Update(new UserDTO
                {
                    Code = request.Code,
                    DisplayKey = request.DisplayKey,
                    OrderNo = request.OrderNo
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
        public override async Task<ResponseBase> Delete(UserCode request, ServerCallContext context)
        {
            var result = new ResponseBase();
            try
            {
                await _coreService.Delete(new UserDTO{Code = request.Code});
                result.Code = ResponseCode.Success;
            }
            catch (Exception)
            {
                result.Code = ResponseCode.GeneralError;
            }

            return result;
        }

        [Time]
        public override async Task<User> Get(UserCode request, ServerCallContext context)
        {
            try
            {
                var data = await _coreService.GetByCode(request.Code);
                return new User
                {
                    Code = data.Code,
                    DisplayKey = data.DisplayKey,
                    OrderNo = data.OrderNo
                };
               
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
                var result = new Users();
                var datas = await _coreService.GetAll();
                foreach (var data in datas)
                {
                    result.Value.Add(new User
                    {
                        Code = data.Code,
                        DisplayKey = data.DisplayKey,
                        OrderNo = data.OrderNo
                            });
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}