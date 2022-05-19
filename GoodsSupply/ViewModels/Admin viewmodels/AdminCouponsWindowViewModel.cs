using GoodsSupply.Commands;
using GoodsSupply.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GoodsSupply.ViewModels.Admin_viewmodels
{
    partial class AdminCouponsWindowViewModel : BaseViewModel
    {
        private readonly GoodsSupplyContext context = new GoodsSupplyContext();

        private ObservableCollection<COUPONS> couponsList = null;
        private Visibility isCouponsEmpty = Visibility.Collapsed;
        private Visibility isCouponsNotEmpty = Visibility.Visible;
        private COUPONS selectedCoupon = null;
        private int percentOff;
        private double moneyOff;
        private bool isPercent = false;
        private bool isMoney = false;

        public ObservableCollection<COUPONS> CouponsList
        {
            get => couponsList;
            set => Set(ref couponsList, value);
        }
        public Visibility IsCouponsEmpty
        {
            get => isCouponsEmpty;
            set => Set(ref isCouponsEmpty, value);
        }
        public Visibility IsCouponsNotEmpty
        {
            get => isCouponsNotEmpty;
            set => Set(ref isCouponsNotEmpty, value);
        }
        public COUPONS SelectedCoupon
        {
            get => selectedCoupon;
            set
            {
                Set(ref selectedCoupon, value);
                SetOff();

                if (SelectedCoupon != null && SelectedCoupon.CouponCode.Equals("Новый купон"))
                {
                    IsMoney = true; IsPercent = true;
                }
            }
        }
        public int PercentOff
        {
            get => percentOff;
            set
            {
                Set(ref percentOff, value);
                WhichDiscount();
            }
        }
        public double MoneyOff
        {
            get => moneyOff;
            set
            {
                Set(ref moneyOff, value);
                WhichDiscount();
            }
        }
        public bool IsPercent
        {
            get => isPercent;
            set => Set(ref isPercent, value);
        }
        public bool IsMoney
        {
            get => isMoney;
            set => Set(ref isMoney, value);
        }

        private void SetOff()
        {
            if (SelectedCoupon != null)
            {
                if (SelectedCoupon.IsPercent.Equals("Y"))
                {
                    PercentOff = SelectedCoupon.PercentOff;
                    MoneyOff = 0;
                    IsPercent = true; IsMoney = false;
                }
                else
                {
                    MoneyOff = (double)SelectedCoupon.MoneyOff;
                    PercentOff = 0;
                    IsPercent = false; IsMoney = true;
                }
            }
        }

        private bool CouponValidation()
        {
            bool flag = true;

            if (SelectedCoupon != null)
            {
                int couponId = SelectedCoupon.CouponId;
                var coupon = context.COUPONS.FirstOrDefault(f => f.CouponId == couponId);

                if (coupon.CouponCode.Length > 0 && coupon.CouponCode.Length <= 100 && coupon.CouponCode != "")
                {
                    if (coupon.CouponText.Length > 0 && coupon.CouponText.Length <= 100 && coupon.CouponText != "")
                    {
                        if (coupon.IsPercent.Equals("Y") && PercentOff > 0 && PercentOff <= 100)
                            flag = true;
                        else if (coupon.IsPercent.Equals("N") && MoneyOff > 0 && MoneyOff <= 1000)
                            flag = true;
                        else flag = false;
                    }
                    else flag = false;
                }
                else flag = false;
            }
            else flag = false;

            return flag;
        }

        private void IsCouponsListEmpty()
        {
            if (CouponsList.Count() > 0)
            {
                IsCouponsEmpty = Visibility.Collapsed;
                IsCouponsNotEmpty = Visibility.Visible;
            }
            else
            {
                IsCouponsEmpty = Visibility.Visible;
                IsCouponsNotEmpty = Visibility.Collapsed;
            }
        }

        private void WhichDiscount()
        {
            if (SelectedCoupon != null)
            {
                if (PercentOff > 0)
                {
                    IsMoney = false;
                    SelectedCoupon.IsPercent = "Y"; context.SaveChanges();
                }
                else if (MoneyOff > 0)
                {
                    IsPercent = false;
                    SelectedCoupon.IsPercent = "N"; context.SaveChanges();
                }
            }
        }

        public ICommand UpdateCouponCommand { get; }
        private bool CanUpdateCouponCommandExecute(object p) => CouponValidation();
        private void OnUpdateCouponCommandExecuted(object p)
        {
            var coupon = context.COUPONS.FirstOrDefault(f => f.CouponId == SelectedCoupon.CouponId);

            coupon.PercentOff = PercentOff; coupon.MoneyOff = MoneyOff;
            context.SaveChanges();

            MessageBox.Show("Купон обновлен");
        }

        public ICommand DeleteCouponCommand { get; }
        private bool CanDeleteCouponCommandExecute(object p) => SelectedCoupon != null;
        private void OnDeleteCouponCommandExecuted(object p)
        {
            var coupon = context.COUPONS.FirstOrDefault(f => f.CouponId == SelectedCoupon.CouponId);
            MessageBoxButton buttons = MessageBoxButton.YesNo;

            if (coupon != null)
            {
                var result = MessageBox.Show("Вы точно хотите удалить этот" + "\n" + $"купон? ({coupon.CouponCode})", "Удаление купона", buttons, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    context.COUPONS.Remove(coupon); context.SaveChanges();

                    CouponsList = new ObservableCollection<COUPONS>(context.COUPONS);
                    IsCouponsListEmpty();
                }
                else return;
            }
        }

        public ICommand AddCouponCommand { get; }
        private bool CanAddCouponCommandExecute(object p)
        {
            var flag = true;
            var coupons = new ObservableCollection<COUPONS>(context.COUPONS.Where(f => f.CouponCode.Equals("Новый купон")));

            if (coupons.Count() > 0)
                flag = false;
            else if (CouponValidation() || SelectedCoupon == null)
                flag = true;

            return flag;
        }
        private void OnAddCouponCommandExecuted(object p)
        {
            if (CouponValidation() || SelectedCoupon == null)
            {
                var element = new COUPONS("Новый купон", "Y", 0, 0, "Нажмите для редактирования");
                context.COUPONS.Add(element); context.SaveChanges();

                CouponsList = new ObservableCollection<COUPONS>(context.COUPONS);
                IsCouponsListEmpty();
            }
        }

        public AdminCouponsWindowViewModel()
        {
            CouponsList = new ObservableCollection<COUPONS>(context.COUPONS);

            IsCouponsListEmpty();

            UpdateCouponCommand = new DelegateCommand(OnUpdateCouponCommandExecuted, CanUpdateCouponCommandExecute);
            DeleteCouponCommand = new DelegateCommand(OnDeleteCouponCommandExecuted, CanDeleteCouponCommandExecute);
            AddCouponCommand = new DelegateCommand(OnAddCouponCommandExecuted, CanAddCouponCommandExecute);
        }
    }
}
