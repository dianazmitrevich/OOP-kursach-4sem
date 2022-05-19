using GoodsSupply.Commands;
using GoodsSupply.Models;
using GoodsSupply.Views;
using GoodsSupply.Views.Admin_views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        #region private variables
        private readonly GoodsSupplyContext context = new GoodsSupplyContext();

        private ObservableCollection<CATEGORIES> categoriesList;
        private ObservableCollection<PRODUCTS> productsList = null;
        private CATEGORIES selectedItem = null;
        private ObservableCollection<PRODUCTS_DETAIL> productsDetail = null;
        private PRODUCTS selectedProductItem = null;
        private Visibility isCategoryEmpty = Visibility.Collapsed;
        private Visibility isProductSelected = Visibility.Collapsed;
        private Visibility isCategorySelected = Visibility.Visible;
        private Visibility isCategorySelectedToEdit = Visibility.Collapsed;
        private Visibility isOrdersEmpty = Visibility.Visible;
        private Visibility orderCountLabel = Visibility.Collapsed;
        private string activeOrdersCount;
        private string notAnsweredReviewsCount;
        #endregion

        #region public variables
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
                IsProductSelected = Visibility.Collapsed;
                IsCategorySelectedToEdit = Visibility.Visible;
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
        public Visibility IsOrdersEmpty
        {
            get => isOrdersEmpty;
            set => Set(ref isOrdersEmpty, value);
        }
        public Visibility OrderCountLabel
        {
            get => orderCountLabel;
            set => Set(ref orderCountLabel, value);
        }
        public string ActiveOrdersCount
        {
            get => activeOrdersCount;
            set => Set(ref activeOrdersCount, value);
        }
        public string NotAnsweredReviewsCount
        {
            get => notAnsweredReviewsCount;
            set => Set(ref notAnsweredReviewsCount, value);
        }
        public Visibility IsProductSelected
        {
            get => isProductSelected;
            set => Set(ref isProductSelected, value);
        }
        public Visibility IsCategorySelected
        {
            get => isCategorySelected;
            set => Set(ref isCategorySelected, value);
        }
        public Visibility IsCategorySelectedToEdit
        {
            get => isCategorySelectedToEdit;
            set => Set(ref isCategorySelectedToEdit, value);
        }
        public PRODUCTS SelectedProductItem
        {
            get => selectedProductItem;
            set
            {
                Set(ref selectedProductItem, value);
                ShowDetail();
                IsProductSelected = Visibility.Visible;
            }
        }
        #endregion

        private void ShowProducts()
        {
            if (SelectedItem != null)
            {
                int categoryId = SelectedItem.CategoryId;
                ProductsList = new ObservableCollection<PRODUCTS>(context.PRODUCTS.Where(f => f.LinkToCategoryId.Equals(categoryId)));
                IsCategorySelected = Visibility.Collapsed;

                if (ProductsList.Count == 0)
                    IsCategoryEmpty = Visibility.Visible;
                else
                {
                    IsCategoryEmpty = Visibility.Collapsed;
                    IsCategorySelected = Visibility.Collapsed;
                }
            }
        }
        private void ShowDetail()
        {
            if (SelectedProductItem != null)
                ProductsDetail = new ObservableCollection<PRODUCTS_DETAIL>(context.PRODUCTS_DETAIL.Where(f => f.LinkToProductId.Equals(SelectedProductItem.ProductId)));
        }
        private bool ValidationCheck()
        {
            bool flag = true;

            if (SelectedItem != null)
            {
                if (SelectedProductItem != null)
                {
                    var elementCategory = CategoriesList.FirstOrDefault(f => f.CategoryId.Equals(SelectedItem.CategoryId));
                    var element = ProductsList.FirstOrDefault(f => f.ProductId.Equals(SelectedProductItem.ProductId));
                    var elementDetail = ProductsDetail;

                    if (elementCategory != null && element != null && elementDetail.Count > 0)
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
                    }
                    else flag = false;
                }
                else flag = false;
            }
            else flag = false;
            return flag;
        }

        private void SetOrdersCount()
        {
            var orders = context.ORDERS.Count();

            if (orders > 0)
            {
                double sum = 0;
                var ordersList = new ObservableCollection<ORDERS>(context.ORDERS);

                if (ordersList != null)
                {
                    foreach (var item in ordersList)
                        sum += item.FinalOrderPrice;
                }

                ActiveOrdersCount = $"{orders} активных заказов на сумму {sum} рублей";
                IsOrdersEmpty = Visibility.Collapsed;
                OrderCountLabel = Visibility.Visible;
            }
            else
            {
                IsOrdersEmpty = Visibility.Visible;
                OrderCountLabel = Visibility.Collapsed;
            }
        }

        private void ShowNotAnsweredReviewsCount() => NotAnsweredReviewsCount = context.REVIEWS.Where(f => f.AdminText.Equals("Пока еще нет ответа")).Count().ToString();

        public ICommand SaveCChangesCommand { get; }
        private bool CanSaveCChangesCommandExecute(object p)
        {
            bool flag = true;

            if (SelectedItem != null)
            {
                var elementCategory = CategoriesList.FirstOrDefault(f => f.CategoryId.Equals(SelectedItem.CategoryId));

                if (elementCategory != null)
                {
                    if (elementCategory.Name.Length <= 100 && elementCategory.Name != "")
                        flag = true;
                    else flag = false;
                }
                else flag = false;
            }
            else flag = false;
            return flag;

        }
        private void OnSaveCChangesCommandExecuted(object p)
        {
            context.SaveChanges();
            MessageBox.Show("Данные успешно" + "\n" + "обновлены");
        }

        public ICommand SavePChangesCommand { get; }
        private bool CanSavePChangesCommandExecute(object p) => ValidationCheck();
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
            SelectedItem = element;

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
                if (SelectedItem == null) flag = false;
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

        public ICommand DeleteCategoryCommand { get; }
        private bool CanDeleteCategoryCommandExecute(object p) => SelectedItem != null;
        private void OnDeleteCategoryCommandExecuted(object p)
        {
            var element = context.CATEGORIES.FirstOrDefault(f => f.CategoryId.Equals(SelectedItem.CategoryId));

            MessageBoxButton buttons = MessageBoxButton.YesNo;
            if (element != null)
            {
                var result = MessageBox.Show("Вы точно хотите удалить эту" + "\n" + "категорию? Будут удалены все" + "\n" + "товары, привязанные к ней", "Удаление категории", buttons, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var products = new ObservableCollection<PRODUCTS>(context.PRODUCTS.Where(f => f.LinkToCategoryId.Equals(SelectedItem.CategoryId)));
                    foreach (var item in products)
                    {
                        var detail = context.PRODUCTS_DETAIL.FirstOrDefault(f => f.LinkToProductId.Equals(item.ProductId));
                        context.PRODUCTS_DETAIL.Remove(detail);
                        context.PRODUCTS.Remove(item);

                        DirectoryInfo directory;
                        directory = new DirectoryInfo(@"..\..\..");
                        if (File.Exists(directory.FullName + $@"\GoodsSupply\ProductImages\{item.ProductId}.png"))
                            File.Delete(directory.FullName + $@"\GoodsSupply\ProductImages\{item.ProductId}.png");
                    }
                    context.CATEGORIES.Remove(element); context.SaveChanges();
                }
                else return;
            }

            CategoriesList = new ObservableCollection<CATEGORIES>(context.CATEGORIES);
            ProductsList = null;
            IsCategorySelected = Visibility.Visible; IsCategoryEmpty = Visibility.Collapsed;
        }

        public ICommand DeleteProductCommand { get; }
        private bool CanDeleteProductCommandExecute(object p) => SelectedProductItem != null;
        private void OnDeleteProductCommandExecuted(object p)
        {
            var element = context.PRODUCTS.FirstOrDefault(f => f.ProductId.Equals(SelectedProductItem.ProductId));

            if (element != null)
            {
                MessageBoxButton buttons = MessageBoxButton.YesNo;
                if (element != null)
                {
                    var result = MessageBox.Show("Вы точно хотите удалить этот" + "\n" + "товар? Будет удалена и его" + "\n" + "детальная страница", "Удаление товара", buttons, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        var detail = context.PRODUCTS_DETAIL.FirstOrDefault(f => f.LinkToProductId.Equals(SelectedProductItem.ProductId));
                        // var orderedProducts = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.OrderedProductId.Equals(detail.ProductCode)));
                        context.PRODUCTS_DETAIL.Remove(detail);
                        context.PRODUCTS.Remove(element);
                        var reviews = context.REVIEWS.Where(f => f.LinkToProductId.Equals(element.ProductId));
                        foreach (var item in reviews)
                        {
                            context.REVIEWS.Remove(item);
                        }

                        DirectoryInfo directory;
                        directory = new DirectoryInfo(@"..\..\..");
                        if (File.Exists(directory.FullName + $@"\GoodsSupply\ProductImages\{element.ProductId}.png"))
                            File.Delete(directory.FullName + $@"\GoodsSupply\ProductImages\{element.ProductId}.png");

                        context.SaveChanges();
                    }
                    else return;
                }
            }

            ProductsDetail.Clear();
            ProductsList = new ObservableCollection<PRODUCTS>(context.PRODUCTS.Where(f => f.LinkToCategoryId.Equals(SelectedItem.CategoryId)));
            if (ProductsList.Count > 0)
                IsCategoryEmpty = Visibility.Collapsed;
            else IsCategoryEmpty = Visibility.Visible;
        }

        public ICommand OpenCouponsWindowCommand { get; }
        private void OnOpenCouponsWindowCommandExecuted(object p)
        {
            var model = new AdminCouponsWindowViewModel();
            var couponsWindow = new AdminCouponsWindow();
            couponsWindow.DataContext = model;

            couponsWindow.ShowDialog();
        }

        public ICommand OpenReviewsWindowCommand { get; }
        private void OnOpenReviewsWindowCommandExecuted(object p)
        {
            var model = new AdminReviewsWindowViewModel();
            var reviewsWindow = new AdminReviewsWindow();
            reviewsWindow.DataContext = model;

            reviewsWindow.ShowDialog();

            if (!reviewsWindow.IsActive)
                ShowNotAnsweredReviewsCount();
        }

        public ICommand OpenOrdersWindowCommand { get; }
        private void OnOpenOrdersWindowCommandExecuted(object p)
        {
            var model = new AdminOrdersWindowViewModel();
            var ordersWindow = new AdminOrdersWindow();
            ordersWindow.DataContext = model;

            ordersWindow.ShowDialog();

            if (!ordersWindow.IsActive)
            {
                SetOrdersCount();
            }
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

        public ICommand AddImageCommand { get; }
        private bool CanAddImageCommandExecute(object p) => SelectedProductItem != null;
        private void OnAddImageCommandExecuted(object p)
        {
            DirectoryInfo directory;
            directory = new DirectoryInfo(@"..\..\..");

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Фотографии|*.jpg;*.png;*.jpeg;";

            if (openFileDialog.ShowDialog() == true)
            {
                var detail = SelectedProductItem;
                var path = openFileDialog.FileName;

                if (File.Exists(directory.FullName + $@"\GoodsSupply\ProductImages\{SelectedProductItem.ProductId}.png"))
                    File.Delete(directory.FullName + $@"\GoodsSupply\ProductImages\{SelectedProductItem.ProductId}.png");

                File.Copy(path, directory.FullName + $@"\GoodsSupply\ProductImages\{SelectedProductItem.ProductId}.png", true);

                ProductsList = new ObservableCollection<PRODUCTS>(context.PRODUCTS.Where(f => f.LinkToCategoryId == SelectedItem.CategoryId));
                SelectedProductItem = detail;
            }
        }

        public AdminMainWindowViewModel()
        {
            CategoriesList = new ObservableCollection<CATEGORIES>(context.CATEGORIES);
            SetOrdersCount(); ShowNotAnsweredReviewsCount();

            SaveCChangesCommand = new DelegateCommand(OnSaveCChangesCommandExecuted, CanSaveCChangesCommandExecute);
            SavePChangesCommand = new DelegateCommand(OnSavePChangesCommandExecuted, CanSavePChangesCommandExecute);
            AddCategoryCommand = new DelegateCommand(OnAddCategoryCommandExecuted, CanAddCategoryCommandExecute);
            AddProductCommand = new DelegateCommand(OnAddProductCommandExecuted, CanAddProductCommandExecute);
            DeleteCategoryCommand = new DelegateCommand(OnDeleteCategoryCommandExecuted, CanDeleteCategoryCommandExecute);
            DeleteProductCommand = new DelegateCommand(OnDeleteProductCommandExecuted, CanDeleteProductCommandExecute);
            OpenCouponsWindowCommand = new DelegateCommand(OnOpenCouponsWindowCommandExecuted);
            OpenReviewsWindowCommand = new DelegateCommand(OnOpenReviewsWindowCommandExecuted);
            OpenOrdersWindowCommand = new DelegateCommand(OnOpenOrdersWindowCommandExecuted);
            LogOutCommand = new DelegateCommand(OnLogOutCommandExecuted);
            AddImageCommand = new DelegateCommand(OnAddImageCommandExecuted, CanAddImageCommandExecute);
        }
    }
}
