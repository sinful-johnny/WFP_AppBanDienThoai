using HW4.BUS;
using HW4.UI.Promotions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for GrantPromoDialog.xaml
    /// </summary>
    public partial class GrantPromoDialog : Window
    {
        SqlConnection _con;
        int _rowPerPage = 32;
        int _currentPage = 1;
        int _pageSize = 10;
        int totalPages = -1;
        int totalItems = -1;
        DataTable _promos;
        BindingList<Page> PageOptions = new BindingList<Page>();
        int CUS_ID;
        public GrantPromoDialog(SqlConnection connection, int CUS_ID)
        {
            InitializeComponent();
            _con = connection;
            this.CUS_ID = CUS_ID;
        }
        void LoadData()
        {
            try
            {
                int oldTotalPages = totalPages;
                (_promos, totalItems, totalPages) = PROMOTIONS_Control.GetAllPaging(_con, _currentPage, _rowPerPage);
                PromoDataGrid.ItemsSource = _promos.DefaultView;
                if (oldTotalPages != totalPages)
                {
                    PageOptions.Clear();
                    for (int i = 1; i <= totalPages; i++)
                    {
                        PageOptions.Add(new Page()
                        {
                            _pageNo = i,
                            _totalPages = totalPages
                        });
                    }
                    PageSelectComboBox.ItemsSource = PageOptions;
                    PageSelectComboBox.SelectedIndex = 0;
                }

                if (_currentPage == 1)
                {
                    PrevButton.IsEnabled = false;
                }
                else
                {
                    PrevButton.IsEnabled = true;
                }

                if (_currentPage == totalPages)
                {
                    NextButton.IsEnabled = false;
                }
                else
                {
                    NextButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void PageSelectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PageSelectComboBox.SelectedItem != null && _currentPage != ((Page)PageSelectComboBox.SelectedItem)._pageNo)
            {
                _currentPage = ((Page)PageSelectComboBox.SelectedItem)._pageNo;
                LoadData();
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            PageSelectComboBox.SelectedItem = PageOptions.FirstOrDefault(x => x._pageNo == _currentPage - 1);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            PageSelectComboBox.SelectedItem = PageOptions.FirstOrDefault(x => x._pageNo == _currentPage + 1);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            PageSelectComboBox.SelectedIndex = 0;
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var row = (DataRowView)PromoDataGrid.SelectedItem;
                var promoId = (int)row.Row.ItemArray[0]!;
                BUS_Customer.addCustomerPromo(_con, CUS_ID, promoId);
                DialogResult = true;
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(),"Error",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
