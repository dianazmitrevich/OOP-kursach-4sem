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
    class MainWindowViewModel : BaseViewModel
    {
        private readonly GoodsSupplyContext context = new GoodsSupplyContext();

        private ObservableCollection<CATEGORIES> categoriesList;
        private ObservableCollection<PRODUCTS> productsList = null;
        private CATEGORIES selectedItemIndex;
        private Visibility selectedCategoryFlag = Visibility.Visible;

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
        public CATEGORIES SelectedItemIndex
        {
            get => selectedItemIndex;
            set
            {
                Set(ref selectedItemIndex, value);
                ShowProducts();
            }
        }
        public Visibility SelectedCategoryFlag
        {
            get => selectedCategoryFlag;
            set => Set(ref selectedCategoryFlag, value);
        }

        public void ShowProducts()
        {
            ProductsList = new ObservableCollection<PRODUCTS>(context.PRODUCTS.Where(f => f.LinkToCategoryId.Equals(selectedItemIndex.CategoryId)));
            SelectedCategoryFlag = Visibility.Collapsed;
        }
        public MainWindowViewModel()
        {
            categoriesList = new ObservableCollection<CATEGORIES>(context.CATEGORIES);
        }
    }
}