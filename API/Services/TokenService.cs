using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interface;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;
public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"]?? throw new Exception("Cannot access tokenkey from appsettings");
        if(tokenKey.Length < 64) throw new Exception("Your tokenkey needs to be longer");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var claims  = new List<Claim>{
            new Claim(ClaimTypes.NameIdentifier, user.UserName)
        };
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = cred
        };
        var handler  = new JwtSecurityTokenHandler();
        var token  = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
}
