using Microsoft.Win32;
using project.Models;
using project.Tools;
using System.IO;
using System.Windows;

namespace project.ViewModels;

public class ProductDialogViewModel : BaseViewModel
{
    public bool IsEditing { get; }
    public string WindowTitle => IsEditing ? "Редактирование товара" : "Добавление товара";
    public string SaveButtonText => IsEditing ? "Сохранить" : "Добавить";

    private int _productId;

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _selectedCategory = string.Empty;
    public string SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    private string _manufacturer = string.Empty;
    public string Manufacturer
    {
        get => _manufacturer;
        set => SetProperty(ref _manufacturer, value);
    }

    private string _priceText = string.Empty;
    public string PriceText
    {
        get => _priceText;
        set => SetProperty(ref _priceText, value);
    }

    private string _quantityText = string.Empty;
    public string QuantityText
    {
        get => _quantityText;
        set => SetProperty(ref _quantityText, value);
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private string _specifications = string.Empty;
    public string Specifications
    {
        get => _specifications;
        set => SetProperty(ref _specifications, value);
    }

    // === ИЗОБРАЖЕНИЕ ===
    private string _imagePath = string.Empty;
    public string ImagePath
    {
        get => _imagePath;
        set
        {
            SetProperty(ref _imagePath, value);
            OnPropertyChanged(nameof(FullImagePath));
            OnPropertyChanged(nameof(HasImage));
        }
    }

    public string FullImagePath
    {
        get
        {
            if (string.IsNullOrWhiteSpace(ImagePath))
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "default.png");

            var full = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ImagePath);
            return File.Exists(full) ? full
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "default.png");
        }
    }

    public bool HasImage => !string.IsNullOrWhiteSpace(ImagePath);

    public List<string> Categories { get; }

    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand ChooseImageCommand { get; }
    public RelayCommand RemoveImageCommand { get; }

    public Product? ResultProduct { get; private set; }
    public Action<bool>? CloseAction { get; set; }

    public ProductDialogViewModel(Product? product, List<string> categories)
    {
        Categories = categories;
        IsEditing = product != null;

        if (product != null)
        {
            _productId = product.Id;
            Name = product.Name;
            SelectedCategory = product.Category;
            Manufacturer = product.Manufacturer;
            PriceText = product.Price.ToString("F2");
            QuantityText = product.Quantity.ToString();
            Description = product.Description;
            Specifications = product.Specifications;
            ImagePath = product.ImagePath;
        }

        SaveCommand = new RelayCommand(ExecuteSave);
        CancelCommand = new RelayCommand(() => CloseAction?.Invoke(false));
        ChooseImageCommand = new RelayCommand(ExecuteChooseImage);
        RemoveImageCommand = new RelayCommand(ExecuteRemoveImage);
    }

    private void ExecuteChooseImage()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Выберите изображение товара",
            Filter = ImageHelper.ImageFilter,
            Multiselect = false
        };

        if (dialog.ShowDialog() == true)
        {
            if (!ImageHelper.IsImageFile(dialog.FileName))
            {
                MessageBox.Show("Выбранный файл не является изображением.\nДопустимые форматы: JPG, PNG, BMP, GIF, WEBP",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // При редактировании удаляем старое изображение
            if (IsEditing && !string.IsNullOrWhiteSpace(ImagePath))
                ImageHelper.DeleteImage(ImagePath);

            // Для нового товара используем временный ID (будет заменён после сохранения)
            var tempId = IsEditing ? _productId : new Random().Next(10000, 99999);
            ImagePath = ImageHelper.SaveImage(dialog.FileName, tempId);
        }
    }

    private void ExecuteRemoveImage()
    {
        if (!string.IsNullOrWhiteSpace(ImagePath))
        {
            ImageHelper.DeleteImage(ImagePath);
            ImagePath = string.Empty;
        }
    }

    private void ExecuteSave()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            MessageBox.Show("Название товара обязательно для заполнения", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedCategory))
        {
            MessageBox.Show("Выберите категорию товара", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(Manufacturer))
        {
            MessageBox.Show("Производитель товара обязателен для заполнения", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(PriceText.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal price) || price <= 0)
        {
            MessageBox.Show("Цена должна быть положительным числом", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(QuantityText, out int quantity) || quantity < 0)
        {
            MessageBox.Show("Количество должно быть целым числом не меньше 0", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        ResultProduct = new Product
        {
            Id = _productId,
            Name = Name.Trim(),
            Category = SelectedCategory,
            Manufacturer = Manufacturer.Trim(),
            Price = price,
            Quantity = quantity,
            Description = Description?.Trim() ?? string.Empty,
            Specifications = Specifications?.Trim() ?? string.Empty,
            ImagePath = ImagePath?.Trim() ?? string.Empty
        };

        CloseAction?.Invoke(true);
    }
}