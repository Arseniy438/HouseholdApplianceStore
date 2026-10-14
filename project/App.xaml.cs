using System.IO;
using System.Windows;
using project.Tools;
using project.Views;

namespace project;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            using var context = new AppDbContext();
            context.Database.EnsureCreated();
            DbInitializer.Initialize(context);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Ошибка инициализации базы данных:\n\n{ex.Message}",
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
            return;
        }

        var loginWindow = new LoginWindow();
        loginWindow.Show();
    }
}