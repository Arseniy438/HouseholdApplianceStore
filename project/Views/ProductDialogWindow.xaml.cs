using project.Models;
using project.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace project.Views
{
    public partial class ProductDialogWindow : Window
    {
        private readonly ProductDialogViewModel _viewModel;

        public Product? ResultProduct => _viewModel.ResultProduct;

        public ProductDialogWindow(Product? product, List<string> categories)
        {
            InitializeComponent();
            _viewModel = new ProductDialogViewModel(product, categories);
            _viewModel.CloseAction = result =>
            {
                DialogResult = result;
                Close();
            };
            DataContext = _viewModel;
        }
    }
}
