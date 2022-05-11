﻿using GoodsSupply.Commands;
using GoodsSupply.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GoodsSupply.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        private readonly GoodsSupplyContext context = new GoodsSupplyContext();

        private PERSONAL_ACCOUNTS account;
        private ObservableCollection<CATEGORIES> categoriesList;
        private ObservableCollection<PRODUCTS> productsList = null;
        private ObservableCollection<PRODUCTS_DETAIL> productsDetail = null;
        private ObservableCollection<REVIEWS> productReviews = null;
        private ObservableCollection<ORDERED_PRODUCTS> cartItems = null;
        private CATEGORIES selectedItem;
        private PRODUCTS selectedProductItem;
        private Visibility noCategoryselectedFlag = Visibility.Visible;
        private Visibility noProductSelectedFlag = Visibility.Visible;
        private Visibility isCategorySelectedFlag = Visibility.Hidden;
        private Visibility isProductSelectedFlag = Visibility.Collapsed;
        private Visibility isReviewsEmpty = Visibility.Visible;
        private Visibility isCartEmpty = Visibility.Visible;
        private Visibility isEmptySearch = Visibility.Collapsed;

        private int selectedQuantity = 0;
        private int selectedItemQuantity = 0;
        private double cartPrice = 0;
        private int searchProductCode;

        private string quantityLabel;
        private Brush brushQuantity;

        public PERSONAL_ACCOUNTS Account
        {
            get => account;
            set => Set(ref account, value);
        }
        public ObservableCollection<CATEGORIES> CategoriesList
        {
            get => categoriesList;
            set => Set(ref categoriesList, value);
        }
        public ObservableCollection<PRODUCTS> ProductsList
        {
            get => productsList;
            set => Set(ref productsList, value);
        }
        public ObservableCollection<ORDERED_PRODUCTS> CartItems
        {
            get => cartItems;
            set => Set(ref cartItems, value);
        }
        public CATEGORIES SelectedItem
        {
            get => selectedItem;
            set
            {
                Set(ref selectedItem, value);
                ShowProducts();
            }
        }
        public PRODUCTS SelectedProductItem
        {
            get => selectedProductItem;
            set
            {
                Set(ref selectedProductItem, value);
                ShowProductsDetail();
            }
        }
        public Visibility NoCategoryselectedFlag
        {
            get => noCategoryselectedFlag;
            set => Set(ref noCategoryselectedFlag, value);
        }
        public Visibility NoProductSelectedFlag
        {
            get => noProductSelectedFlag;
            set => Set(ref noProductSelectedFlag, value);
        }
        public Visibility IsCategorySelectedFlag
        {
            get => isCategorySelectedFlag;
            set => Set(ref isCategorySelectedFlag, value);
        }
        public Visibility IsProductSelectedFlag
        {
            get => isProductSelectedFlag;
            set => Set(ref isProductSelectedFlag, value);
        }
        public Visibility IsReviewsEmpty
        {
            get => isReviewsEmpty;
            set => Set(ref isReviewsEmpty, value);
        }
        public Visibility IsCartEmpty
        {
            get => isCartEmpty;
            set => Set(ref isCartEmpty, value);
        }
        public Visibility IsEmptySearch
        {
            get => isEmptySearch;
            set => Set(ref isEmptySearch, value);
        }
        public ObservableCollection<PRODUCTS_DETAIL> ProductsDetail
        {
            get => productsDetail;
            set => Set(ref productsDetail, value);
        }
        public ObservableCollection<REVIEWS> ProductReviews
        {
            get => productReviews;
            set => Set(ref productReviews, value);
        }
        public string QuantityLabel
        {
            get => quantityLabel;
            set => Set(ref quantityLabel, value);
        }
        public int SearchProductCode
        {
            get => searchProductCode;
            set => Set(ref searchProductCode, value);
        }
        public int SelectedQuantity
        {
            get => selectedQuantity;
            set => Set(ref selectedQuantity, value);
        }
        public int SelectedItemQuantity
        {
            get => selectedItemQuantity;
            set => Set(ref selectedItemQuantity, value);
        }
        public double CartPrice
        {
            get => cartPrice;
            set => Set(ref cartPrice, value);
        }
        public Brush BrushQuantity
        {
            get => brushQuantity;
            set => Set(ref brushQuantity, value);
        }

        void ShowProducts()
        {
            ProductsList = new ObservableCollection<PRODUCTS>(context.PRODUCTS.Where(f => f.LinkToCategoryId.Equals(SelectedItem.CategoryId)));
            NoCategoryselectedFlag = Visibility.Collapsed;
            NoProductSelectedFlag = Visibility.Visible;
            IsCategorySelectedFlag = Visibility.Visible;
            IsProductSelectedFlag = Visibility.Collapsed;
            IsReviewsEmpty = Visibility.Visible;
            IsEmptySearch = Visibility.Collapsed;
            SelectedQuantity = 0;
        }

        void ShowProductsDetail()
        {
            if (SelectedProductItem != null)
            {
                int quantity = SelectedProductItem.Quantity;
                if (quantity < 5)
                {
                    QuantityLabel = "<5 на складе";
                    BrushQuantity = Brushes.Red;
                }
                else if (quantity >= 5 && quantity < 10)
                {
                    QuantityLabel = "<10 на складе";
                    BrushQuantity = Brushes.Orange;
                }
                else if (quantity >= 10)
                {
                    QuantityLabel = ">10 на складе";
                    BrushQuantity = Brushes.Green;
                }

                ProductsDetail = new ObservableCollection<PRODUCTS_DETAIL>(context.PRODUCTS_DETAIL.Where(f => f.LinkToProductId.Equals(SelectedProductItem.ProductId)));
                ProductReviews = new ObservableCollection<REVIEWS>(context.REVIEWS.Where(f => f.LinkToProductId.Equals(SelectedProductItem.ProductId)));
            }

            SelectedQuantity = 0;
            SelectedItemQuantity = SelectedProductItem.Quantity;
            IsReviewsEmpty = Visibility.Visible;
            IsProductSelectedFlag = Visibility.Visible;
            NoProductSelectedFlag = Visibility.Collapsed;

            if (ProductReviews.Count > 0)
                IsReviewsEmpty = Visibility.Collapsed;
        }

        public ICommand AddQuantityCommand { get; }
        private bool CanAddQuantityCommandExecute(object p) => SelectedItemQuantity > SelectedQuantity;
        private void OnAddQuantityCommandExecuted(object p)
        {
            SelectedQuantity++;
        }

        public ICommand RemoveQuantityCommand { get; }
        private bool CanRemoveQuantityCommandExecute(object p) => SelectedQuantity > 0;
        private void OnRemoveQuantityCommandExecuted(object p)
        {
            SelectedQuantity--;
        }

        public ICommand AddToCartCommand { get; }
        private bool CanAddToCartCommandExecute(object p) => SelectedQuantity > 0;
        private void OnAddToCartCommandExecuted(object p)
        {
            int elementCount = context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == 3 && context.PRODUCTS_DETAIL.FirstOrDefault(t => t.LinkToProductId.Equals(this.SelectedProductItem.ProductId)).ProductCode == f.OrderedProductId).Count();
            var element = context.ORDERED_PRODUCTS.FirstOrDefault(f => f.LinkToOrderId == 3 && context.PRODUCTS_DETAIL.FirstOrDefault(t => t.LinkToProductId.Equals(this.SelectedProductItem.ProductId)).ProductCode == f.OrderedProductId);
            int previousQuantity = 0;
            if (elementCount > 0)
            {
                previousQuantity = element.OrderedQuantity;
                context.ORDERED_PRODUCTS.Remove(element);
            }
            ORDERED_PRODUCTS newelement = new ORDERED_PRODUCTS(context.PRODUCTS_DETAIL.FirstOrDefault(f => f.LinkToProductId.Equals(this.SelectedProductItem.ProductId)).ProductCode, SelectedQuantity + previousQuantity, 3, (float)SelectedProductItem.Price);
            context.ORDERED_PRODUCTS.Add(newelement); context.SaveChanges();

            if (context.ORDERED_PRODUCTS.Count() > 0)
                IsCartEmpty = Visibility.Collapsed;
            else
                IsCartEmpty = Visibility.Visible;

            CartItems = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == 3));

            if (context.ORDERED_PRODUCTS.Count() > 0)
            {
                for (int i = 0; i < context.ORDERED_PRODUCTS.Count(); i++)
                {
                    CartPrice += CartItems[i].OrderedProductPrice * CartItems[i].OrderedQuantity;
                }
            }
        }

        public ICommand SearchCodeCommand { get; }
        private bool CanSearchCodeCommandExecute(object p) => context.PRODUCTS.Count() > 0;
        private void OnSearchCodeCommandExecuted(object p)
        {
            var productDetail = new ObservableCollection<PRODUCTS_DETAIL>(context.PRODUCTS_DETAIL.Where(f => f.ProductCode.Equals(SearchProductCode)));

            if (productDetail.Count() > 0)
            {
                var product = productDetail[0];
                ProductsList = new ObservableCollection<PRODUCTS>(context.PRODUCTS.Where(f => f.ProductId.Equals(product.LinkToProductId)));
                IsEmptySearch = Visibility.Collapsed;
                NoCategoryselectedFlag = Visibility.Collapsed;
                IsCategorySelectedFlag = Visibility.Visible;
                SelectedProductItem = ProductsList[0];
            }
            else
            {
                ProductsList = null;
                NoCategoryselectedFlag = Visibility.Collapsed;
                IsEmptySearch = Visibility.Visible;
                IsCategorySelectedFlag = Visibility.Collapsed;
            }
        }

        public ICommand SortAscCommand { get; }
        private bool CanSortAscCommandExecute(object p) => context.PRODUCTS.Count() > 0 && NoCategoryselectedFlag == Visibility.Collapsed && IsEmptySearch == Visibility.Collapsed;
        private void OnSortAscCommandExecuted(object p) => ProductsList = new ObservableCollection<PRODUCTS>(ProductsList.OrderBy(f => f.Price));

        public ICommand SortDescCommand { get; }
        private bool CanSortDescCommandExecute(object p) => context.PRODUCTS.Count() > 0 && NoCategoryselectedFlag == Visibility.Collapsed && IsEmptySearch == Visibility.Collapsed;
        private void OnSortDescCommandExecuted(object p) => ProductsList = new ObservableCollection<PRODUCTS>(ProductsList.OrderByDescending(f => f.Price));

        public ICommand SortAlphabetCommand { get; }
        private bool CanSortAlphabetCommandExecute(object p) => context.PRODUCTS.Count() > 0 && NoCategoryselectedFlag == Visibility.Collapsed && IsEmptySearch == Visibility.Collapsed;
        private void OnSortAlphabetCommandExecuted(object p) => ProductsList = new ObservableCollection<PRODUCTS>(ProductsList.OrderBy(f => f.Name));

        public MainWindowViewModel(PERSONAL_ACCOUNTS accountParameter)
        {
            this.Account = accountParameter;
            CategoriesList = new ObservableCollection<CATEGORIES>(context.CATEGORIES);
        }

        public MainWindowViewModel()
        {
            CartPrice = 0;
            CategoriesList = new ObservableCollection<CATEGORIES>(context.CATEGORIES);
            CartItems = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == 3));

            if (CartItems.Count > 0)
                IsCartEmpty = Visibility.Collapsed;
            else
                IsCartEmpty = Visibility.Visible;

            AddQuantityCommand = new DelegateCommand(OnAddQuantityCommandExecuted, CanAddQuantityCommandExecute);
            RemoveQuantityCommand = new DelegateCommand(OnRemoveQuantityCommandExecuted, CanRemoveQuantityCommandExecute);
            AddToCartCommand = new DelegateCommand(OnAddToCartCommandExecuted, CanAddToCartCommandExecute);
            SearchCodeCommand = new DelegateCommand(OnSearchCodeCommandExecuted, CanSearchCodeCommandExecute);
            SortAscCommand = new DelegateCommand(OnSortAscCommandExecuted, CanSortAscCommandExecute);
            SortDescCommand = new DelegateCommand(OnSortDescCommandExecuted, CanSortDescCommandExecute);
            SortAlphabetCommand = new DelegateCommand(OnSortAlphabetCommandExecuted, CanSortAlphabetCommandExecute);
        }
    }
}