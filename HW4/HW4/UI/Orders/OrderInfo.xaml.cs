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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HW4
{
    /// <summary>
    /// Interaction logic for OrderInfo.xaml
    /// </summary>
    public partial class OrderInfo : UserControl
    {
        ORDER getOrder;
        public OrderInfo()
        {
            InitializeComponent();
            getOrder = (ORDER)DataContext;
        }

        private void UpdatePhoneCount(object sender, RoutedEventArgs e)
        {

        }
        private void DeletePhone(object sender, RoutedEventArgs e)
        {

        }

        private void AddPhone(object sender, RoutedEventArgs e)
        {
            
        }

        private void AddPromo(object sender, RoutedEventArgs e)
        {

        }

        private void DeletePromo(object sender, RoutedEventArgs e)
        {

        }
    }
}
