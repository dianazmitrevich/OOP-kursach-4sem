﻿using GoodsSupply.Commands;
using GoodsSupply.Models;
using GoodsSupply.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GoodsSupply.ViewModels
{
    class CartWindowViewModel : BaseViewModel
    {
        #region private variables
        private readonly GoodsSupplyContext context = new GoodsSupplyContext();

        private ObservableCollection<ORDERED_PRODUCTS> orderedProductsList = null;
        private string orderedProductsName;
        private string couponCodeToAdd;
        private double cartPrice;
        private double cartPriceNew;
        private double price;
        private ORDERS order;
        private PERSONAL_ACCOUNTS account;
        private Visibility isCartEmpty = Visibility.Collapsed;
        private Visibility isCartNotEmpty = Visibility.Visible;
        private string paymentMethod;
        private bool isByCash = false;
        private bool isByCard = false;
        private bool isSendEmail = false;
        private string adress;
        #endregion

        #region public variables
        public ObservableCollection<ORDERED_PRODUCTS> OrderedProductsList
        {
            get => orderedProductsList;
            set => Set(ref orderedProductsList, value);
        }
        public string OrderedProductsName
        {
            get => orderedProductsName;
            set => Set(ref orderedProductsName, value);
        }
        public string PaymentMethod
        {
            get => paymentMethod;
            set => Set(ref paymentMethod, value);
        }
        public string Adress
        {
            get => adress;
            set => Set(ref adress, value);
        }
        public bool IsSendEmail
        {
            get => isSendEmail;
            set => Set(ref isSendEmail, value);
        }
        public bool IsByCash
        {
            get => isByCash;
            set
            {
                Set(ref isByCash, value);
                SetPaymentMethod();
            }
        }
        public bool IsByCard
        {
            get => isByCard;
            set
            {
                Set(ref isByCard, value);
                SetPaymentMethod();
            }
        }
        public Visibility IsCartEmpty
        {
            get => isCartEmpty;
            set => Set(ref isCartEmpty, value);
        }
        public Visibility IsCartNotEmpty
        {
            get => isCartNotEmpty;
            set => Set(ref isCartNotEmpty, value);
        }
        public ORDERS Order
        {
            get => order;
            set => Set(ref order, value);
        }
        public PERSONAL_ACCOUNTS Account
        {
            get => account;
            set => Set(ref account, value);
        }
        public string CouponCodeToAdd
        {
            get => couponCodeToAdd;
            set => Set(ref couponCodeToAdd, value);
        }
        public double CartPrice
        {
            get => cartPrice;
            set => Set(ref cartPrice, value);
        }
        public double CartPriceNew
        {
            get => cartPriceNew;
            set => Set(ref cartPriceNew, value);
        }
        #endregion

        private void EditCartPrice()
        {
            if (OrderedProductsList.Count() > 0)
            {
                price = 0;
                for (int i = 0; i < OrderedProductsList.Count(); i++)
                {
                    price += OrderedProductsList[i].OrderedProductPrice * OrderedProductsList[i].OrderedQuantity;
                }

                IsCartNotEmpty = Visibility.Visible;
                IsCartEmpty = Visibility.Collapsed;
                CartPrice = price;
                CartPriceNew = CartPrice;
            }
            else
            {
                IsCartNotEmpty = Visibility.Collapsed;
                IsCartEmpty = Visibility.Visible;
                CartPrice = 0;
                CartPriceNew = 0;
            }
        }

        private void SetPaymentMethod()
        {
            if (IsByCard == true && IsByCash == false)
                PaymentMethod = "картой";
            else if (IsByCash == true && IsByCard == false)
                PaymentMethod = "наличными";
            else return;
        }

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

        public ICommand SearchCouponsCommand { get; }
        private void OnSearchCouponsCommandExecuted(object p)
        {
            var couponsWindow = new CouponsWindow();
            couponsWindow.ShowDialog();
        }

        public ICommand ApplyCouponCommand { get; }
        private bool CanApplyCouponCommandExecute(object p) => CouponCodeToAdd != null;
        private void OnApplyCouponCommandExecuted(object p)
        {
            if (!Regex.IsMatch(CouponCodeToAdd, @"[-.?!)(,:]"))
            {
                var coupon = context.COUPONS.FirstOrDefault(f => f.CouponCode.Equals(CouponCodeToAdd));

                if (coupon != null)
                {
                    if (coupon.IsPercent.Equals("Y"))
                    {
                        CartPriceNew = CartPrice - CartPrice * coupon.PercentOff / 100;
                    }
                    if (coupon.IsPercent.Equals("N"))
                    {
                        if (CartPrice >= 1000)
                            CartPriceNew = (double)(CartPrice - coupon.MoneyOff);
                    }
                }
                else CartPriceNew = CartPrice;
            }
            else CartPriceNew = CartPrice;
        }

        public ICommand RemoveQuantityCommand { get; }
        private bool CanRemoveQuantityCommandExecute(object p)
        {
            bool flag = true;
            int productCode = Convert.ToInt32(p);
            var element = context.ORDERED_PRODUCTS.FirstOrDefault(f => f.OrderedProductId.Equals(productCode) && f.LinkToOrderId == Order.OrderId);

            if (element != null && element.OrderedQuantity > 1)
                flag = true;
            else flag = false;
            return flag;
        }
        private void OnRemoveQuantityCommandExecuted(object p)
        {
            int productCode = Convert.ToInt32(p);
            var element = context.ORDERED_PRODUCTS.FirstOrDefault(f => f.OrderedProductId.Equals(productCode) && f.LinkToOrderId == Order.OrderId);

            if (element != null)
            {
                element.OrderedQuantity -= 1; context.SaveChanges();

                var productDetail = context.PRODUCTS_DETAIL.FirstOrDefault(f => f.ProductCode.Equals(productCode));
                var product = context.PRODUCTS.FirstOrDefault(f => f.ProductId.Equals(productDetail.LinkToProductId));
                product.Quantity += 1; context.SaveChanges();

                EditCartPrice();
                OrderedProductsList = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == Order.OrderId));
            }
        }

        public ICommand AddQuantityCommand { get; }
        private bool CanAddQuantityCommandExecute(object p)
        {
            bool flag = true;
            int productCode = Convert.ToInt32(p);
            var element = context.ORDERED_PRODUCTS.FirstOrDefault(f => f.OrderedProductId.Equals(productCode) && f.LinkToOrderId == Order.OrderId);

            if (element != null)
            {
                var elementDetail = context.PRODUCTS_DETAIL.FirstOrDefault(o => o.ProductCode.Equals(productCode)).LinkToProductId;
                int elementQuantity = context.PRODUCTS.FirstOrDefault(f => f.ProductId.Equals(elementDetail)).Quantity;

                if (element != null && element.OrderedQuantity <= elementQuantity)
                    flag = true;
                else flag = false;
            }
            return flag;
        }
        private void OnAddQuantityCommandExecuted(object p)
        {
            int productCode = Convert.ToInt32(p);
            var element = context.ORDERED_PRODUCTS.FirstOrDefault(f => f.OrderedProductId.Equals(productCode) && f.LinkToOrderId == Order.OrderId);

            if (element != null)
            {
                element.OrderedQuantity += 1; context.SaveChanges();

                EditCartPrice();
                OrderedProductsList = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == Order.OrderId));
            }
        }

        public ICommand DeleteOrderedProductCommand { get; }
        private bool CanDeleteOrderedProductCommandExecute(object p)
        {
            bool flag = true;
            int productCode = Convert.ToInt32(p);
            var element = context.ORDERED_PRODUCTS.FirstOrDefault(f => f.OrderedProductId.Equals(productCode));

            if (element != null)
                flag = true;
            else flag = false;
            return flag;
        }
        private void OnDeleteOrderedProductCommandExecuted(object p)
        {
            int productCode = Convert.ToInt32(p);
            var element = context.ORDERED_PRODUCTS.FirstOrDefault(f => f.OrderedProductId.Equals(productCode));
            MessageBoxButton buttons = MessageBoxButton.YesNo;

            if (element != null)
            {
                var result = MessageBox.Show("Вы точно хотите удалить этот" + "\n" + "товар из корзины?", "Удаление товара", buttons, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var productDetail = context.PRODUCTS_DETAIL.FirstOrDefault(f => f.ProductCode.Equals(productCode));
                    var product = context.PRODUCTS.FirstOrDefault(f => f.ProductId.Equals(productDetail.LinkToProductId));
                    product.Quantity += element.OrderedQuantity; context.SaveChanges();

                    context.ORDERED_PRODUCTS.Remove(element); context.SaveChanges();

                    OrderedProductsList = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == Order.OrderId));
                    EditCartPrice();
                }
                else return;
            }
        }

        public ICommand AddOrderCommand { get; }
        private bool CanAddOrderCommandExecute(object p) => Adress != null && (IsByCash || IsByCard) && OrderedProductsList.Count() > 0;
        private void OnAddOrderCommandExecuted(object p)
        {
            var order = context.ORDERS.FirstOrDefault(f => f.OrderId == Order.OrderId);
            order.Adress = Adress;
            order.PaymentMethod = PaymentMethod;
            order.OrderPrice = CartPrice;
            order.FinalOrderPrice = CartPriceNew;
            if (CouponCodeToAdd != null)
                order.Coupon = CouponCodeToAdd;
            order.Status = "Заказ оформлен";

            context.SaveChanges();

            var window = Application.Current.Windows[1];
            var model = new MyOrdersWindowViewModel(Account);
            var myOrders = new MyOrdersWindow();
            myOrders.DataContext = model;

            if (IsSendEmail)
            {
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

                        MessageBox.Show("Заказ успешно" + "\n" + "оформлен. Чек отправлен" + "\n" + "на вашу почту.");
                    }
                    catch
                    {
                        MessageBox.Show("Не удалось отправить" + "\n" + "чек на почту :(" + "\n\n" + "Свой заказ можете" + "\n" + "просмотреть в 'Мои заказы'");
                    }
                }
            }
            else
                MessageBox.Show("Заказ успешно+" + "\n" + "оформлен");

            OrderedProductsList.Clear();
            myOrders.Show();
            window.Close();
        }

        public CartWindowViewModel(ORDERS order, PERSONAL_ACCOUNTS account)
        {
            this.Order = order;
            this.Account = account;

            OrderedProductsList = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == Order.OrderId));
            SearchCouponsCommand = new DelegateCommand(OnSearchCouponsCommandExecuted);
            ApplyCouponCommand = new DelegateCommand(OnApplyCouponCommandExecuted, CanApplyCouponCommandExecute);
            RemoveQuantityCommand = new DelegateCommand(OnRemoveQuantityCommandExecuted, CanRemoveQuantityCommandExecute);
            AddQuantityCommand = new DelegateCommand(OnAddQuantityCommandExecuted, CanAddQuantityCommandExecute);
            DeleteOrderedProductCommand = new DelegateCommand(OnDeleteOrderedProductCommandExecuted, CanDeleteOrderedProductCommandExecute);
            AddOrderCommand = new DelegateCommand(OnAddOrderCommandExecuted, CanAddOrderCommandExecute);

            EditCartPrice();
        }
    }
}