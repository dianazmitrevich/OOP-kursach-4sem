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
            if (context.ORDERED_PRODUCTS.Count() > 0)
            {
                price = 0;
                for (int i = 0; i < context.ORDERED_PRODUCTS.Count(); i++)
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

            if (element != null && element.OrderedQuantity > 0)
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

                EditCartPrice();
                OrderedProductsList = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == 3));
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
                OrderedProductsList = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == 3));
            }
        }

        public CartWindowViewModel()
        {
            OrderedProductsList = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == 3));
            SearchCouponsCommand = new DelegateCommand(OnSearchCouponsCommandExecuted);
            ApplyCouponCommand = new DelegateCommand(OnApplyCouponCommandExecuted, CanApplyCouponCommandExecute);
            RemoveQuantityCommand = new DelegateCommand(OnRemoveQuantityCommandExecuted, CanRemoveQuantityCommandExecute);
            AddQuantityCommand = new DelegateCommand(OnAddQuantityCommandExecuted, CanAddQuantityCommandExecute);

            EditCartPrice();
        }
    }
}