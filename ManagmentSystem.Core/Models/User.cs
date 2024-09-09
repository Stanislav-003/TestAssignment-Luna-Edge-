using ManagmentSystem.Core.Shared;
using System.Text.RegularExpressions;

namespace ManagmentSystem.Core.Models
{
    public class User
    {
        private const string EMAIL_REGEX = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        public const int MAX_USERNAME_LENGTH = 100;
        public const int MAX_EMAIL_LENGTH = 255;
        public const int MIN_PASSWORD_LENGTH = 8;
        public const int MAX_PASSWORD_LENGTH = 128;

        private User(Guid id, string userName, string email, string password, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            UserName = userName;
            Email = email;
            Password= password;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public Guid Id { get; }
        public string UserName { get; } = string.Empty;
        public string Email { get; } = string.Empty;
        public string Password { get; } = string.Empty;
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        public static (User? User, Error? Error) Create(Guid id, string userName, string email, string password)
        {
            if (string.IsNullOrEmpty(userName) || userName.Length > MAX_USERNAME_LENGTH)
            {
                return (null, new Error("InvalidUserName", "UserName can not be empty or longer than 100 symbols."));
            }

            if (string.IsNullOrEmpty(email) || email.Length > MAX_EMAIL_LENGTH || !IsValidEmail(email))
            {
                return (null, new Error("InvalidEmail", "Email is invalid or longer than 255 characters."));
            }

            if (string.IsNullOrEmpty(password) || password.Length < MIN_PASSWORD_LENGTH || password.Length > MAX_PASSWORD_LENGTH)
            {
                return (null, new Error("InvalidPassword", "Password must be between 8 and 128 characters."));
            }

            var user = new User(id, userName, email, password, DateTime.UtcNow, DateTime.UtcNow);

            return (user, Error.None);
        }

        private static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, EMAIL_REGEX);
        }

        public void Update()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
