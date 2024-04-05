using HW4.UI.Promotions;
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

        public enum PromotionManagementActions
        {
            AddPromo,
            EditPromo,
            DeletePromo,
            SeeAvailablePromo
        }
        public void HandleParentEvent(PromotionManagementActions action)
        {
            switch (action)
            {
                case PromotionManagementActions.AddPromo:
                    AddPromotion();
                    break;
                case PromotionManagementActions.EditPromo:
                    EditPromotion();
                    break;
                case PromotionManagementActions.DeletePromo:
                    break;
                case PromotionManagementActions.SeeAvailablePromo:
                    break;
            }
        }

        void LoadData()
        {
            try
            {
                (_promos, totalItems, totalPages) = PROMOTIONS_Control.GetAllPaging(_con, _currentPage, _rowPerPage);
                PromoDataGrid.ItemsSource = _promos.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if(_con.State == ConnectionState.Open)
            {
                _con.Close();
            }
        }

        private void AddPromotion()
        {
            var screen = new PromoInfoDialog(_con);
            screen.ShowDialog();
        }

        private void EditPromotion()
        {
            if(PromoDataGrid.SelectedItems.Count != 0)
            {
                var selectRow = (DataRowView)PromoDataGrid.SelectedItems[0]!;
                var screen = new PromoInfoDialog(_con, selectRow);
                screen.ShowDialog();
                LoadData();
            }
            else
            {
                MessageBox.Show("Choose an item to edit!","Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }
}
