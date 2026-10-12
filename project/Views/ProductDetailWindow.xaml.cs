using project.Models;
using project.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace project.Views
{
    public partial class ProductDetailWindow : Window
    {
        public ProductDetailWindow(Product product)
        {
            InitializeComponent();
            DataContext = new ProductDetailViewModel(product);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
