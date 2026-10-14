using System.Collections.ObjectModel;
using System.Windows;
using project.Models;
using project.Services;
using project.Views;

namespace project.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly AuthService _authService;
    private readonly ProductService _productService = new();
    private readonly OrderService _orderService = new();

    private List<Product> _allProducts = new();

    public string UserDisplayName => _authService.CurrentUser?.DisplayName ?? "Пользователь";
    public string UserRole => _authService.CurrentUser?.Role ?? "";
    public string UserInfo => $"{UserDisplayName} ({UserRole})";

    private ObservableCollection<Product> _products = new();
    public ObservableCollection<Product> Products
    {
        get => _products;
        set => SetProperty(ref _products, value);
    }

    private Product? _selectedProduct;
    public Product? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            SetProperty(ref _selectedProduct, value);
            OnPropertyChanged(nameof(IsProductSelected));
        }
    }

    public bool IsProductSelected => SelectedProduct != null;

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

    public List<string> SortOptions { get; } = new()
    {
        "Без сортировки",
        "По названию (А–Я)",
        "По названию (Я–А)",
        "По цене (по возрастанию)",
        "По цене (по убыванию)",
        "По количеству (по возрастанию)",
        "По количеству (по убыванию)"
    };

    public RelayCommand AddProductCommand { get; }
    public RelayCommand EditProductCommand { get; }
    public RelayCommand DeleteProductCommand { get; }
    public RelayCommand ViewDetailsCommand { get; }
    public RelayCommand ViewOrdersCommand { get; }
    public RelayCommand LogoutCommand { get; }
    public RelayCommand RefreshCommand { get; }

    public Action? CloseAction { get; set; }

    public MainViewModel(AuthService authService)
    {
        _authService = authService;

        AddProductCommand = new RelayCommand(ExecuteAddProduct);
        EditProductCommand = new RelayCommand(ExecuteEditProduct, _ => IsProductSelected);
        DeleteProductCommand = new RelayCommand(ExecuteDeleteProduct, _ => IsProductSelected);
        ViewDetailsCommand = new RelayCommand(ExecuteViewDetails, _ => IsProductSelected);
        ViewOrdersCommand = new RelayCommand(ExecuteViewOrders);
        LogoutCommand = new RelayCommand(ExecuteLogout);
        RefreshCommand = new RelayCommand(_ => LoadProducts());

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

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.ToLower().Trim();
            filtered = filtered.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.Manufacturer.ToLower().Contains(search) ||
                p.Category.ToLower().Contains(search));
        }

        filtered = SelectedSortIndex switch
        {
            1 => filtered.OrderBy(p => p.Name),
            2 => filtered.OrderByDescending(p => p.Name),
            3 => filtered.OrderBy(p => p.Price),
            4 => filtered.OrderByDescending(p => p.Price),
            5 => filtered.OrderBy(p => p.Quantity),
            6 => filtered.OrderByDescending(p => p.Quantity),
            _ => filtered
        };

        Products = new ObservableCollection<Product>(filtered);
    }

    private void ExecuteAddProduct(object? parameter)
    {
        var dialog = new ProductDialogWindow(null, _productService.GetCategories());
        if (dialog.ShowDialog() == true && dialog.ResultProduct != null)
        {
            var result = _productService.AddProduct(dialog.ResultProduct);
            if (result.Success)
            {
                MessageBox.Show("Товар успешно добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadProducts();
            }
            else
            {
                MessageBox.Show(result.ErrorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ExecuteEditProduct(object? parameter)
    {
        if (SelectedProduct == null) return;

        var freshProduct = _productService.GetProductById(SelectedProduct.Id);
        if (freshProduct == null)
        {
            MessageBox.Show("Товар не найден в базе данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var dialog = new ProductDialogWindow(freshProduct, _productService.GetCategories());
        if (dialog.ShowDialog() == true && dialog.ResultProduct != null)
        {
            var result = _productService.UpdateProduct(dialog.ResultProduct);
            if (result.Success)
            {
                MessageBox.Show("Данные товара успешно изменены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadProducts();
            }
            else
            {
                MessageBox.Show(result.ErrorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ExecuteDeleteProduct(object? parameter)
    {
        if (SelectedProduct == null) return;

        var confirm = MessageBox.Show(
            $"Вы действительно хотите удалить выбранный товар?\n\n\"{SelectedProduct.Name}\"",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirm == MessageBoxResult.Yes)
        {
            var result = _productService.DeleteProduct(SelectedProduct.Id);
            if (result.Success)
            {
                MessageBox.Show("Товар успешно удалён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                SelectedProduct = null;
                LoadProducts();
            }
            else
            {
                MessageBox.Show(result.ErrorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ExecuteViewDetails(object? parameter)
    {
        if (SelectedProduct == null) return;

        var freshProduct = _productService.GetProductById(SelectedProduct.Id);
        if (freshProduct == null) return;

        var detailWindow = new ProductDetailWindow(freshProduct);
        detailWindow.ShowDialog();
    }

    private void ExecuteViewOrders(object? parameter)
    {
        var ordersWindow = new OrdersWindow(_orderService, _authService);
        ordersWindow.ShowDialog();
    }

    private void ExecuteLogout(object? parameter)
    {
        _authService.Logout();

        var loginWindow = new LoginWindow();
        loginWindow.Show();

        CloseAction?.Invoke();
    }
}