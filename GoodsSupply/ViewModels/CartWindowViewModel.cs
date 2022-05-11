using GoodsSupply.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodsSupply.ViewModels
{
    class CartWindowViewModel : BaseViewModel
    {
        private readonly GoodsSupplyContext context = new GoodsSupplyContext();

        private ObservableCollection<ORDERED_PRODUCTS> orderedProductsList = null;
        private string orderedProductsName;

        public ObservableCollection<ORDERED_PRODUCTS> OrderedProductsList
        {
            get => orderedProductsList;
            set => Set(ref orderedProductsList, value);
        }
        public string OrderedProductsName
        {
            get => orderedProductsName;
            set => Set(ref orderedProductsName, value);
        }

        public CartWindowViewModel()
        {
            OrderedProductsList = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == 3));
            OrderedProductsName = "xdfcghijok";
        }
    }
}
