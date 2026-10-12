using project.Data;
using project.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace project.Services
{
    public class AuthService
    {
        public User? CurrentUser { get; private set; }

        public (bool Success, string ErrorMessage) Login(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
                return (false, "Логин не может быть пустым");

            if (string.IsNullOrWhiteSpace(password))
                return (false, "Пароль не может быть пустым");

            var validation = PasswordHelper.ValidatePassword(password);
            if (!validation.IsValid)
                return (false, validation.ErrorMessage);

            using var context = new AppDbContext();
            var user = context.Users.FirstOrDefault(u => u.Login == login);

            if (user == null)
                return (false, "Неверный логин или пароль");

            if (!PasswordHelper.VerifyPassword(password, user.PasswordHash))
                return (false, "Неверный логин или пароль");

            CurrentUser = user;
            return (true, string.Empty);
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public bool IsManager => CurrentUser?.Role == "Менеджер";
        public bool IsClient => CurrentUser?.Role == "Клиент";
    }
}
