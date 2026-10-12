using project.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace project.ViewModels
{
    public class ProductDetailViewModel : BaseViewModel
    {
        public Product Product { get; }

        public string AvailabilityColor => Product.Quantity > 0 ? "#4CAF50" : "#F44336";

        public ProductDetailViewModel(Product product)
        {
            Product = product;
        }
    }
}
