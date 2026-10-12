using project.Services;
using project.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace project.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(AuthService authService)
        {
            InitializeComponent();
            var viewModel = new MainViewModel(authService);
            viewModel.CloseAction = () => this.Close();
            DataContext = viewModel;
        }
    }
}
