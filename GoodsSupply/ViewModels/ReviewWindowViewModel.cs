﻿using GoodsSupply.Commands;
using GoodsSupply.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GoodsSupply.ViewModels
{
    class ReviewWindowViewModel : BaseViewModel
    {
        GoodsSupplyContext context = new GoodsSupplyContext();

        private PRODUCTS product;
        private string productCode;
        private string productName;
        private string reviewText;
        private string reviewTextSymbols;
        private Brush symbolsLabel;
        private bool isSymbolsAbove;

        public PRODUCTS Product
        {
            get => product;
            set => Set(ref product, value);
        }
        public string ProductCode
        {
            get => productCode;
            set => Set(ref productCode, value);
        }
        public string ProductName
        {
            get => productName;
            set => Set(ref productName, value);
        }
        public string ReviewText
        {
            get => reviewText;
            set
            {
                Set(ref reviewText, value);
                GetSymbols(); ChangeLabelColor();
                IsTextAbove();
            }
        }
        public string ReviewTextSymbols
        {
            get => reviewTextSymbols;
            set => Set(ref reviewTextSymbols, value);
        }
        public Brush SymbolsLabel
        {
            get => symbolsLabel;
            set => Set(ref symbolsLabel, value);
        }
        public bool IsSymbolsAbove
        {
            get => isSymbolsAbove;
            set => Set(ref isSymbolsAbove, value);
        }

        private void GetSymbols()
        {
            if (ReviewText != "")
            {
                ReviewTextSymbols = $"{ReviewText.Length}/200";
            }
        }
        private void IsTextAbove()
        {
            if (ReviewText.Length <= 200)
                IsSymbolsAbove = false;
            else if (ReviewText.Length > 200)
                IsSymbolsAbove = true;
        }

        private void ChangeLabelColor()
        {
            if (ReviewText.Length > 200)
                SymbolsLabel = new SolidColorBrush(Color.FromRgb(190, 23, 23));
            else
                SymbolsLabel = new SolidColorBrush(Color.FromRgb(183, 183, 183));
        }

        public ICommand AddReviewCommand { get; }
        private bool CanAddReviewCommandExecute(object p) => ReviewText != "" && IsSymbolsAbove == false;
        private void OnAddReviewCommandExecuted(object p)
        {
            if (ReviewText.Length <= 200)
            {
                var window = Application.Current.Windows[1];
                REVIEWS element = new REVIEWS(product.ProductId, "diana", ReviewText);
                context.REVIEWS.Add(element); context.SaveChanges();
                MessageBox.Show("Отзыв добавлен!");
                window.Close();
            }
        }

        public ReviewWindowViewModel(PRODUCTS productParameter)
        {
            AddReviewCommand = new DelegateCommand(OnAddReviewCommandExecuted, CanAddReviewCommandExecute);
            this.Product = productParameter;
            ProductCode = context.PRODUCTS_DETAIL.FirstOrDefault(f => f.LinkToProductId.Equals(Product.ProductId)).ProductCode.ToString();
            ProductName = Product.Name;
            ReviewText = "Текст отзыва";
            ReviewTextSymbols = "0/200";
            SymbolsLabel = new SolidColorBrush(Color.FromRgb(183, 183, 183));
            GetSymbols();
        }

        public ReviewWindowViewModel()
        {
            AddReviewCommand = new DelegateCommand(OnAddReviewCommandExecuted, CanAddReviewCommandExecute);
        }
    }
}
