using GoodsSupply.Models;
using System.Windows.Input;
using GoodsSupply.Commands;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using GoodsSupply.Views;
using GoodsSupply.Views.UserControls;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System;
using GoodsSupply.Views.Admin_views;
using System.Collections.ObjectModel;

namespace GoodsSupply.ViewModels
{
    class AuthorizationViewModel : BaseViewModel
    {
        private readonly GoodsSupplyContext context = new GoodsSupplyContext();

        private string login;
        private string password;
        private string passwordCheck;
        private string name;
        private string email;
        private object selectedAuthorizationWindow = new LogInContainer();
        private string authorizationWindowTitle = "Войдите в свой личный кабинет";
        private string failedSignupLabel = "";
        private Visibility failedAuthorizationFlag = Visibility.Hidden;
        private Visibility failedNameFlag = Visibility.Hidden;
        private Visibility failedEmailFlag = Visibility.Hidden;
        private Visibility failedLoginFlag = Visibility.Hidden;
        private Visibility failedPasswordFlag = Visibility.Hidden;
        private Visibility failedPasswordCheckFlag = Visibility.Hidden;

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
        public string PasswordCheck
        {
            get => passwordCheck;
            set => Set(ref passwordCheck, value);
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
        public string AuthorizationWindowTitle
        {
            get => authorizationWindowTitle;
            set => Set(ref authorizationWindowTitle, value);
        }
        public string FailedSignupLabel
        {
            get => failedSignupLabel;
            set => Set(ref failedSignupLabel, value);
        }
        public Visibility FailedAuthorizationFlag
        {
            get => failedAuthorizationFlag;
            set => Set(ref failedAuthorizationFlag, value);
        }
        public Visibility FailedNameFlag
        {
            get => failedNameFlag;
            set => Set(ref failedNameFlag, value);
        }
        public Visibility FailedEmailFlag
        {
            get => failedEmailFlag;
            set => Set(ref failedEmailFlag, value);
        }
        public Visibility FailedLoginFlag
        {
            get => failedLoginFlag;
            set => Set(ref failedLoginFlag, value);
        }
        public Visibility FailedPasswordFlag
        {
            get => failedPasswordFlag;
            set => Set(ref failedPasswordFlag, value);
        }
        public Visibility FailedPasswordCheckFlag
        {
            get => failedPasswordCheckFlag;
            set => Set(ref failedPasswordCheckFlag, value);
        }

        public ICommand LoginCommand { get; }
        private bool CanLoginCommandExecute(object p) => Login?.Length > 0 && Password?.Length > 0;
        private void OnLoginCommandExecuted(object p)
        {
            var window = Application.Current.Windows[0];
            string hashPassword = USERS.getHash(Password);

            var user = context.USERS.FirstOrDefault(u => u.Login == Login && u.Password == hashPassword);
            if (user != null)
            {
                if (user.Login == "admin" && user.IsAdmin == "Y")
                {
                    var MainWindow = new AdminMainWindow();
                    MainWindow.Show();
                    window.Close();
                }
                else
                {
                    var userLinkAccountId = user.LinkAccountId;
                    var account = context.PERSONAL_ACCOUNTS.FirstOrDefault(f => f.AccountId == userLinkAccountId).AccountId;

                    var foundOrder = context.ORDERS.FirstOrDefault(f => f.LinkAccountId.Equals(account));
                    if (foundOrder != null)
                    {
                        if (foundOrder.Adress == null)
                        {
                            var orderedProducts = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId.Equals(foundOrder.OrderId)));

                            foreach (var item in orderedProducts)
                            {
                                if (item != null)
                                {
                                    var productDetail = context.PRODUCTS_DETAIL.FirstOrDefault(f => f.ProductCode.Equals(item.OrderedProductId));
                                    var product = context.PRODUCTS.FirstOrDefault(f => f.ProductId.Equals(productDetail.LinkToProductId));

                                    product.Quantity += item.OrderedQuantity;

                                    context.ORDERED_PRODUCTS.Remove(item);
                                }
                            }
                            context.ORDERS.Remove(foundOrder);
                        }
                    }

                    ORDERS order = new ORDERS(account); context.ORDERS.Add(order); context.SaveChanges();
                    MainWindowViewModel model = new MainWindowViewModel(context.PERSONAL_ACCOUNTS.FirstOrDefault(f => context.USERS.FirstOrDefault(u => u.Login == Login).LinkAccountId == f.AccountId), order);
                    var MainWindow = new MainWindow(model);
                    MainWindow.Show();
                    window.Close();
                }
            }
            else
                FailedAuthorizationFlag = Visibility.Visible;
        }

        public ICommand CloseWindowCommand { get; }
        private void OnCloseWindowCommandExecuted(object p) => Application.Current.Shutdown();

        public ICommand SignUpWindowCommand { get; }
        private void OnSignUpWindowCommandExecuted(object p)
        {
            Login = ""; Password = "";
            SelectedAuthorizationWindow = new SignUpContainer();
            AuthorizationWindowTitle = "Создание нового аккаунта";
        }

