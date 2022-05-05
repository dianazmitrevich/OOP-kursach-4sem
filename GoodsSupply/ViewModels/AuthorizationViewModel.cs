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
        private string failedEmailLabel = "";
        private string failedLoginLabel = "";
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
        public string FailedEmailLabel
        {
            get => failedEmailLabel;
            set => Set(ref failedEmailLabel, value);
        }
        public string FailedLoginLabel
        {
            get => failedLoginLabel;
            set => Set(ref failedLoginLabel, value);
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
            if (context.USERS.FirstOrDefault(u => u.Login == Login && u.Password == Password) != null)
            {
                var MainWindow = new MainWindow();
                MainWindow.Show();
                window.Close();
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

        private string namePattern = @"^\D+$";
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

                if (Regex.IsMatch(Name, namePattern))
                {
                    if (Regex.IsMatch(Email, emailPattern, RegexOptions.IgnoreCase))
                    {
                        if (context.PERSONAL_ACCOUNTS.FirstOrDefault(u => u.Email == Email) == null)
                        {
                            if (Regex.IsMatch(Login, loginPattern))
                            {
                                if (Regex.Matches(Login, @"\S*admin\S*", RegexOptions.IgnoreCase).Count == 0)
                                {
                                    if (context.USERS.FirstOrDefault(u => u.Login == Login) == null)
                                    {
                                        if (Regex.IsMatch(Password, passwordPattern))
                                        {
                                            if (Equals(Password, PasswordCheck))
                                            {
                                                PERSONAL_ACCOUNTS element = new PERSONAL_ACCOUNTS(Name, Email);
                                                context.PERSONAL_ACCOUNTS.Add(element); context.SaveChanges();
                                                int elementId = context.PERSONAL_ACCOUNTS.FirstOrDefault(u => u.Email == Email).AccountId;
                                                USERS newElement = new USERS(Login, Password, elementId);
                                                context.USERS.Add(newElement); context.SaveChanges();
                                                MessageBox.Show("Аккаунт создан, попробуйте войти в него");
                                            }
                                            else
                                                FailedPasswordCheckFlag = Visibility.Visible;
                                        }
                                        else
                                            FailedPasswordFlag = Visibility.Visible;
                                    }
                                    else
                                    {
                                        FailedLoginFlag = Visibility.Visible;
                                        FailedLoginLabel = "Уже существует аккаунт с таким логином";
                                    }
                                }
                                else
                                {
                                    FailedLoginFlag = Visibility.Visible;
                                    FailedLoginLabel = "Аккаунт с таким логином не может быть создан";
                                }
                            }
                            else
                            {
                                FailedLoginFlag = Visibility.Visible;
                                FailedLoginLabel = "Неверный формат логина";
                            }
                        }
                        else
                        {
                            FailedEmailFlag = Visibility.Visible;
                            FailedEmailLabel = "Уже существует аккаунт с этой почтой";
                        }
                    }
                    else
                    {
                        FailedEmailFlag = Visibility.Visible;
                        FailedEmailLabel = "Неверный формат почты";
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
                        // raise a new exception nesting
                        // the current instance as InnerException
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
