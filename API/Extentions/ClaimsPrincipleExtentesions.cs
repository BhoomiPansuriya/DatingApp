using System.Security.Claims;

namespace API.Extentions;

public static class ClaimsPrincipleExtentions
{
    public static string GetUserName(this ClaimsPrincipal user)
    {
        var username = user.FindFirstValue(ClaimTypes.Name);
        if (username == null)
            throw new Exception("Cannot get username from token");
        return username;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        var id = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
        if (id == null)
            throw new Exception("Cannot get id from token");
        return id;
    }
}
