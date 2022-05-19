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
    partial class AdminOrdersWindowViewModel : BaseViewModel
    {
        private readonly GoodsSupplyContext context = new GoodsSupplyContext();

        private ObservableCollection<ORDERS> ordersList = null;
        private Visibility isOrdersEmpty = Visibility.Collapsed;
        private Visibility isOrdersNotEmpty = Visibility.Visible;
        private ORDERS selectedOrder = null;

        public ObservableCollection<ORDERS> OrdersList
        {
            get => ordersList;
            set => Set(ref ordersList, value);
        }
        public Visibility IsOrdersEmpty
        {
            get => isOrdersEmpty;
            set => Set(ref isOrdersEmpty, value);
        }
        public Visibility IsOrdersNotEmpty
        {
            get => isOrdersNotEmpty;
            set => Set(ref isOrdersNotEmpty, value);
        }
        public ORDERS SelectedOrder
        {
            get => selectedOrder;
            set
            {
                Set(ref selectedOrder, value);
            }
        }

        private void IsOrdersListEmpty()
        {
            if (OrdersList.Count() > 0)
            {
                IsOrdersEmpty = Visibility.Collapsed;
                IsOrdersNotEmpty = Visibility.Visible;
            }
            else
            {
                IsOrdersEmpty = Visibility.Visible;
                IsOrdersNotEmpty = Visibility.Collapsed;
            }
        }

        public ICommand DeleteOrderCommand { get; }
        private bool CanDeleteOrderCommandExecute(object p) => SelectedOrder != null;
        private void OnDeleteOrderCommandExecuted(object p)
        {
            var order = context.ORDERS.FirstOrDefault(f => f.OrderId == SelectedOrder.OrderId);
            MessageBoxButton buttons = MessageBoxButton.YesNo;

            if (order != null)
            {
                var result = MessageBox.Show("Вы точно хотите удалить этот" + "\n" + "заказ?", "Удаление заказа", buttons, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var orderedProducts = new ObservableCollection<ORDERED_PRODUCTS>(context.ORDERED_PRODUCTS.Where(f => f.LinkToOrderId == order.OrderId));

                    if (orderedProducts != null && orderedProducts.Count > 0)
                    {
                        foreach (var item in orderedProducts)
                            context.ORDERED_PRODUCTS.Remove(item);
                    }

                    context.ORDERS.Remove(order); context.SaveChanges();
                    MessageBox.Show("Заказ успешно" + "\n" + "удален");

                    OrdersList = new ObservableCollection<ORDERS>(context.ORDERS);
                    IsOrdersListEmpty();
                }
                else return;
            }
        }

        public ICommand UpdateOrderCommand { get; }
        private bool CanUpdateOrderCommandExecute(object p)
        {
            bool flag = true;

            if (SelectedOrder != null)
            {
                var order = context.ORDERS.FirstOrDefault(f => f.OrderId == SelectedOrder.OrderId);
                if (order != null)
                {
                    if (order.Status.Length > 0 && order.Status.Length <= 100 && order.Status != null && order.Status != "")
                        flag = true;
                    else flag = false;
                }
                else flag = false;
            }
            else flag = false;

            return flag;
        }
        private void OnUpdateOrderCommandExecuted(object p)
        {
            var order = context.ORDERS.FirstOrDefault(f => f.OrderId == SelectedOrder.OrderId);

            if (order != null)
            {
                context.SaveChanges();
                MessageBox.Show("Статус заказа" + "\n" + "обновлен");
            }
        }

            public AdminOrdersWindowViewModel()
        {
            OrdersList = new ObservableCollection<ORDERS>(context.ORDERS);

            IsOrdersListEmpty();

            DeleteOrderCommand = new DelegateCommand(OnDeleteOrderCommandExecuted, CanDeleteOrderCommandExecute);
            UpdateOrderCommand = new DelegateCommand(OnUpdateOrderCommandExecuted, CanUpdateOrderCommandExecute);
        }

    }
}
