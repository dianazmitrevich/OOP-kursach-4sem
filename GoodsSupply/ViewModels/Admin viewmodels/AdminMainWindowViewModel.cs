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
        private Visibility isCategoryEmpty = Visibility.Collapsed;

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
        public Visibility IsCategoryEmpty
        {
            get => isCategoryEmpty;
            set => Set(ref isCategoryEmpty, value);
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
            if (ProductsList.Count == 0)
                IsCategoryEmpty = Visibility.Visible;
            else IsCategoryEmpty = Visibility.Collapsed;
        }
        private void ShowDetail()
        {
            if (SelectedProductItem != null)
                ProductsDetail = new ObservableCollection<PRODUCTS_DETAIL>(context.PRODUCTS_DETAIL.Where(f => f.LinkToProductId.Equals(SelectedProductItem.ProductId)));
        }
        private bool ValidationCheck()
        {
            bool flag = true;

            var elementCategory = CategoriesList.FirstOrDefault(f => f.CategoryId.Equals(SelectedItem.CategoryId));
            var element = ProductsList.FirstOrDefault(f => f.ProductId.Equals(SelectedProductItem.ProductId));
            var elementDetail = ProductsDetail;

            if (elementCategory != null && element != null && elementDetail != null)
            {
                if (elementCategory.Name.Length <= 100 && elementCategory.Name != "")
                {
                    if (element.Name.Length <= 100 && element.Name != "")
                    {
                        if (element.Description.Length <= 100 && element.Description != "")
                        {
                            if (element.Price != 0 && element.Price > 0)
                            {
                                if (element.Quantity >= 0)
                                {
                                    if (elementDetail[0].ProductCode.ToString().Length == 6)
                                    {
                                        if (elementDetail[0].Material.Length <= 100 && elementDetail[0].Material != "")
                                        {
                                            if (elementDetail[0].Package.Length <= 100 && elementDetail[0].Package != "")
                                            {
                                                if (elementDetail[0].Size.Length <= 100 && elementDetail[0].Size != "")
                                                {
                                                    if (elementDetail[0].BigDescription.Length <= 200 && elementDetail[0].BigDescription != "")
                                                        flag = true;
                                                }
                                                else flag = false;
                                            }
                                            else flag = false;
                                        }
                                        else flag = false;
                                    }
                                    else flag = false;
                                }
                                else flag = false;
                            }
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

        private bool CanSaveChangesCommandExecute(object p) => ValidationCheck();
        public ICommand SaveCChangesCommand { get; }
        private void OnSaveCChangesCommandExecuted(object p)
        {
            context.SaveChanges();
            MessageBox.Show("Данные успешно" + "\n" + "обновлены");
        }
        public ICommand SavePChangesCommand { get; }
        private void OnSavePChangesCommandExecuted(object p)
        {
            context.SaveChanges();
            MessageBox.Show("Данные успешно" + "\n" + "обновлены");
        }

        public ICommand AddCategoryCommand { get; }
        private bool CanAddCategoryCommandExecute(object p)
        {
            bool flag = true;
            var categories = new ObservableCollection<CATEGORIES>(context.CATEGORIES);
            if (categories != null)
            {
                foreach (var item in categories)
                {
                    if (item.Name.Length == 0)
                        flag = false;
                    else if (context.CATEGORIES.FirstOrDefault(f => f.Name.Equals("Новая категория. Нажмите для редактирования")) != null)
                        flag = false;
                }
            }
            return flag;
        }
        private void OnAddCategoryCommandExecuted(object p)
        {
            CATEGORIES element = new CATEGORIES("Новая категория. Нажмите для редактирования");
            context.CATEGORIES.Add(element); context.SaveChanges();

            CategoriesList = new ObservableCollection<CATEGORIES>(context.CATEGORIES);
        }

        public ICommand AddProductCommand { get; }
        private bool CanAddProductCommandExecute(object p)
        {
            bool flag = true;
            var categories = new ObservableCollection<CATEGORIES>(context.CATEGORIES);
            var products = new ObservableCollection<PRODUCTS>(context.PRODUCTS);
            var productDetail = new ObservableCollection<PRODUCTS_DETAIL>(context.PRODUCTS_DETAIL);
            if (categories.Count > 0)
            {
                foreach (var item in categories)
                {
                    if (item.Name.Length == 0)
                        flag = false;
                    else if (context.CATEGORIES.FirstOrDefault(f => f.Name.Equals("Новая категория. Нажмите для редактирования")) != null)
                        flag = false;
                }

                if (products != null && productDetail != null)
                {
                    foreach (var item in products)
                    {
                        if (item.Name.Length == 0) flag = false;
                        else if (item.Description.Length == 0) flag = false;
                        else if (item.Price <= 0) flag = false;
                        else if (item.Quantity < 0) flag = false;
                        else if (context.PRODUCTS.FirstOrDefault(f => f.Name.Equals("Новый товар. Нажмите для редактирования")) != null)
                            flag = false;
                    }
                    foreach (var item in productDetail)
                    {
                        if (item.ProductCode.ToString().Length == 0) flag = false;
                        else if (item.Material.Length == 0) flag = false;
                        else if (item.Package.Length == 0) flag = false;
                        else if (item.Size.Length == 0) flag = false;
                        else if (item.BigDescription.Length == 0) flag = false;
                    }
                }
            }
            else flag = false;
            return flag;
        }
            //context.PRODUCTS.FirstOrDefault(f => f.Name.Equals("Новый товар. Нажмите для редактирования")) == null;
        private void OnAddProductCommandExecuted(object p)
        {
            PRODUCTS element = new PRODUCTS(SelectedItem.CategoryId, "Новый товар. Нажмите для редактирования", "Описание для анонса длиной в 100 символов", 0, 0);
            context.PRODUCTS.Add(element); context.SaveChanges();

            PRODUCTS_DETAIL elementDetail = new PRODUCTS_DETAIL(element.ProductId, 000000, "Описание материала продукта длиной в 100 символов", "Описание упаковки продукта длиной в 100 символов", "Описание размера продукта длиной в 100 символов", "Детальное описание продукта длиной в 200 символов");
            context.PRODUCTS_DETAIL.Add(elementDetail); context.SaveChanges();

            ProductsList = new ObservableCollection<PRODUCTS>(context.PRODUCTS.Where(f => f.LinkToCategoryId.Equals(SelectedItem.CategoryId)));
            if (ProductsList.Count > 0)
                IsCategoryEmpty = Visibility.Collapsed;
            SelectedProductItem = element;
            ShowDetail();
        }

        public AdminMainWindowViewModel()
        {
            CategoriesList = new ObservableCollection<CATEGORIES>(context.CATEGORIES);

            SaveCChangesCommand = new DelegateCommand(OnSaveCChangesCommandExecuted, CanSaveChangesCommandExecute);
            SavePChangesCommand = new DelegateCommand(OnSavePChangesCommandExecuted, CanSaveChangesCommandExecute);
            AddCategoryCommand = new DelegateCommand(OnAddCategoryCommandExecuted, CanAddCategoryCommandExecute);
            AddProductCommand = new DelegateCommand(OnAddProductCommandExecuted, CanAddProductCommandExecute);
        }
    }
}
