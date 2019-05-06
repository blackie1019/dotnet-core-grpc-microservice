using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockSite.Web.Models;
using MockSite.Web.Services;

namespace MockSite.Web.Controllers
{
    [Authorize]
    [Route("api/[Controller]")]
    public class AuthorizedController : ControllerBase
    {
        private IUserService _userService;
        
        public AuthorizedController(IUserService userService)
        {
            _userService = userService;
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody]LoginUser userParam)
        {
            var user = _userService.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }
    }
}