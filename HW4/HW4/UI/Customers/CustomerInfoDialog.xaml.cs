using HW4.BUS;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for CustomerInfoDialog.xaml
    /// </summary>
    public partial class CustomerInfoDialog : Window
    {
        CUSTOMER _customer;
        SqlConnection _con;
        int _mode = Mode.Insert;
        public CustomerInfoDialog(SqlConnection conn)
        {
            InitializeComponent();
            _mode = Mode.Insert;
            _con = conn;
            _customer = new CUSTOMER() {
                Cus_ID = -1,
                DOB = DateTime.Now,
                Pfp = "./Images/blank-pfp.png"
            };
            DataContext = _customer;
        }
        public CustomerInfoDialog(SqlConnection conn, CUSTOMER info)
        {
            InitializeComponent();
            _mode = Mode.Update;
            _con = conn;
            _customer = info;
            DataContext = _customer;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(_mode ==  Mode.Insert)
            {
                var result = BUS_Customer.Insert(_con, _customer);
            }
            else if (_mode == Mode.Update)
            {
                var result = BUS_Customer.Update(_con, _customer);
            }
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
    internal class Mode
    {
        static public int Insert { get { return 1; } }
        static public int Update { get { return 2; } }
    }
}
