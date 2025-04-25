using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController(
        UserManager<AppUser> userManager,
        ITokenService tokenService,
        IMapper mapper
    ) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDTOs>> Register(RegisterDTOs registerDTOs)
        {
            if (await UserExist(registerDTOs.Username))
                return BadRequest("User is already exist");
            using var hmac = new HMACSHA512();
            var user = mapper.Map<AppUser>(registerDTOs);
            user.UserName = registerDTOs.Username.ToLower();
            var result = await userManager.CreateAsync(user, registerDTOs.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
           // user.Users.Add(user);
            //await context.SaveChangesAsync();
            // return Ok();
            return new UserDTOs
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender,
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTOs>> Login(LoginDTOs loginDTOs)
        {
            var user = await userManager
                .Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(u => u.NormalizedUserName == loginDTOs.Username.ToLower());
            if (user == null)
                return Unauthorized("Invalid username or password");

            // using var hmac = new HMACSHA512(user.PasswordSalt);
            // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTOs.Password));
            // for (int i = 0; i < computedHash.Length; i++)
            // {
            //     if (computedHash[i] != user.PasswordHash[i])
            //         return Unauthorized("Invalid Password");
            // }
            return new UserDTOs
            {
                Username = loginDTOs.Username,
                Token = await tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender,
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            };
        }

        private async Task<bool> UserExist(string username)
        {
            return await userManager.Users.AnyAsync(x =>
                x.NormalizedUserName.ToLower() == username.ToLower()
            );
        }
    }
}
