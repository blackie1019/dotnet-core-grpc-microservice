#region

using System.Collections.Generic;

#endregion

namespace MockSite.Web.Models
{
    public class UserVo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Token { get; set; }

        public IEnumerable<string> Policies { get; set; }
    }
}