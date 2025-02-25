using System.Collections.Generic;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public ICollection<UserItem> UserItems { get; set; }
    public ICollection<TradeRequest> TradeRequests { get; set; }
}
