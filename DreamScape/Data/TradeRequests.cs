using System.Collections.Generic;

public class TradeRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }

    public int AskedUserId { get; set; }
    public User AskedUser { get; set; }

    public ICollection<RequestedItem> RequestedItems { get; set; }
    public ICollection<GivenItem> GivenItems { get; set; }
}
