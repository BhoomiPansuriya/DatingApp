using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
    {
      [HttpPost("register")]
      public async Task<ActionResult<UserDTOs>> Register(RegisterDTOs registerDTOs) {
        if(await UserExist(registerDTOs.Username)) return BadRequest("User is already exist");
        return Ok();
        // using var hmac = new HMACSHA512();
        // var user = new AppUser {
        //     UserName = registerDTOs.Username.ToLower(),
        //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTOs.Password)),
        //     PasswordSalt = hmac.Key
        // };
        // context.Users.Add(user);
        // await context.SaveChangesAsync();
        // return new UserDTOs{
        //   UserName = user.UserName,
        //   Token = tokenService.CreateToken(user)
        // };
      }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTOs>> Login(LoginDTOs loginDTOs) {
        var user  = await context.Users.FirstOrDefaultAsync(u => u.UserName == loginDTOs.Username.ToLower());
        if (user ==null) return Unauthorized("Invalid username or password");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTOs.Password));
        for(int i=0; i<computedHash.Length; i++) {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        }
        return new UserDTOs {
          UserName = loginDTOs.Username,
          Token = tokenService.CreateToken(user)
        };
    }
      private async Task<bool> UserExist(string username) {
        return await context.Users.AnyAsync(x=>x.UserName.ToLower() == username.ToLower());
      }
    }
}
