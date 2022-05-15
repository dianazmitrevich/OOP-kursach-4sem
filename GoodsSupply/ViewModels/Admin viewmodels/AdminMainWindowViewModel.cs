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
    class AdminMainWindowViewModel : BaseViewModel
    {
        private readonly GoodsSupplyContext context = new GoodsSupplyContext();

        private ObservableCollection<CATEGORIES> categoriesList;
        private ObservableCollection<PRODUCTS> productsList = null;
        private CATEGORIES selectedItem;
        private ObservableCollection<PRODUCTS_DETAIL> productsDetail = null;
        private PRODUCTS selectedProductItem;

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
        public ObservableCollection<PRODUCTS_DETAIL> ProductsDetail
        {
            get => productsDetail;
            set => Set(ref productsDetail, value);
        }
        public PRODUCTS SelectedProductItem
        {
            get => selectedProductItem;
            set
            {
                Set(ref selectedProductItem, value);
                ShowDetail();
            }
        }

        private void ShowProducts()
        {
            ProductsList = new ObservableCollection<PRODUCTS>(context.PRODUCTS.Where(f => f.LinkToCategoryId.Equals(SelectedItem.CategoryId)));
        }
        private void ShowDetail()
        {
            ProductsDetail = new ObservableCollection<PRODUCTS_DETAIL>(context.PRODUCTS_DETAIL.Where(f => f.LinkToProductId.Equals(SelectedProductItem.ProductId)));
        }

        public ICommand SaveChangesCommand { get; }
        private void OnAddQuantityCommandExecuted(object p)
        {
            context.SaveChanges();
            MessageBox.Show("Данные успешно" + "\n" + "обновлены");
        }

        public ICommand AddCategoryCommand { get; }
        private void OnAddCategoryCommandExecuted(object p)
        {
            context.SaveChanges();
            MessageBox.Show("Данные успешно" + "\n" + "обновлены");
        }

        public AdminMainWindowViewModel()
        {
            CategoriesList = new ObservableCollection<CATEGORIES>(context.CATEGORIES);

            SaveChangesCommand = new DelegateCommand(OnAddQuantityCommandExecuted);
            AddCategoryCommand = new DelegateCommand(OnAddCategoryCommandExecuted);
        }
    }
}
