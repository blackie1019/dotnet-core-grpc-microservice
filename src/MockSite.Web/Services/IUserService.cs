#region

using MockSite.Web.Models;

#endregion

namespace MockSite.Web.Services
{
    public interface IUserService
    {
        UserVo Authenticate(string username, string password);
    }
}