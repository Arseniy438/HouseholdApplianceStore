using project.Models;
using project.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace project.ViewModels
{
    public class CartViewModel : BaseViewModel
    {
        private readonly CartService _cartService;
        private readonly OrderService _orderService;
        private readonly int _userId;

        public ObservableCollection<CartItem> CartItems { get; set; } = new();

        private CartItem? _selectedItem;
        public CartItem? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        private string _totalPrice = "0,00 ₽";
        public string TotalPrice
        {
            get => _totalPrice;
            set => SetProperty(ref _totalPrice, value);
        }

        public bool HasItems => CartItems.Count > 0;

        public RelayCommand RemoveItemCommand { get; }
        public RelayCommand ClearCartCommand { get; }
        public RelayCommand CheckoutCommand { get; }
        public RelayCommand IncreaseQuantityCommand { get; }
        public RelayCommand DecreaseQuantityCommand { get; }

        public Action? CloseAction { get; set; }

        public CartViewModel(CartService cartService, OrderService orderService, int userId)
        {
            _cartService = cartService;
            _orderService = orderService;
            _userId = userId;

            RemoveItemCommand = new RelayCommand(ExecuteRemoveItem, _ => SelectedItem != null);
            ClearCartCommand = new RelayCommand(ExecuteClearCart, _ => HasItems);
            CheckoutCommand = new RelayCommand(ExecuteCheckout, _ => HasItems);
            IncreaseQuantityCommand = new RelayCommand(ExecuteIncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand(ExecuteDecreaseQuantity);

            RefreshCart();
        }

        public void RefreshCart()
        {
            CartItems = new ObservableCollection<CartItem>(_cartService.Items);
            OnPropertyChanged(nameof(CartItems));
            OnPropertyChanged(nameof(HasItems));
            TotalPrice = _cartService.TotalPriceFormatted;
        }

        private void ExecuteRemoveItem(object? parameter)
        {
            if (SelectedItem == null) return;

            _cartService.RemoveProduct(SelectedItem.Product.Id);
            RefreshCart();
        }

        private void ExecuteClearCart(object? parameter)
        {
            var confirm = MessageBox.Show("Очистить корзину?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm == MessageBoxResult.Yes)
            {
                _cartService.Clear();
                RefreshCart();
            }
        }

        private void ExecuteCheckout(object? parameter)
        {
            var confirm = MessageBox.Show(
                $"Оформить заказ на сумму {_cartService.TotalPriceFormatted}?",
                "Подтверждение заказа",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                var result = _orderService.CreateOrder(_userId, _cartService.Items.ToList());
                if (result.Success)
                {
                    MessageBox.Show(
                        $"Заказ №{result.OrderId} успешно оформлен!\nСумма: {_cartService.TotalPriceFormatted}",
                        "Заказ оформлен",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    _cartService.Clear();
                    RefreshCart();
                    CloseAction?.Invoke();
                }
                else
                {
                    MessageBox.Show(result.ErrorMessage, "Ошибка оформления заказа", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteIncreaseQuantity(object? parameter)
        {
            if (parameter is CartItem item)
            {
                _cartService.UpdateQuantity(item.Product.Id, item.Quantity + 1);
                RefreshCart();
            }
        }

        private void ExecuteDecreaseQuantity(object? parameter)
        {
            if (parameter is CartItem item)
            {
                if (item.Quantity > 1)
                {
                    _cartService.UpdateQuantity(item.Product.Id, item.Quantity - 1);
                }
                else
                {
                    _cartService.RemoveProduct(item.Product.Id);
                }
                RefreshCart();
            }
        }
    }
}
