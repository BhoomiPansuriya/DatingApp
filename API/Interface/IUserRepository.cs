using System;
using API.DTOs;
using API.Entities;
using API.Helpers;
namespace API.Interface;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<bool> SaveAllAsync();
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByNameAsync(string username);

    Task<PagedList<MemberDTOs>> GetMembersAsync(UserParams userParams);
    Task<MemberDTOs?> GetMemberAsync(string username);
    Task<AppUser?> GetUserByPhotoId(int photoId);
}
