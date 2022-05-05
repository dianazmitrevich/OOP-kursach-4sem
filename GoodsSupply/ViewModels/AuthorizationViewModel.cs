using GoodsSupply.Models;
using System.Windows.Input;
using GoodsSupply.Commands;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using GoodsSupply.Views;
using GoodsSupply.Views.UserControls;

namespace GoodsSupply.ViewModels
{
    class AuthorizationViewModel : BaseViewModel
    {
        private readonly GoodsSupplyContext context = new GoodsSupplyContext();

        private string login;
        private string password;
        private string name;
        private string email;
        private object selectedAuthorizationWindow = new LogInContainer();

        public string Login
        {
            get => login;
            set => Set(ref login, value);
        }
        public string Password
        {
            get => password;
            set => Set(ref password, value);
        }
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }
        public string Email
        {
            get => email;
            set => Set(ref email, value);
        }
        public object SelectedAuthorizationWindow
        {
            get => selectedAuthorizationWindow;
            set => Set(ref selectedAuthorizationWindow, value);
        }

        public ICommand LoginCommand { get; }
        private bool CanLoginCommandExecute(object p) => Login?.Length > 0 && Password?.Length > 0;
        private void OnLoginCommandExecuted(object p)
        {
            var window = Application.Current.Windows[0];
            if (context.USERS.FirstOrDefault(u => u.Login == Login && u.Password == Password) != null)
            {
                var MainWindow = new MainWindow();
                MainWindow.Show();
                window.Close();
            }
            else
                MessageBox.Show(Password);
        }

        public ICommand CloseWindowCommand { get; }
        private void OnCloseWindowCommandExecuted(object p) => Application.Current.Shutdown();

        public ICommand SignUpWindowCommand { get; }
        private void OnSignUpWindowCommandExecuted(object p) => SelectedAuthorizationWindow = new SignUpContainer();

        public ICommand LogInWindowCommand { get; }
        private void OnLogInWindowExecuted(object p) => SelectedAuthorizationWindow = new LogInContainer();

        public AuthorizationViewModel()
        {
            LoginCommand = new DelegateCommand(OnLoginCommandExecuted, CanLoginCommandExecute);
            CloseWindowCommand = new DelegateCommand(OnCloseWindowCommandExecuted);
            SignUpWindowCommand = new DelegateCommand(OnSignUpWindowCommandExecuted);
            LogInWindowCommand = new DelegateCommand(OnLogInWindowExecuted);
        }
    }
}
