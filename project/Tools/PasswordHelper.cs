using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace project.Tools
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password + "ApplianceStoreSalt2024");
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            var computedHash = HashPassword(password);
            return computedHash == hash;
        }

        public static (bool IsValid, string ErrorMessage) ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Пароль не может быть пустым");

            if (password.Length < 6)
                return (false, "Пароль должен содержать минимум 6 символов");

            if (!password.Any(char.IsUpper))
                return (false, "Пароль должен содержать хотя бы одну заглавную букву");

            if (!password.Any(char.IsDigit))
                return (false, "Пароль должен содержать хотя бы одну цифру");

            return (true, string.Empty);
        }
    }
}
