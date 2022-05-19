using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GoodsSupply.Assistants
{
    class ToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DirectoryInfo directory;
            directory = new DirectoryInfo(@"..\..\..");

            if (File.Exists(directory.FullName + $@"\GoodsSupply\ProductImages\{(int)value}.png"))
            {
                BitmapImage imgTemp = new BitmapImage();
                imgTemp.BeginInit();
                imgTemp.CacheOption = BitmapCacheOption.OnLoad;
                imgTemp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                imgTemp.UriSource = new Uri(directory.FullName + $@"\GoodsSupply\ProductImages\{(int)value}.png");
                imgTemp.EndInit();
                return imgTemp;
            }
            else
                return "/ProductImages/empty.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
