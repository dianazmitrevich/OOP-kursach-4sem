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

        public ICommand LoginCommand { get; }
        private bool CanLoginCommandExecute(object p) => Login?.Length > 0 && Password?.Length > 0;
        private void OnLoginCommandExecuted(object p)
        {
            MessageBox.Show(Password);
        }

        public AuthorizationViewModel()
        {
            LoginCommand = new DelegateCommand(OnLoginCommandExecuted, CanLoginCommandExecute);
        }
    }
}
