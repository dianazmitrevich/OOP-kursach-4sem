﻿using GoodsSupply.Commands;
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
using System.Windows.Media;

namespace GoodsSupply.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        #region private variables
        private readonly GoodsSupplyContext context = new GoodsSupplyContext();

        private PERSONAL_ACCOUNTS account;
        private ORDERS order;
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
        private double price;

        private Brush borderProductBrush;

        private int selectedQuantity = 0;
        private int selectedItemQuantity = 0;
        private double cartPrice = 0;
        private string searchProductCode = "Поиск по артикулу";

        private string accountLogin;

        private string quantityLabel;
        private Brush brushQuantity;
        #endregion

        #region public variables
        public PERSONAL_ACCOUNTS Account
        {
            get => account;
            set => Set(ref account, value);
        }
        public ORDERS Order
        {
            get => order;
            set => Set(ref order, value);
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
        public Brush BorderProductBrush
        {
            get => borderProductBrush;
            set => Set(ref borderProductBrush, value);
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
        public string SearchProductCode
        {
            get => searchProductCode;
            set => Set(ref searchProductCode, value);
        }
        public string AccountLogin
        {
            get => accountLogin;
            set => Set(ref accountLogin, value);
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
        #endregion

        private void ShowProducts()
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

        private void ShowProductsDetail()
        {
            if (SelectedProductItem != null)
            {
                int quantity = SelectedProductItem.Quantity;
                if(quantity == 0)
                {
                    QuantityLabel = "0 в наличии";
                    BrushQuantity = Brushes.Tomato;
                }
                else if (quantity < 5)
                {
                    QuantityLabel = "< 5 в наличии";
                    BrushQuantity = Brushes.Red;
                }
                else if (quantity >= 5 && quantity < 10)
                {
                    QuantityLabel = "< 10 в наличии";
                    BrushQuantity = Brushes.Orange;
                }
                else if (quantity >= 10)
                {
                    QuantityLabel = "> 10 в наличии";
                    BrushQuantity = Brushes.Green;
                }

                ProductsDetail = new ObservableCollection<PRODUCTS_DETAIL>(context.PRODUCTS_DETAIL.Where(f => f.LinkToProductId.Equals(SelectedProductItem.ProductId)));
                ProductReviews = new ObservableCollection<REVIEWS>(context.REVIEWS.Where(f => f.LinkToProductId.Equals(SelectedProductItem.ProductId)));
                SelectedItemQuantity = SelectedProductItem.Quantity;
            }

            SelectedQuantity = 0;
            IsReviewsEmpty = Visibility.Visible;
            IsProductSelectedFlag = Visibility.Visible;
            NoProductSelectedFlag = Visibility.Collapsed;

            if (ProductReviews.Count > 0)
                IsReviewsEmpty = Visibility.Collapsed;
        }

        private void RefreshAll()
        {
            foreach (var entity in context.ChangeTracker.Entries())
            {
                entity.Reload();
            }
        }

        private void ShowCartPrice()
        {
            if (CartItems.Count > 0)
            {
                price = 0;
                for (int i = 0; i < CartItems.Count(); i++)
                {
                    price += CartItems[i].OrderedProductPrice * CartItems[i].OrderedQuantity;
                }
                IsCartEmpty = Visibility.Collapsed;
                CartPrice = price;
            }
            else
            {
                IsCartEmpty = Visibility.Visible;
                CartPrice = 0;
            }
        }

        public ICommand AddQuantityCommand { get; }
        private bool CanAddQuantityCommandExecute(object p) => SelectedItemQuantity > SelectedQuantity;
        private void OnAddQuantityCommandExecuted(object p) => SelectedQuantity++;

        public ICommand RemoveQuantityCommand { get; }
        private bool CanRemoveQuantityCommandExecute(object p) => SelectedQuantity > 0;
        private void OnRemoveQuantityCommandExecuted(object p) => SelectedQuantity--;

        public ICommand AddToCartCommand { get; }
        private bool CanAddToCartCommandExecute(object p) => SelectedQuantity > 0;
        private void OnAddToCartCommandExecuted(object p)
        {
            var orderId = Order.OrderId;
            int elementCount = context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == orderId && context.PRODUCTS_DETAIL.FirstOrDefault(t => t.LinkToProductId.Equals(this.SelectedProductItem.ProductId)).ProductCode == f.OrderedProductId).Count();
            var element = context.ORDERED_PRODUCTS.FirstOrDefault(f => f.LinkToOrderId == orderId && context.PRODUCTS_DETAIL.FirstOrDefault(t => t.LinkToProductId.Equals(this.SelectedProductItem.ProductId)).ProductCode == f.OrderedProductId);
            int previousQuantity = 0;
            if (elementCount > 0)
            {
                previousQuantity = element.OrderedQuantity;
                context.ORDERED_PRODUCTS.Remove(element);
            }
            ORDERED_PRODUCTS newelement = new ORDERED_PRODUCTS(context.PRODUCTS_DETAIL.FirstOrDefault(f => f.LinkToProductId.Equals(this.SelectedProductItem.ProductId)).ProductCode, SelectedQuantity + previousQuantity, orderId, (float)SelectedProductItem.Price);
            context.ORDERED_PRODUCTS.Add(newelement); context.SaveChanges();
            context.PRODUCTS.FirstOrDefault(f => f.ProductId.Equals(SelectedProductItem.ProductId)).Quantity -= SelectedQuantity; context.SaveChanges();

            if (context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId.Equals(orderId)).Count() > 0)
                IsCartEmpty = Visibility.Collapsed;
            else
                IsCartEmpty = Visibility.Visible;

            CartItems = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == orderId));

            ShowCartPrice();
            ShowProductsDetail();
        }

        public ICommand SearchCodeCommand { get; }
        private bool CanSearchCodeCommandExecute(object p) => context.PRODUCTS.Count() > 0;
        private void OnSearchCodeCommandExecuted(object p)
        {
            if (SearchProductCode is string && SearchProductCode != "")
            {
                if (!Regex.IsMatch(SearchProductCode, @"\D+"))
                {
                    int search = Convert.ToInt32(SearchProductCode);
                    var productDetail = new ObservableCollection<PRODUCTS_DETAIL>(context.PRODUCTS_DETAIL.Where(f => f.ProductCode.Equals(search)));

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
                else MessageBox.Show("Неверный формат кода" + "\n" + "продукта - код должен" + "\n" + "содержать только цифры.");
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

        public ICommand OpenCartCommand { get; }
        private bool CanOpenCartCommandExecute(object p)
        {
            bool flag = true;

            flag = context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == Order.OrderId).Count() > 0 && CartItems.Count() > 0;

            return flag;
        }
        private void OnOpenCartCommandExecuted(object p)
        {
            var model = new CartWindowViewModel(Order, Account);
            var cartWindow = new CartWindow(model);
            cartWindow.ShowDialog();

            if (!cartWindow.IsActive)
            {
                RefreshAll(); var orderId = Order.OrderId;

                var IsOrder = context.ORDERS.FirstOrDefault(f => f.OrderId == orderId).Adress;
                if (IsOrder != null)
                {
                    CartItems.Clear();
                    ORDERS order = new ORDERS(Account.AccountId); context.ORDERS.Add(order); context.SaveChanges();
                    var orderElement = context.ORDERS.FirstOrDefault(f => f.OrderId == order.OrderId);
                    orderElement.Status = "Пустой заказ"; context.SaveChanges();
                }
                else
                    CartItems = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == orderId));

                ShowCartPrice();
            }
        }

        public ICommand AddReviewCommand { get; }
        private void OnAddReviewCommandExecuted(object p)
        {
            var model = new ReviewWindowViewModel(SelectedProductItem, Account);
            var reviewWindow = new ReviewWindow();
            reviewWindow.DataContext = model;

            reviewWindow.ShowDialog();
            ShowProductsDetail();
        }

        public ICommand OpenMyOrdersCommand { get; }
        private void OnOpenMyOrdersCommandExecuted(object p)
        {
            var model = new MyOrdersWindowViewModel(Account);
            var myOrders = new MyOrdersWindow();
            myOrders.DataContext = model;

            myOrders.ShowDialog();
        }

        public ICommand OpenCouponsCommand { get; }
        private void OnOpenCouponsCommandExecuted(object p)
        {
            var couponsWindow = new CouponsWindow();

            couponsWindow.ShowDialog();
        }

        public ICommand OpenReviewsCommand { get; }
        private void OnOpenReviewsCommandExecuted(object p)
        {
            var model = new MyReviewsWindowViewModel(Account);
            var reviewsWindow = new MyReviewsWindow();
            reviewsWindow.DataContext = model;

            reviewsWindow.ShowDialog();
        }

        public ICommand LogOutCommand { get; }
        private void OnLogOutCommandExecuted(object p)
        {
            MessageBoxButton buttons = MessageBoxButton.YesNo;

            var result = MessageBox.Show("Вы точно хотите выйти из" + "\n" + "своего аккаунта?", "Выход из приложения", buttons, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var window = Application.Current.Windows[0];
                var authorizationWindow = new AuthorizationWindow();

                window.Close();
                authorizationWindow.Show();
            }
            else return;
        }

        public MainWindowViewModel(PERSONAL_ACCOUNTS accountParameter, ORDERS order)
        {
            this.Account = accountParameter;
            this.Order = order;

            AccountLogin = context.USERS.FirstOrDefault(f => f.LinkAccountId == accountParameter.AccountId).Login;

            CategoriesList = new ObservableCollection<CATEGORIES>(context.CATEGORIES);
            CartItems = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == Order.OrderId));

            ShowCartPrice();

            AddQuantityCommand = new DelegateCommand(OnAddQuantityCommandExecuted, CanAddQuantityCommandExecute);
            RemoveQuantityCommand = new DelegateCommand(OnRemoveQuantityCommandExecuted, CanRemoveQuantityCommandExecute);
            AddToCartCommand = new DelegateCommand(OnAddToCartCommandExecuted, CanAddToCartCommandExecute);
            SearchCodeCommand = new DelegateCommand(OnSearchCodeCommandExecuted, CanSearchCodeCommandExecute);
            SortAscCommand = new DelegateCommand(OnSortAscCommandExecuted, CanSortAscCommandExecute);
            SortDescCommand = new DelegateCommand(OnSortDescCommandExecuted, CanSortDescCommandExecute);
            SortAlphabetCommand = new DelegateCommand(OnSortAlphabetCommandExecuted, CanSortAlphabetCommandExecute);
            OpenCartCommand = new DelegateCommand(OnOpenCartCommandExecuted, CanOpenCartCommandExecute);
            AddReviewCommand = new DelegateCommand(OnAddReviewCommandExecuted);
            OpenMyOrdersCommand = new DelegateCommand(OnOpenMyOrdersCommandExecuted);
            OpenCouponsCommand = new DelegateCommand(OnOpenCouponsCommandExecuted);
            OpenReviewsCommand = new DelegateCommand(OnOpenReviewsCommandExecuted);
            LogOutCommand = new DelegateCommand(OnLogOutCommandExecuted);
        }

        public MainWindowViewModel() { }
    }
}