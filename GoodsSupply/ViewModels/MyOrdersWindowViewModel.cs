using GoodsSupply.Commands;
using GoodsSupply.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GoodsSupply.ViewModels
{
    partial class MyOrdersWindowViewModel : BaseViewModel
    {
        #region private variables
        private readonly GoodsSupplyContext context = new GoodsSupplyContext();

        private PERSONAL_ACCOUNTS account;
        public ObservableCollection<ORDERS> ordersList;
        private Visibility isOrdersEmpty = Visibility.Collapsed;
        private Visibility isOrdersNotEmpty = Visibility.Visible;
        #endregion

        #region public variables
        public PERSONAL_ACCOUNTS Account
        {
            get => account;
            set => Set(ref account, value);
        }
        public ObservableCollection<ORDERS> OrdersList
        {
            get => ordersList;
            set => Set(ref ordersList, value);
        }
        public Visibility IsOrdersEmpty
        {
            get => isOrdersEmpty;
            set => Set(ref isOrdersEmpty, value);
        }
        public Visibility IsOrdersNotEmpty
        {
            get => isOrdersNotEmpty;
            set => Set(ref isOrdersNotEmpty, value);
        }
        #endregion

        private static bool CanPing()
        {
            const int timeout = 1000;
            const string host = "google.com";

            var ping = new Ping();
            var buffer = new byte[32];
            var pingOptions = new PingOptions();

            try
            {
                var reply = ping.Send(host, timeout, buffer, pingOptions);
                return (reply != null && reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ICommand SendCheckByEmailCommand { get; }
        private bool CanSendCheckByEmailCommandExecute(object p)
        {
            bool flag = true;
            int orderId = Convert.ToInt32(p);
            var OrderedProductsList = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == orderId));

            if (OrderedProductsList.Count() == 0)
                flag = false;

            return flag;
        }
        private void OnSendCheckByEmailCommandExecuted(object p)
        {
            int orderId = Convert.ToInt32(p);
            var order = context.ORDERS.FirstOrDefault(f => f.OrderId == orderId);
            var OrderedProductsList = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == orderId));

            if (CanPing())
            {
                try
                {
                    MailAddress from = new MailAddress("fsdvjnwrjnjsldfe@gmail.com", "Администратор приложения");
                    MailAddress to = new MailAddress($"{Account.Email}");
                    MailMessage m = new MailMessage(from, to);
                    m.Subject = "Доставка из IKEA";
                    m.Body = $"<h2>Добрый день, {Account.Name}!</h2>" +
                        $"<p>На сумму {order.FinalOrderPrice}₽ ({order.OrderPrice}₽ без купона) была заказана доставка по адресу {order.Adress}, оплата {order.PaymentMethod}.</p><p>";

                    foreach (var item in OrderedProductsList)
                    {
                        var detail = context.PRODUCTS_DETAIL.FirstOrDefault(f => f.ProductCode.Equals(item.OrderedProductId));
                        var product = context.PRODUCTS.FirstOrDefault(f => f.ProductId == detail.LinkToProductId);
                        m.Body += $"{product.Name}. {product.Description} - {item.OrderedQuantity} шт.<br>";
                    }
                    m.Body += $"</p><p>Текущий статус заказа - <i>{order.Status}</i></p>";

                    m.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.Credentials = new NetworkCredential("fsdvjnwrjnjsldfe@gmail.com", "Kursach_OOP2022");
                    smtp.EnableSsl = true;
                    smtp.Send(m);
                    Console.Read();

                    MessageBox.Show("Чек отправлен" + "\n" + "на вашу почту.");
                }
                catch
                {
                    MessageBox.Show("Не удалось отправить" + "\n" + "чек на почту :(");
                }
            }
        }


        public MyOrdersWindowViewModel(PERSONAL_ACCOUNTS account)
        {
            this.Account = account;

            OrdersList = new ObservableCollection<ORDERS>(context.ORDERS.Where(f => f.LinkAccountId == Account.AccountId));
            
            if (OrdersList.Count() > 0)
            {
                IsOrdersEmpty = Visibility.Collapsed;
                IsOrdersNotEmpty = Visibility.Visible;
            }
            else
            {
                IsOrdersEmpty = Visibility.Visible;
                IsOrdersNotEmpty = Visibility.Collapsed;
            }

            SendCheckByEmailCommand = new DelegateCommand(OnSendCheckByEmailCommandExecuted, CanSendCheckByEmailCommandExecute);
        }
    }
}
