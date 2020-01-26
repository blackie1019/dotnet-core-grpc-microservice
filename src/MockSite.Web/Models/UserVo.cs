#region

using System.Collections.Generic;

#endregion

namespace MockSite.Web.Models
{
    public class UserVo
    {
        public int Id { get; }

        public string Name { get; }

        public string Token { get; }

        public IEnumerable<string> Policies { get; }

        public UserVo(int id, string name, string token, IEnumerable<string> policies)
        {
            Id = id;
            Name = name;
            Token = token;
            Policies = policies;
        }
    }
}