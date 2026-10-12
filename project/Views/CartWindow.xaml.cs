using project.Services;
using project.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace project.Views
{
    public partial class CartWindow : Window
    {
        public CartWindow(CartService cartService, OrderService orderService, int userId)
        {
            InitializeComponent();
            var viewModel = new CartViewModel(cartService, orderService, userId);
            viewModel.CloseAction = () => this.Close();
            DataContext = viewModel;
        }
    }
}
