using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace StudyMATEUpload.Repository
{
    public class AuthRepository
    {
        public string GetToken(string email, string role, bool isActive, TimeSpan duration, bool hasDuration)
        {
            //get key
            string key = Startup.Configuration[AppConstant.Secret];
            //get symmetric key
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            //get signin credentials
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            //get claims
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Email, email)
            };
            if (isActive)
            {
                claims.Add(new Claim(ClaimTypes.Role, "ActiveUser"));
            }

            //create web token
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: Startup.Configuration[AppConstant.Issuer],
                audience: Startup.Configuration[AppConstant.Audience],
                signingCredentials: signingCredentials,
                claims: claims,
                expires: hasDuration ? DateTime.Now + duration : DateTime.Now.AddDays(7)
                );
            //return a writable token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
