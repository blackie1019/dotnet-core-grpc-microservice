#region

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MockSite.Common.Core.Utilities;
using MockSite.Web.Constants;
using MockSite.Web.Models;

#endregion

namespace MockSite.Web.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly List<UserPo> _users = new List<UserPo>
        {
            new UserPo
            {
                Id = 1,
                Username = "admin",
                Password = "test123",
                Policies = new[] {Policy.UserReadonly, Policy.UserModify, Policy.UserDelete}
            }
        };

        public UserVo Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);
            if (user == null) return null;

            var token = new JwtSecurityTokenHandler().WriteToken(GenerateToken(user));
            return new UserVo
            {
                Id = user.Id,
                Name = user.Username,
                Token = token,
                Policies = user.Policies
            };
        }

        private JwtSecurityToken GenerateToken(UserPo user)
        {
            var claims = new List<Claim> {new Claim(ClaimTypes.Name, user.Id.ToString())};
            claims.AddRange(user.Policies.Select(policy => new Claim(ClaimTypes.Role, policy)));

            var settings = AppSettingsHelper.Instance;
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.GetValueFromKey(AppSetting.JwtSecretKey))),
                SecurityAlgorithms.HmacSha256Signature
            );

            return new JwtSecurityToken(
                settings.GetValueFromKey(AppSetting.JwtIssuerKey),
                settings.GetValueFromKey(AppSetting.JwtAudienceKey),
                signingCredentials: signingCredentials,
                expires: DateTime.Now.AddHours(1),
                claims: claims
            );
        }
    }
}