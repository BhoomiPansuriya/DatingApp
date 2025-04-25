using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController(ILikesRepository likesRepository) : BaseApiController
{
    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        var sourceUserId = User.GetUserId();
        if (sourceUserId == targetUserId)
        {
            return BadRequest("You cannot like yourself");
        }
        var likedUser = await likesRepository.GetUserLike(sourceUserId, targetUserId);
        if (likedUser != null)
        {
            likesRepository.DeleteLike(likedUser);
            return Ok();
        }
        else
        {
            var newLike = new UserLike { SourceUserId = sourceUserId, TargetUserId = targetUserId };
            likesRepository.AddLike(newLike);
            return Ok();
        }

        if (await likesRepository.SaveAllAsync())
        {
            return Ok();
        }
        return BadRequest("Failed to like this user");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LikeDTOs>>> GetUserLikes(
        [FromQuery] LikesParams likesParams
    )
    {
        likesParams.UserId = User.GetUserId();
        var likes = await likesRepository.GetUserLikes(likesParams);
        Response.AddPaginationHeader(likes);
        return Ok(likes);
    }
}
