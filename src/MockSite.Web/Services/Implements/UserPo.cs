#region

using System.Collections.Generic;

#endregion

namespace MockSite.Web.Services.Implements
{
    public class UserPo
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public IEnumerable<string> Policies { get; set; }
    }
}