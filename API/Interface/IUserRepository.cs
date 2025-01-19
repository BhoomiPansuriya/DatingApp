using System;
using API.DTOs;
using API.Entities;

namespace API.Interface;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<bool> SaveAllAsync();
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByNameAsync(string username);

    Task<IEnumerable<MemberDTOs>> GetMembersAsync();
    Task<MemberDTOs?> GetMemberAsync(string username);
}
