using project.Services;
using project.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace project.Views
{
    public partial class OrdersWindow : Window
    {
        public OrdersWindow(OrderService orderService, AuthService authService)
        {
            InitializeComponent();
            DataContext = new OrdersViewModel(orderService, authService);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
