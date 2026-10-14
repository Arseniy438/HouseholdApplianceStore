using System.Windows;
using project.Services;
using project.Views;

namespace project.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly AuthService _authService = new();

    private string _login = string.Empty;
    public string Login
    {
        get => _login;
        set => SetProperty(ref _login, value);
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private bool _hasError;
    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    public RelayCommand LoginCommand { get; }
    public RelayCommand GuestLoginCommand { get; }
    public RelayCommand ExitCommand { get; }

    public Action<bool>? CloseAction { get; set; }

    public LoginViewModel()
    {
        LoginCommand = new RelayCommand(_ => { });
        GuestLoginCommand = new RelayCommand(ExecuteGuestLogin);
        ExitCommand = new RelayCommand(() => Application.Current.Shutdown());
    }

    public void ExecuteLoginWithPassword(string password)
    {
        HasError = false;
        ErrorMessage = string.Empty;

        var result = _authService.Login(Login, password);

        if (result.Success)
        {
            OpenMainWindow();
        }
        else
        {
            ErrorMessage = result.ErrorMessage;
            HasError = true;
        }
    }

    private void ExecuteGuestLogin()
    {
        _authService.LoginAsGuest();
        OpenMainWindow();
    }

    private void OpenMainWindow()
    {
        Window window;

        if (_authService.IsManager)
            window = new MainWindow(_authService);
        else
            window = new ShopWindow(_authService);

        window.Show();
        CloseAction?.Invoke(true);
    }
}