using API.DTOs;
using API.Helpers;

namespace API;

public interface ILikesRepository
{   Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId);
    Task<PagedList<MemberDTOs>> GetUserLikes(LikesParams likesParams);
    Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId);
    void DeleteLike(UserLike like);
    void AddLike(UserLike like);
    Task<bool> SaveAllAsync(); // This method is used to save changes to the database context.
}
