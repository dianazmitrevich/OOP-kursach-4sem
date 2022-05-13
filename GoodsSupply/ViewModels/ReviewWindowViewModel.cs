using GoodsSupply.Commands;
using GoodsSupply.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
                GetSymbols();
            }
        }
        public string ReviewTextSymbols
        {
            get => reviewTextSymbols;
            set => Set(ref reviewTextSymbols, value);
        }

        public void GetSymbols()
        {
            if (ReviewText != "")
            {
                ReviewTextSymbols = $"{ReviewText.Length}/200";
            }
        }

        public ICommand AddReviewCommand { get; }
        private bool CanAddReviewCommandExecute(object p) => ReviewText != "";
        private void OnAddReviewCommandExecuted(object p)
        {
            if (ReviewText.Length <= 200)
            {
                REVIEWS element = new REVIEWS();
                context.REVIEWS.Add(element); context.SaveChanges();
            }
        }

        public ReviewWindowViewModel(PRODUCTS productParameter)
        {
            this.Product = productParameter;
            ProductCode = context.PRODUCTS_DETAIL.FirstOrDefault(f => f.LinkToProductId.Equals(Product.ProductId)).ProductCode.ToString();
            ProductName = Product.Name;
            ReviewText = "Текст отзыва";
            ReviewTextSymbols = "0/200";
            GetSymbols();
        }

        public ReviewWindowViewModel()
        {
            AddReviewCommand = new DelegateCommand(OnAddReviewCommandExecuted, CanAddReviewCommandExecute);

        }
    }
}
