#region

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockSite.Message;
using MockSite.Web.Constants;
using MockSite.Web.Models;
using ResponseCode = MockSite.Web.Enums.ResponseCode;

#endregion

namespace MockSite.Web.Controllers
{
    [Route("api/[Controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService.UserServiceClient _serviceClient;

        public UserController(UserService.UserServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }

        [Authorize(Roles = Policy.UserReadonly)]
        [HttpGet("GetUsers")]
        public async Task<ResponseBaseModel<IEnumerable<User>>> GetUsers()
        {
            var result = await _serviceClient.GetAllAsync(new Empty());
            return new ResponseBaseModel<IEnumerable<User>>(ResponseCode.Success, result.Value);
        }

        [Authorize(Roles = Policy.UserReadonly)]
        [HttpGet("GetUser/{id}")]
        public async Task<ResponseBaseModel<User>> GetUser(int id)
        {
            var user = await _serviceClient.GetAsync(new QueryUserMessage {Id = id});
            return new ResponseBaseModel<User>(ResponseCode.Success, user);
        }

        [Authorize(Roles = Policy.UserModify)]
        [HttpPost("CreateUser")]
        public async Task<ResponseBaseModel<string>> CreateUser([FromBody] CreateUserMessage request)
        {
            var result = await _serviceClient.CreateAsync(request);
            return new ResponseBaseModel<string>((ResponseCode) result.Code, null);
        }

        [Authorize(Roles = Policy.UserModify)]
        [HttpPost("UpdateUser")]
        public async Task<ResponseBaseModel<string>> UpdateUser([FromBody] UpdateUserMessage request)
        {
            var result = await _serviceClient.UpdateAsync(request);
            return new ResponseBaseModel<string>((ResponseCode) result.Code, null);
        }

        [Authorize(Roles = Policy.UserDelete)]
        [HttpPost("DeleteUser/{id}")]
        public async Task<ResponseBaseModel<string>> DeleteUser(int id)
        {
            var result = await _serviceClient.DeleteAsync(new QueryUserMessage {Id = id});
            return new ResponseBaseModel<string>((ResponseCode) result.Code, null);
        }
    }
}