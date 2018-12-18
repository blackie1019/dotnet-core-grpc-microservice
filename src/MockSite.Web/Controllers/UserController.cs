using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Infrastructure;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockSite.Message;
using MockSite.Web.Models;

namespace MockSite.Web.Controllers
{
    [Authorize]
    [Route("api/[Controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService.UserServiceClient _serviceClient;

        public UserController()
        {
            var host = AppSettingsHelper.Instance.GetValueFromKey(MessageConst.TestHostNameKey);
            var port = AppSettingsHelper.Instance.GetValueFromKey(MessageConst.TestPortKey);

            var channel = new Channel($"{host}:{port}", ChannelCredentials.Insecure);
            _serviceClient = new UserService.UserServiceClient(channel);
        }

        [HttpPost("CreateUser")]
        public async Task<ResponseBaseModel<string>> CreateUser([FromBody]User request)
        {   
            var response = new ResponseBaseModel<string>();
            try
            {
                var result = await _serviceClient.CreateAsync(request);
                response.SetCode((Enums.ResponseCode) Convert.ToInt32(result.Code));
            }
            catch (Exception e)
            {
                response.SetErrorMsg(e.Message);
            }
            return response;
        }
        
        [HttpPost("UpdateUser")]
        public async Task<ResponseBaseModel<string>> UpdateUser([FromBody]User request)
        {   
            var response = new ResponseBaseModel<string>();
            try
            {
                var result = await _serviceClient.UpdateAsync(request);
                response.SetCode((Enums.ResponseCode) Convert.ToInt32(result.Code));
            }
            catch (Exception e)
            {
                response.SetErrorMsg(e.Message);
            }
            return response;
        }
        
        [HttpPost("DeleteUser")]
        public async Task<ResponseBaseModel<string>> DeleteUser([FromBody]UserCode request)
        {   
            var response = new ResponseBaseModel<string>();
            try
            {
                var result = await _serviceClient.DeleteAsync(request);
                response.SetCode((Enums.ResponseCode) Convert.ToInt32(result.Code));
            }
            catch (Exception e)
            {
                response.SetErrorMsg(e.Message);
            }
            return response;
        }
        
        [HttpGet("GetUsers/{code}",Name = "User_GetUser")]
        public async Task<ResponseBaseModel<User>> GetUser(int code)
        {   
            var response = new ResponseBaseModel<User>();
            try
            {
                var result = await _serviceClient.GetAsync(new UserCode{Code = code});
                response.SetData(result);
            }
            catch (Exception e)
            {
                response.SetErrorMsg(e.Message);
            }
            return response;
        }
                
        [HttpGet("GetUsers",Name = "User_GetUsers")]
        public async Task<ResponseBaseModel<IEnumerable<User>>> GetUsers()
        {   
            var response = new ResponseBaseModel<IEnumerable<User>>();
            try
            {
                var result = await _serviceClient.GetAllAsync(new Empty());
                response.SetData(result.Value);
            }
            catch (Exception e)
            {
                response.SetErrorMsg(e.Message);
            }
            return response;
        }

    }
}