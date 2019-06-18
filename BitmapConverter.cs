using Splat;
using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfLazyLoadImages
{
    public class BitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IBitmap bmp)
            {
                return bmp.ToNative();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
