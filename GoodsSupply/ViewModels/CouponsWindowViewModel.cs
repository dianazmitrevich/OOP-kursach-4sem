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

namespace GoodsSupply.ViewModels
{
    class CouponsWindowViewModel : BaseViewModel
    {
        private readonly GoodsSupplyContext context = new GoodsSupplyContext();

        private ObservableCollection<COUPONS> couponsList;
        private COUPONS selectedItem;

        public ObservableCollection<COUPONS> CouponsList
        {
            get => couponsList;
            set => Set(ref couponsList, value);
        }
        public COUPONS SelectedItem
        {
            get => selectedItem;
            set
            {
                Set(ref selectedItem, value);
                Clipboard.SetText(SelectedItem.CouponCode);
                MessageBox.Show("Код скопирован!");
            }
        }

        public CouponsWindowViewModel()
        {
            if (context.COUPONS.Count() > 0)
                CouponsList = new ObservableCollection<COUPONS>(context.COUPONS);
        }
    }
}
