using DreamScape.Data;
using System.Linq;

namespace DreamScape.Services
{
    public static class AuthService
    {
        public static int? CurrentUserId { get; private set; }

        public static User Login(string username, string password)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
                if (user != null)
                {
                    CurrentUserId = user.Id;
                    return user;
                }
            }
            return null;
        }

        public static void Logout()
        {
            CurrentUserId = null;
        }
    }
}
