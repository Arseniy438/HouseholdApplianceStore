using project.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace project.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            _viewModel.CloseAction = success =>
            {
                if (success) this.Close();
            };
            DataContext = _viewModel;

            LoginTextBox.Focus();

            PasswordBox.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                    PerformLogin();
            };

            LoginTextBox.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                    PasswordBox.Focus();
            };
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin();
        }

        private void PerformLogin()
        {
            _viewModel.ExecuteLoginWithPassword(PasswordBox.Password);
        }
    }
}
