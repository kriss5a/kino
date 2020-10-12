using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using kino;
using kino.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

    public class JwtTokenGenerator
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<User> userManager;

        public JwtTokenGenerator(IConfiguration configuration, UserManager<User> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
        }

        public TokenViewModel GenerateToken(User user)
        {
            string issuer = "http://localhost:3000";
            string audience = "http://localhost:3000";
            string jwtKey = "dcf205a7-6207-48bf-8489-62b8a77ec00a";
            string expireTimeInDays = "3";

            var roles = string.Join(";", userManager.GetRolesAsync(user).Result);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, roles)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key,
                SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(double.Parse(expireTimeInDays));

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new TokenViewModel
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                AccessTokenExpiration = expires
            };
        }
    }
