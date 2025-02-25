using System.Collections.Generic;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string Rarity { get; set; }
    public int Power { get; set; }
    public int Speed { get; set; }
    public int Durability { get; set; }
    public string MagicalProperties { get; set; }

    public ICollection<UserItem> UserItems { get; set; }
}
