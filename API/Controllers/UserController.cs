namespace API.Controllers;

using API.DTOs;
using API.Entities;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[AllowAnonymous]
public class UsersController(IUserRepository repository) : BaseApiController
{

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTOs>>> GetUsers()
        {
                var user = await repository.GetMembersAsync();
                return Ok(user);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTOs>> GetUser(string username)
        {
                var user = await repository.GetMemberAsync(username);
                if (user == null) return NotFound();
                return user;
        }
}