using DreamScape.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace DreamScape.Services
{
    public static class ValidationService
    {
        public static bool IsValidPassword(string password)
        {
            return password.Length >= 8 && Regex.IsMatch(password, @"[A-Za-z]") && Regex.IsMatch(password, @"[0-9]");
        }

        public static bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }

        public static bool ArePasswordsMatching(string password, string confirmPassword)
        {
            return password == confirmPassword;
        }

        public static bool IsUsernameTaken(string username, int? userId = null)
        {
            using (var db = new AppDbContext())
            {
                return db.Users.Any(u => u.Username == username && (userId == null || u.Id != userId));
            }
        }
    }
}
