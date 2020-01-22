#region

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MockSite.Message;
using MockSite.Web.Constants;
using MockSite.Web.Models;

#endregion

namespace MockSite.Web.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly Message.UserService.UserServiceClient _serviceClient;

        public UserService(Message.UserService.UserServiceClient serviceClient, IConfiguration configuration)
        {
            _serviceClient = serviceClient;
            _configuration = configuration;
        }

        public UserVo Authenticate(string username, string password)
        {
            var response = _serviceClient.Authenticate(new AuthenticateMessage
            {
                Name = username,
                Password = password
            });

            if (response.Code != ResponseCode.Success) return null;
            var user = response.Data;

            if (user == null) return null;

            var userPo = new UserPo
            {
                Username = user.Name,
                Email = user.Email,
                Id = user.Id,
                Policies = new[]
                {
                    Policy.UserReadonly, Policy.UserModify, Policy.UserDelete, Policy.CommonReadonly,
                    Policy.CommonModify, Policy.CommonDelete
                }
            };

            var token = new JwtSecurityTokenHandler().WriteToken(GenerateToken(userPo));

            return new UserVo
            (
                userPo.Id,
                userPo.Username,
                token,
                userPo.Policies
            );

        }

        private JwtSecurityToken GenerateToken(UserPo user)
        {
            var claims = new List<Claim> {new Claim(ClaimTypes.Name, user.Id.ToString())};
            claims.AddRange(user.Policies.Select(policy => new Claim(ClaimTypes.Role, policy)));

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[AppSetting.JwtSecretKey])),
                SecurityAlgorithms.HmacSha256Signature
            );

            return new JwtSecurityToken(
                _configuration[AppSetting.JwtIssuerKey],
                _configuration[AppSetting.JwtAudienceKey],
                signingCredentials: signingCredentials,
                expires: DateTime.Now.AddHours(1),
                claims: claims
            );
        }
    }
}