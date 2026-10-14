using System.Collections.ObjectModel;
using System.Windows;
using project.Models;
using project.Services;
using project.Views;

namespace project.ViewModels;

public class ShopViewModel : BaseViewModel
{
    private readonly AuthService _authService;
    private readonly ProductService _productService = new();
    private readonly OrderService _orderService = new();
    private readonly CartService _cartService = new();

    private List<Product> _allProducts = new();

    public string UserDisplayName => _authService.CurrentUser?.DisplayName ?? "Гость";
    public bool IsGuest => _authService.IsGuest;
    public bool IsClient => _authService.IsClient;

    private ObservableCollection<Product> _products = new();
    public ObservableCollection<Product> Products
    {
        get => _products;
        set => SetProperty(ref _products, value);
    }

    private ObservableCollection<CartItem> _cartItems = new();
    public ObservableCollection<CartItem> CartItems
    {
        get => _cartItems;
        set => SetProperty(ref _cartItems, value);
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                ApplyFilterAndSort();
        }
    }

    private int _selectedSortIndex;
    public int SelectedSortIndex
    {
        get => _selectedSortIndex;
        set
        {
            if (SetProperty(ref _selectedSortIndex, value))
                ApplyFilterAndSort();
        }
    }

    private string _selectedCategory = "Все категории";
    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
                ApplyFilterAndSort();
        }
    }

    public List<string> Categories { get; }

    public List<string> SortOptions { get; } = new()
    {
        "Рекомендуемые",
        "По названию (А–Я)",
        "По названию (Я–А)",
        "По цене (дешевле)",
        "По цене (дороже)"
    };

    public int CartCount => _cartService.ItemCount;
    public string CartTotal => _cartService.TotalPriceFormatted;
    public bool HasCartItems => _cartService.ItemCount > 0;

    public RelayCommand AddToCartCommand { get; }
    public RelayCommand IncreaseCommand { get; }
    public RelayCommand DecreaseCommand { get; }
    public RelayCommand RemoveFromCartCommand { get; }
    public RelayCommand CheckoutCommand { get; }
    public RelayCommand ClearCartCommand { get; }
    public RelayCommand ViewDetailsCommand { get; }
    public RelayCommand ViewOrdersCommand { get; }
    public RelayCommand LogoutCommand { get; }

    public Action? CloseAction { get; set; }

    public ShopViewModel(AuthService authService)
    {
        _authService = authService;

        Categories = new List<string> { "Все категории" };
        Categories.AddRange(_productService.GetCategories());

        AddToCartCommand = new RelayCommand(ExecuteAddToCart);
        IncreaseCommand = new RelayCommand(ExecuteIncrease);
        DecreaseCommand = new RelayCommand(ExecuteDecrease);
        RemoveFromCartCommand = new RelayCommand(ExecuteRemoveFromCart);
        CheckoutCommand = new RelayCommand(ExecuteCheckout, _ => HasCartItems);
        ClearCartCommand = new RelayCommand(ExecuteClearCart, _ => HasCartItems);
        ViewDetailsCommand = new RelayCommand(ExecuteViewDetails);
        ViewOrdersCommand = new RelayCommand(ExecuteViewOrders);
        LogoutCommand = new RelayCommand(ExecuteLogout);

        LoadProducts();
    }

    public void LoadProducts()
    {
        _allProducts = _productService.GetAllProducts();
        ApplyFilterAndSort();
    }

    private void ApplyFilterAndSort()
    {
        var filtered = _allProducts.AsEnumerable();

        if (SelectedCategory != "Все категории" && !string.IsNullOrEmpty(SelectedCategory))
            filtered = filtered.Where(p => p.Category == SelectedCategory);

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.ToLower().Trim();
            filtered = filtered.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.Manufacturer.ToLower().Contains(search));
        }

        filtered = SelectedSortIndex switch
        {
            1 => filtered.OrderBy(p => p.Name),
            2 => filtered.OrderByDescending(p => p.Name),
            3 => filtered.OrderBy(p => p.Price),
            4 => filtered.OrderByDescending(p => p.Price),
            _ => filtered.OrderByDescending(p => p.Quantity > 0).ThenBy(p => p.Name)
        };

        Products = new ObservableCollection<Product>(filtered);
    }

    private void RefreshCart()
    {
        CartItems = new ObservableCollection<CartItem>(_cartService.Items);
        OnPropertyChanged(nameof(CartCount));
        OnPropertyChanged(nameof(CartTotal));
        OnPropertyChanged(nameof(HasCartItems));
    }

    private void ExecuteAddToCart(object? parameter)
    {
        if (IsGuest) return;

        if (parameter is Product product)
        {
            if (product.Quantity <= 0)
            {
                MessageBox.Show("Товара нет в наличии", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _cartService.AddProduct(product);
            RefreshCart();
        }
    }

    private void ExecuteIncrease(object? parameter)
    {
        if (parameter is CartItem item)
        {
            _cartService.UpdateQuantity(item.Product.Id, item.Quantity + 1);
            RefreshCart();
        }
    }

    private void ExecuteDecrease(object? parameter)
    {
        if (parameter is CartItem item)
        {
            if (item.Quantity > 1)
                _cartService.UpdateQuantity(item.Product.Id, item.Quantity - 1);
            else
                _cartService.RemoveProduct(item.Product.Id);
            RefreshCart();
        }
    }

    private void ExecuteRemoveFromCart(object? parameter)
    {
        if (parameter is CartItem item)
        {
            _cartService.RemoveProduct(item.Product.Id);
            RefreshCart();
        }
    }

    private void ExecuteCheckout(object? parameter)
    {
        if (!HasCartItems)
        {
            MessageBox.Show("Корзина пуста", "Внимание",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var confirm = MessageBox.Show(
            $"Оформить заказ на сумму {CartTotal}?",
            "Подтверждение заказа",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirm != MessageBoxResult.Yes) return;

        var result = _orderService.CreateOrder(_authService.CurrentUser!.Id, _cartService.Items.ToList());
        if (result.Success)
        {
            MessageBox.Show(
                $"Заказ №{result.OrderId} успешно оформлен!\nСумма: {CartTotal}\n\nСпасибо за покупку!",
                "Заказ оформлен",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            _cartService.Clear();
            RefreshCart();
            LoadProducts();
        }
        else
        {
            MessageBox.Show(result.ErrorMessage, "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExecuteClearCart(object? parameter)
    {
        var confirm = MessageBox.Show("Очистить корзину?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm == MessageBoxResult.Yes)
        {
            _cartService.Clear();
            RefreshCart();
        }
    }

    private void ExecuteViewDetails(object? parameter)
    {
        if (parameter is Product product)
        {
            var detailWindow = new ProductDetailWindow(product);
            detailWindow.ShowDialog();
        }
    }

    private void ExecuteViewOrders(object? parameter)
    {
        var ordersWindow = new OrdersWindow(_orderService, _authService);
        ordersWindow.ShowDialog();
    }

    private void ExecuteLogout(object? parameter)
    {
        _authService.Logout();
        _cartService.Clear();

        var loginWindow = new LoginWindow();
        loginWindow.Show();

        CloseAction?.Invoke();
    }
}