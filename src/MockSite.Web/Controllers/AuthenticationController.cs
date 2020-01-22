#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockSite.Common.Core.Enums;
using MockSite.Common.Core.Models;
using MockSite.Web.Models;
using MockSite.Web.Services;

#endregion

namespace MockSite.Web.Controllers
{
    [Authorize]
    [Route("api/[Controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public ResponseBaseModel<UserVo> Login([FromBody] LoginRequest request)
        {
            var user = _userService.Authenticate(request.Username, request.Password);

            return new ResponseBaseModel<UserVo>(
                user != null ? ResponseCode.Success : ResponseCode.GeneralError,
                user
            );
        }
    }
}