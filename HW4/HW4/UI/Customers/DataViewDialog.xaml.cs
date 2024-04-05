using System;
using System.Collections.Generic;
using System.Data;
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

namespace HW4.UI.Customers
{
    /// <summary>
    /// Interaction logic for DataViewDialog.xaml
    /// </summary>
    public partial class DataViewDialog : Window
    {
        DataTable data;
        public DataViewDialog(DataTable data)
        {
            InitializeComponent();
            this.data = data;
            DataGridView.ItemsSource = this.data.DefaultView;
        }
    }
}
