using GoodsSupply.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        }
    }
}
