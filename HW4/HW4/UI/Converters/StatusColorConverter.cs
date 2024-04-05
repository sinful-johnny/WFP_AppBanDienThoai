using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
namespace HW4.UI.Converters
{
    internal class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString().Contains("Pending"))
            {
                return Brushes.Yellow;
            }
            if (value != null && value.ToString().Contains("Delivered"))
            {
                return Brushes.Green;
            }
            if (value != null && value.ToString().Contains("Cancelled"))
            {
                return Brushes.Red;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
