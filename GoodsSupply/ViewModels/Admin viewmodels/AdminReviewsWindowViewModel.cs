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
    partial class AdminReviewsWindowViewModel : BaseViewModel
    {
        private readonly GoodsSupplyContext context = new GoodsSupplyContext();

        private ObservableCollection<REVIEWS> reviewsList = null;
        private Visibility isReviewsEmpty = Visibility.Collapsed;
        private Visibility isReviewsNotEmpty = Visibility.Visible;
        private REVIEWS selectedReview = null;

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
        public REVIEWS SelectedReview
        {
            get => selectedReview;
            set
            {
                Set(ref selectedReview, value);
            }
        }

        private void IsReviewsListEmpty()
        {
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


        public ICommand DeleteReviewCommand { get; }
        private bool CanDeleteReviewCommandExecute(object p) => SelectedReview != null;
        private void OnDeleteReviewCommandExecuted(object p)
        {
            var review = context.REVIEWS.FirstOrDefault(f => f.ReviewId == SelectedReview.ReviewId);
            MessageBoxButton buttons = MessageBoxButton.YesNo;

            if (review != null)
            {
                var result = MessageBox.Show("Вы точно хотите удалить этот" + "\n" + $"отзыв?", "Удаление отзыва", buttons, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    context.REVIEWS.Remove(review); context.SaveChanges();

                    ReviewsList = new ObservableCollection<REVIEWS>(context.REVIEWS);
                    IsReviewsListEmpty();
                }
                else return;
            }
        }

        public ICommand UpdateReviewCommand { get; }
        private bool CanUpdateReviewCommandExecute(object p)
        {
            bool flag = true;
            
            if (SelectedReview != null)
            {
                int reviewId = SelectedReview.ReviewId;
                var review = context.REVIEWS.FirstOrDefault(f => f.ReviewId == reviewId);

                if (review != null)
                {
                    if (review.AdminName != null && review.AdminText != null)
                    {
                        if (review.AdminName.Length > 0 && review.AdminName.Length <= 100 & review.AdminName != "")
                        {
                            if (review.AdminText.Length > 0 && review.AdminText.Length <= 200 & review.AdminText != "" && review.AdminText != null)
                                flag = true;
                            else flag = false;
                        }
                        else flag = false;
                    }
                    else flag = false;
                }
                else flag = false;
            }
            else flag = false;

            return flag;
        }
        private void OnUpdateReviewCommandExecuted(object p)
        {
            if (SelectedReview != null)
            {
                context.SaveChanges();

                MessageBox.Show("Отзыв обновлен");
            }
        }

        public AdminReviewsWindowViewModel()
        {
            ReviewsList = new ObservableCollection<REVIEWS>(context.REVIEWS);

            IsReviewsListEmpty();

            DeleteReviewCommand = new DelegateCommand(OnDeleteReviewCommandExecuted, CanDeleteReviewCommandExecute);
            UpdateReviewCommand = new DelegateCommand(OnUpdateReviewCommandExecuted, CanUpdateReviewCommandExecute);
        }
    }
}
