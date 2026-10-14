using project.Services;
using project.ViewModels;
using System.IO;
using System.Windows;

namespace project.Views;

public partial class ShopWindow : Window
{
    public ShopWindow(AuthService authService)
    {
        InitializeComponent();
        var vm = new ShopViewModel(authService);
        vm.CloseAction = () => this.Close();
        DataContext = vm;
        // Временная диагностика
        var testDir = Path.Combine(AppContext.BaseDirectory, "Images");
        var files = Directory.Exists(testDir) ? Directory.GetFiles(testDir) : Array.Empty<string>();
        System.Diagnostics.Debug.WriteLine($"=== Images dir: {testDir} ===");
        System.Diagnostics.Debug.WriteLine($"=== Files count: {files.Length} ===");
        foreach (var f in files)
            System.Diagnostics.Debug.WriteLine($"  {f}");
    }
}