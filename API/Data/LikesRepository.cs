using API.Data;
using API.DTOs;
using API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API;

public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
{
    public void AddLike(UserLike like)
    {
        context.Likes.Add(like);
    }

    public void DeleteLike(UserLike like)
    {
        context.Likes.Remove(like);
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await context
            .Likes.Where(x => x.SourceUserId == currentUserId)
            .Select(x => x.TargetUserId)
            .ToListAsync();
    }

    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PagedList<MemberDTOs>> GetUserLikes(LikesParams likesParams)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MemberDTOs> query;

        switch (likesParams.Predicate)
        {
            case "liked":
                query = likes
                    .Where(x => x.SourceUserId == likesParams.UserId)
                    .Select(x => x.TargetUser)
                    .ProjectTo<MemberDTOs>(mapper.ConfigurationProvider);
                break;
            case "likedBy":
                query = likes
                    .Where(x => x.TargetUserId == likesParams.UserId)
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDTOs>(mapper.ConfigurationProvider);
                break;
            default:
                var likeIds = await GetCurrentUserLikeIds(likesParams.UserId);

                query = likes
                    .Where(x =>
                        x.TargetUserId == likesParams.UserId && likeIds.Contains(x.SourceUserId)
                    )
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDTOs>(mapper.ConfigurationProvider);
                break;
        }

        return await PagedList<MemberDTOs>.CreateAsync(
            query,
            likesParams.PageNumber,
            likesParams.PageSize
        );
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
