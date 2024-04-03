using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HW4
{
    /// <summary>
    /// Interaction logic for PromoManagementUserControl.xaml
    /// </summary>
    public partial class PromoManagementUserControl : UserControl
    {
        SqlConnection _con;
        int _rowPerPage = 32;
        int _currentPage = 1;
        int _pageSize = 10;
        int totalPages = -1;
        int totalItems = -1;
        DataTable _promos;
        public PromoManagementUserControl(SqlConnection connection)
        {
            InitializeComponent();
            _con = connection;
        }
        void LoadData()
        {
            try
            {
                (_promos, totalItems, totalPages) = PROMOTIONS_Control.GetAllPaging(_con, _currentPage, _rowPerPage);
            }catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();

            PromoDataGrid.ItemsSource = _promos.DefaultView;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if(_con.State == ConnectionState.Open)
            {
                _con.Close();
            }
        }
    }
}
