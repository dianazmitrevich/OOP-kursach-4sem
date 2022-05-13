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
        private readonly GoodsSupplyContext context = new GoodsSupplyContext();

        private ObservableCollection<ORDERED_PRODUCTS> orderedProductsList = null;
        private string orderedProductsName;
        private string couponCodeToAdd;
        private double cartPrice;
        private double cartPriceNew;
        private double price;

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
                        CartPriceNew = (double)(CartPrice - coupon.MoneyOff);
                    }
                }
                else CartPriceNew = CartPrice;
            }
            else CartPriceNew = CartPrice;
        }

        public CartWindowViewModel()
        {
            OrderedProductsList = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == 3));
            SearchCouponsCommand = new DelegateCommand(OnSearchCouponsCommandExecuted);
            ApplyCouponCommand = new DelegateCommand(OnApplyCouponCommandExecuted, CanApplyCouponCommandExecute);

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
    }
}