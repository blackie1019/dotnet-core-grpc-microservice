using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Infrastructure.Helpers;
using Microsoft.IdentityModel.Tokens;
using MockSite.Web.Consts;
using MockSite.Web.Models;

namespace MockSite.Web.Services
{
    public interface IAuthorizedService
    {
        LoginUser Authenticate(string username, string password);
    }

    public class AuthorizedService : IAuthorizedService
    {

        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<LoginUser> _users = new List<LoginUser>
        {
            new LoginUser {Username = "test", Password = "test"}
        };

        public LoginUser Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token

            var symmetricSecurityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(AppSettingsHelper.Instance.GetValueFromKey(JwtSettingConsts.SecretKey)));

            var signingCredentials =
                new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Username));

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
    }
}