namespace API.Controllers;

using API.Data;
using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class UsersController(IUserRepository repository, IMapper mapper, IPhotoService photoService)
    : BaseApiController
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTOs>>> GetUsers(
        [FromQuery] UserParams userParams
    )
    {
        userParams.CurrentUserName = User.GetUserName();
        var user = await repository.GetMembersAsync(userParams);
        Response.AddPaginationHeader(user);
        return Ok(user);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDTOs>> GetUser(string username)
    {
        var user = await repository.GetMemberAsync(username);
        if (user == null)
            return NotFound();
        return user;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTOs memberDTOs)
    {
        //var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // if (username == null) return BadRequest("No username found in token");

        var user = await repository.GetUserByNameAsync(User.GetUserName());
        if (user == null)
            return BadRequest("Could not find user");
        mapper.Map(memberDTOs, user);

        if (await repository.SaveAllAsync())
            return NoContent();
        return BadRequest("user is not updated");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDTOs>> AddPhoto(IFormFile file)
    {
        var user = await repository.GetUserByNameAsync(User.GetUserName());
        if (user == null)
            return BadRequest("User Not Found");
        var result = await photoService.AddPhotoAsync(file);
        if (result.Error != null)
            return BadRequest(result.Error.Message);

        var photo = new Photo { Url = result.SecureUrl.AbsoluteUri, PublicId = result.PublicId };
        user.Photos.Add(photo);
        if (await repository.SaveAllAsync())
            return CreatedAtAction(
                nameof(GetUser),
                new { username = user.UserName },
                mapper.Map<PhotoDTOs>(photo)
            );
        mapper.Map<PhotoDTOs>(photo);
        return BadRequest("problem addding photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await repository.GetUserByNameAsync(User.GetUserName());
        if (user == null)
            return BadRequest("Colud not find user");
        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
        if (photo == null || photo.IsMain)
            return BadRequest("Can not use this as main photo");
        var cureentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        if (cureentMain != null)
            cureentMain.IsMain = false;
        photo.IsMain = true;
        if (await repository.SaveAllAsync())
            return NoContent();
        return BadRequest("Problem setting main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await repository.GetUserByNameAsync(User.GetUserName());
        if (user == null)
            return BadRequest("User not found");
        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
        if (photo == null || photo.IsMain)
            return BadRequest("This photo cannot be deleted");
        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null)
                return BadRequest(result.Error.Message);
        }
        user.Photos.Remove(photo);
        if (await repository.SaveAllAsync())
            return Ok();
        return BadRequest("Problem on deleting photo");
    }
}
