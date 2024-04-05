using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace HW4
{
    /// <summary>
    /// Interaction logic for PhoneOrder.xaml
    /// </summary>
    public partial class PhoneOrder : UserControl
    {
        BindingList<ORDER> _OrderList;
        int _rowPerPage = 12;
        int _currentPage = 1;
        int _pageSize = 10;
        int totalPages = -1;
        int totalItems = -1;
        private SqlConnection _connection;
        public PhoneOrder(SqlConnection con)
        {
            InitializeComponent();
            _connection = con;
            startDatePicker.SelectedDate = DateTime.Now;
            endDatePicker.SelectedDate = DateTime.Now;
        }

        public enum OrderManagementAction
        {
            AddOrder,
            DeleteOrder
        }
        private void AddOrder(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateOrder(object sender, RoutedEventArgs e)
        {

        }
        private void DeleteOrder(object sender, RoutedEventArgs e)
        {

        }

        private void Page(object sender, RoutedEventArgs e)
        {

        }
    }
}
