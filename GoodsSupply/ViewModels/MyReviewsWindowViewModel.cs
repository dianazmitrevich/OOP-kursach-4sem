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
    partial class MyReviewsWindowViewModel : BaseViewModel
    {
        #region private variables
        private readonly GoodsSupplyContext context = new GoodsSupplyContext();

        private PERSONAL_ACCOUNTS account;
        public ObservableCollection<REVIEWS> reviewsList;
        private Visibility isReviewsEmpty = Visibility.Collapsed;
        private Visibility isReviewsNotEmpty = Visibility.Visible;
        #endregion

        #region public variables
        public PERSONAL_ACCOUNTS Account
        {
            get => account;
            set => Set(ref account, value);
        }
        public ObservableCollection<REVIEWS> ReviewsList
        {
            get => reviewsList;
            set => Set(ref reviewsList, value);
        }
        public Visibility IsReviewsEmpty
        {
            get => isReviewsEmpty;
            set => Set(ref isReviewsEmpty, value);
        }
        public Visibility IsReviewsNotEmpty
        {
            get => isReviewsNotEmpty;
            set => Set(ref isReviewsNotEmpty, value);
        }
        #endregion

        public MyReviewsWindowViewModel(PERSONAL_ACCOUNTS account)
        {
            this.Account = account;

            var login = context.USERS.FirstOrDefault(f => f.LinkAccountId == Account.AccountId).Login;

            ReviewsList = new ObservableCollection<REVIEWS>(context.REVIEWS.Where(f => f.LinkUserLogin.Equals(login)));

            if (ReviewsList.Count() > 0)
            {
                IsReviewsEmpty = Visibility.Collapsed;
                IsReviewsNotEmpty = Visibility.Visible;
            }
            else
            {
                IsReviewsEmpty = Visibility.Visible;
                IsReviewsNotEmpty = Visibility.Collapsed;
            }
        }
    }
}