        public ICommand LogInWindowCommand { get; }
        private void OnLogInWindowCommandExecuted(object p)
        {
            Login = ""; Password = ""; FailedAuthorizationFlag = Visibility.Hidden;
            SelectedAuthorizationWindow = new LogInContainer();
            AuthorizationWindowTitle = "Войдите в свой личный кабинет";
        }

        private string namePattern = @"^\D*$";
        private string emailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
        private string loginPattern = @"^(?=.*[A-Za-z0-9]$)[A-Za-z][A-Za-z\d.-]{0,19}$";
        private string passwordPattern = @"^\S+$";

        public ICommand SignupCommand { get; }
        private bool CanSignupCommandExecute(object p) => Name?.Length > 0 && Email?.Length > 0 && Login?.Length > 0 && Password?.Length > 0 && PasswordCheck?.Length > 0;
        private void OnSignupCommandExecuted(object p)
        {
            try
            {
                FailedNameFlag = Visibility.Hidden;
                FailedEmailFlag = Visibility.Hidden;
                FailedLoginFlag = Visibility.Hidden;
                FailedPasswordFlag = Visibility.Hidden;
                FailedPasswordCheckFlag = Visibility.Hidden;

                if (Regex.IsMatch(Name, namePattern) && Regex.IsMatch(Name, @"\S+"))
                {
                    if (Regex.IsMatch(Email, emailPattern, RegexOptions.IgnoreCase))
                    {
                        if (context.PERSONAL_ACCOUNTS.FirstOrDefault(u => u.Email == Email) == null)
                        {
                            if (Regex.IsMatch(Login, loginPattern))
                            {
                                if (Login.Length > 2)
                                {
                                    if (Regex.Matches(Login, @"\S*admin\S*", RegexOptions.IgnoreCase).Count == 0)
                                    {
                                        if (context.USERS.FirstOrDefault(u => u.Login == Login) == null)
                                        {
                                            if (Password.Length > 3)
                                            {
                                                if (Regex.IsMatch(Password, passwordPattern))
                                                {
                                                    if (Equals(Password, PasswordCheck))
                                                    {
                                                        PERSONAL_ACCOUNTS element = new PERSONAL_ACCOUNTS(Regex.Replace(Name, @"^\s+|\s+$", ""), Email);
                                                        context.PERSONAL_ACCOUNTS.Add(element); context.SaveChanges();
                                                        int elementId = context.PERSONAL_ACCOUNTS.FirstOrDefault(u => u.Email == Email).AccountId;
                                                        USERS newElement = new USERS(Login, Password, elementId);
                                                        context.USERS.Add(newElement); context.SaveChanges();
                                                        MessageBox.Show("Аккаунт создан, попробуйте войти в него");
                                                        SelectedAuthorizationWindow = new LogInContainer();
                                                    }
                                                    else
                                                    {
                                                        FailedPasswordCheckFlag = Visibility.Visible;
                                                        FailedSignupLabel = "Пароли не совпадают";
                                                    }
                                                }
                                                else
                                                {
                                                    FailedPasswordFlag = Visibility.Visible;
                                                    FailedSignupLabel = "Неверный формат пароля";
                                                }
                                            }
                                            else
                                            {
                                                FailedPasswordFlag = Visibility.Visible;
                                                FailedSignupLabel = "Пароль не может быть короче четырех символов";
                                            }
                                        }
                                        else
                                        {
                                            FailedLoginFlag = Visibility.Visible;
                                            FailedSignupLabel = "Уже существует аккаунт с таким логином";
                                        }
                                    }
                                    else
                                    {
                                        FailedLoginFlag = Visibility.Visible;
                                        FailedSignupLabel = "Аккаунт с таким логином не может быть создан";
                                    }
                                }
                                else
                                {
                                    FailedLoginFlag = Visibility.Visible;
                                    FailedSignupLabel = "Логин не может быть короче трех символов";
                                }
                            }
                            else
                            {
                                FailedLoginFlag = Visibility.Visible;
                                FailedSignupLabel = "Неверный формат логина";
                            }
                        }
                        else
                        {
                            FailedEmailFlag = Visibility.Visible;
                            FailedSignupLabel = "Уже существует аккаунт с этой почтой";
                        }
                    }
                    else
                    {
                        FailedEmailFlag = Visibility.Visible;
                        FailedSignupLabel = "Неверный формат почты";
                    }
                }
                else
                    FailedNameFlag = Visibility.Visible;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        raise = new InvalidOperationException(message, raise);
                    }
                }
            }
        }

        public AuthorizationViewModel()
        {
            LoginCommand = new DelegateCommand(OnLoginCommandExecuted, CanLoginCommandExecute);
            SignupCommand = new DelegateCommand(OnSignupCommandExecuted, CanSignupCommandExecute);
            CloseWindowCommand = new DelegateCommand(OnCloseWindowCommandExecuted);
            SignUpWindowCommand = new DelegateCommand(OnSignUpWindowCommandExecuted);
            LogInWindowCommand = new DelegateCommand(OnLogInWindowCommandExecuted);
        }
    }
}
