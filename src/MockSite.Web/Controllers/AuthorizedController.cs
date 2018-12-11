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
        private IAuthorizedService _service;

        public AuthorizedController(IAuthorizedService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginUser userParam)
        {
            var user = _service.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest(new {message = "Username or password is incorrect"});

            return Ok(user);
        }
    }
}