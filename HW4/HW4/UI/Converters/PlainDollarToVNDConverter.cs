using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HW4.UI.Converters
{
    internal class PlainDollarToVNDConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double Dollar = (double)value;
            double vnd = Dollar * 24535;
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = ",";
            string result = vnd.ToString("#,0.00", nfi);
            if (double.IsNaN(vnd)) { return 0; }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
