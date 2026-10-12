using project.Models;
using project.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace project.ViewModels
{
    public class OrdersViewModel : BaseViewModel
    {
        private readonly OrderService _orderService;
        private readonly AuthService _authService;

        public ObservableCollection<Order> Orders { get; set; } = new();

        private Order? _selectedOrder;
        public Order? SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                SetProperty(ref _selectedOrder, value);
                OnPropertyChanged(nameof(OrderItems));
                OnPropertyChanged(nameof(SelectedOrderStatus));
                OnPropertyChanged(nameof(IsOrderSelected));
            }
        }

        public ObservableCollection<OrderItem> OrderItems =>
            SelectedOrder != null
                ? new ObservableCollection<OrderItem>(SelectedOrder.Items)
                : new ObservableCollection<OrderItem>();

        public bool IsManager => _authService.IsManager;
        public bool IsOrderSelected => SelectedOrder != null;

        public string? SelectedOrderStatus
        {
            get => SelectedOrder?.Status;
            set
            {
                if (SelectedOrder != null && value != null && SelectedOrder.Status != value)
                {
                    var result = _orderService.UpdateOrderStatus(SelectedOrder.Id, value);
                    if (result.Success)
                    {
                        SelectedOrder.Status = value;
                        OnPropertyChanged(nameof(SelectedOrderStatus));
                        MessageBox.Show($"Статус заказа №{SelectedOrder.Id} изменён на \"{value}\"",
                            "Статус обновлён", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadOrders();
                    }
                    else
                    {
                        MessageBox.Show(result.ErrorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public List<string> Statuses { get; }

        public Visibility ManagerStatusVisibility => IsManager ? Visibility.Visible : Visibility.Collapsed;

        public OrdersViewModel(OrderService orderService, AuthService authService)
        {
            _orderService = orderService;
            _authService = authService;
            Statuses = _orderService.GetStatuses();
            LoadOrders();
        }

        private void LoadOrders()
        {
            List<Order> orders;
            if (_authService.IsManager)
            {
                orders = _orderService.GetAllOrders();
            }
            else
            {
                orders = _orderService.GetOrdersByUser(_authService.CurrentUser!.Id);
            }

            Orders = new ObservableCollection<Order>(orders);
            OnPropertyChanged(nameof(Orders));
        }
    }
}
