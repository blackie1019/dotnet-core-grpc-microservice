using System;
using System.Threading.Tasks;
using Grpc.Core;
using MockSite.Message;

namespace MockSite.DomainService
{
    public class UserServiceImpl:UserService.UserServiceBase
    {
        private readonly Core.Services.UserService _coreService;

        public UserServiceImpl(Core.Services.UserService service)
        {
            _coreService = service;
        }

        public override async Task<ResponseBase> Create(User request, ServerCallContext context)
        {
            var result = new ResponseBase();
            try
            {
                await _coreService.Create(new Core.Entities.User
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

        public override async Task<ResponseBase> Update(User request, ServerCallContext context)
        {
            var result = new ResponseBase();
            try
            {
                await _coreService.Update(new Core.Entities.User
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

        public override async Task<ResponseBase> Delete(UserCode request, ServerCallContext context)
        {
            var result = new ResponseBase();
            try
            {
                await _coreService.Delete(new Core.Entities.User{Code = request.Code});
                result.Code = ResponseCode.Success;
            }
            catch (Exception)
            {
                result.Code = ResponseCode.GeneralError;
            }

            return result;
        }

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