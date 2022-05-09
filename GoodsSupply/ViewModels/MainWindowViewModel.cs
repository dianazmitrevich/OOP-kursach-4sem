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
        private CATEGORIES selectedItem;
        private PRODUCTS selectedProductItem;
        private Visibility noCategoryselectedFlag = Visibility.Visible;
        private Visibility noProductSelectedFlag = Visibility.Visible;
        private Visibility isCategorySelectedFlag = Visibility.Hidden;
        private Visibility isProductSelectedFlag = Visibility.Collapsed;

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
        }
        void ShowProductsDetail()
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
            IsProductSelectedFlag = Visibility.Visible;
            NoProductSelectedFlag = Visibility.Collapsed;
        }

        public MainWindowViewModel(PERSONAL_ACCOUNTS accountParameter)
        {
            this.Account = accountParameter;
            CategoriesList = new ObservableCollection<CATEGORIES>(context.CATEGORIES);
        }

        public MainWindowViewModel()
        {
            CategoriesList = new ObservableCollection<CATEGORIES>(context.CATEGORIES);
        }
    }
}