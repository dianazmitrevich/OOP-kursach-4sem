using GoodsSupply.Commands;
using GoodsSupply.Models;
using GoodsSupply.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

                CartPrice = price;
                CartPriceNew = CartPrice;
            }
            else
            {
                CartPrice = 0;
                CartPriceNew = 0;
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
            var element = context.ORDERED_PRODUCTS.FirstOrDefault(f => f.OrderedProductId.Equals(productCode));

            if (element != null && element.OrderedQuantity > 1)
                flag = true;
            else flag = false;
            return flag;
        }
        private void OnRemoveQuantityCommandExecuted(object p)
        {
            int productCode = Convert.ToInt32(p);
            var element = context.ORDERED_PRODUCTS.FirstOrDefault(f => f.OrderedProductId.Equals(productCode));

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
            var element = context.ORDERED_PRODUCTS.FirstOrDefault(f => f.OrderedProductId.Equals(productCode));

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
            var element = context.ORDERED_PRODUCTS.FirstOrDefault(f => f.OrderedProductId.Equals(productCode));

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

                    EditCartPrice();
                    OrderedProductsList = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == Order.OrderId));
                }
                else return;
            }
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

            EditCartPrice();
        }
    }
}