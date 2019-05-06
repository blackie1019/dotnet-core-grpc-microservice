using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MockSite.Common.Core.Utilities;
using MockSite.Web.Consts;
using MockSite.Web.Models;

namespace MockSite.Web.Services
{
    public interface IUserService
    {
        LoginUser Authenticate(string username, string password);
        IEnumerable<LoginUser> GetAll();
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<LoginUser> _users = new List<LoginUser>
        {
            new LoginUser {Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test"}
        };

        public LoginUser Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettingsHelper.Instance.GetValueFromKey(JwtSettingConsts.SecretKey)));
            
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
            
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Id.ToString()));
            
            var token = new JwtSecurityToken(
                issuer: AppSettingsHelper.Instance.GetValueFromKey(JwtSettingConsts.IssuerKey),
                audience: AppSettingsHelper.Instance.GetValueFromKey(JwtSettingConsts.AudienceKey),
                signingCredentials: signingCredentials, 
                expires: DateTime.Now.AddHours(1),
                claims: claims
            );
            
            user.Token = new JwtSecurityTokenHandler().WriteToken(token);

            // remove password before returning
            user.Password = null;

            return user;
        }

        public IEnumerable<LoginUser> GetAll()
        {
            // return users without passwords
            return _users.Select(x =>
            {
                x.Password = null;
                return x;
            });
        }
    }
}