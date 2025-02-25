public class RequestedItem
{
    public int Id { get; set; }
    public int TradeRequestId { get; set; }
    public TradeRequest TradeRequest { get; set; }

    public int ItemId { get; set; }
    public Item Item { get; set; }
}
