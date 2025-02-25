using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace DreamScape.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<UserItem> UserItems { get; set; }
        public DbSet<TradeRequest> TradeRequests { get; set; }
        public DbSet<RequestedItem> RequestedItems { get; set; }
        public DbSet<GivenItem> GivenItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server = localhost; user = root; password =; database = DreamScape",
                ServerVersion.Parse("8.0.30")
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserItem>()
                .HasKey(ui => new { ui.UserId, ui.ItemId });

            modelBuilder.Entity<UserItem>()
                .HasOne(ui => ui.User)
                .WithMany(u => u.UserItems)
                .HasForeignKey(ui => ui.UserId);

            modelBuilder.Entity<UserItem>()
                .HasOne(ui => ui.Item)
                .WithMany(i => i.UserItems)
                .HasForeignKey(ui => ui.ItemId);

            modelBuilder.Entity<TradeRequest>()
                .HasOne(tr => tr.AskedUser)
                .WithMany(u => u.TradeRequests)
                .HasForeignKey(tr => tr.AskedUserId);

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "ShadowSlayer", Password = "Test123!", Role = "Speler", Email = "shadow@example.com" },
                new User { Id = 2, Username = "MysticMage", Password = "Mage2024", Role = "Speler", Email = "mystic@example.com" },
                new User { Id = 3, Username = "DragonKnight", Password = "Dragon!99", Role = "Speler", Email = "dragon@example.com" },
                new User { Id = 4, Username = "AdminMaster", Password = "Admin007", Role = "Beheerder", Email = "admin@example.com" },
                new User { Id = 5, Username = "ThunderRogue", Password = "Thund3r!!", Role = "Speler", Email = "thunder@example.com" },
                new User { Id = 6, Username = "J", Password = "J", Role = "Speler", Email = "J@example.com" },
                new User { Id = 7, Username = "P", Password = "p", Role = "Speler", Email = "P@example.com" }
            );
            
            modelBuilder.Entity<Item>().HasData(
                new Item { Id = 101, Name = "Zwaard des Vuur", Description = "Een mythisch zwaard met een vlammende gloed.", Type = "Wapen", Rarity = "Legendarisch", Power = 90, Speed = 60, Durability = 80, MagicalProperties = "+30% vuurschade" },
                new Item { Id = 102, Name = "IJs Amulet", Description = "Een amulet dat de drager beschermt tegen kou.", Type = "Accessoire", Rarity = "Episch", Power = 20, Speed = 10, Durability = 70, MagicalProperties = "+25% weerstand tegen ijsaanvallen" },
                new Item { Id = 103, Name = "Schaduw Mantel", Description = "Een donkere mantel die je bewegingen verbergt.", Type = "Armor", Rarity = "Zeldzaam", Power = 40, Speed = 85, Durability = 50, MagicalProperties = "+15% kans om aanvallen te ontwijken" },
                new Item { Id = 104, Name = "Hamer der Titanen", Description = "Een massieve hamer met de kracht van de aarde.", Type = "Wapen", Rarity = "Legendarisch", Power = 95, Speed = 40, Durability = 90, MagicalProperties = "Kan vijanden 3 sec verdoven" },
                new Item { Id = 105, Name = "Lichtboog", Description = "Een boog die pijlen van pure energie afvuurt.", Type = "Wapen", Rarity = "Episch", Power = 85, Speed = 75, Durability = 60, MagicalProperties = "+10% kans op kritieke schade" },
                new Item { Id = 106, Name = "Helende Ring", Description = "Een ring die de gezondheid van de drager herstelt.", Type = "Accessoire", Rarity = "Zeldzaam", Power = 10, Speed = 5, Durability = 100, MagicalProperties = "+5 HP per seconde" },
                new Item { Id = 107, Name = "Demonen Harnas", Description = "Een verdoemd harnas met duistere krachten.", Type = "Armor", Rarity = "Legendarisch", Power = 75, Speed = 50, Durability = 95, MagicalProperties = "Absorbeert 20% van ontvangen schade" }
            );
            modelBuilder.Entity<UserItem>().HasData(
                new UserItem { Id = 1, UserId = 6, ItemId = 101},
                new UserItem { Id = 2, UserId = 6, ItemId = 102 },
                new UserItem { Id = 3, UserId = 6, ItemId = 103 },
                new UserItem { Id = 4, UserId = 6, ItemId = 104 },
                new UserItem { Id = 5, UserId = 6, ItemId = 105 },
                new UserItem { Id = 6, UserId = 6, ItemId = 106 },
                new UserItem { Id = 7, UserId = 6, ItemId = 107 },
                new UserItem { Id = 8, UserId = 7, ItemId = 101 }
            );
        }
    }
}