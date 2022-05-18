using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GoodsSupply.Views
{
    /// <summary>
    /// Логика взаимодействия для MyOrdersWindow.xaml
    /// </summary>
    public partial class MyOrdersWindow : Window
    {
        public MyOrdersWindow()
        {
            InitializeComponent();
        }
        public MyOrdersWindow(object dataContextViewModel)
        {
            DataContext = dataContextViewModel;
            InitializeComponent();
        }
    }
}
