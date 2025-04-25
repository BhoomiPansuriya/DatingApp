public class LikeDTOs
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string KnownAs { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
}