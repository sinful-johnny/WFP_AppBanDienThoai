using DocumentFormat.OpenXml.Office2010.Excel;
using HW4.BUS;
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
using System.Windows.Shapes;

namespace HW4.UI.Customers
{
    /// <summary>
    /// Interaction logic for DataViewDialog.xaml
    /// </summary>
    public partial class DataViewDialog : Window
    {
        SqlConnection _con;
        DataTable data;
        int _mode;
        int _CUS_ID;
        public DataViewDialog(SqlConnection connection, int CUS_ID, int mode)
        {
            InitializeComponent();
            _con = connection;

            this._mode = mode;
            this._CUS_ID = CUS_ID;
            LoadData();
        }

        private void LoadData()
        {
            if (_mode == Mode.Promotions)
            {
                this.data = BUS_Customer.getCustomerPromos(_con, _CUS_ID);
            }
            else if (_mode == Mode.Orders)
            {
                this.data = BUS_Customer.getCustomerOders(_con, _CUS_ID);
                RevokePromoMenuClick.IsEnabled = false;
                RenewPromoMenuClick.IsEnabled = false;
            }
            DataGridView.ItemsSource = this.data.DefaultView;
        }

        private void RevokePromoMenuClick_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = (DataRowView)DataGridView.SelectedItem;
                var promoId = (int)selectedRow.Row.ItemArray[1]!;
                BUS_Customer.revokeCustomerPromo(_con, _CUS_ID, promoId);
                LoadData();
            }catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void RenewPromoMenuClick_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = (DataRowView)DataGridView.SelectedItem;
                var promoId = (int)selectedRow.Row.ItemArray[1]!;
                BUS_Customer.renewCustomerPromo(_con, _CUS_ID, promoId);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal class Mode
        {
            static public int Orders { get { return 1; } }
            static public int Promotions { get { return 2; } }
        }
    }
    
}
