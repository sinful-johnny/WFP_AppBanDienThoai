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

namespace HW4.UI.Manufacturers
{
    /// <summary>
    /// Interaction logic for PickAPhoneDialog.xaml
    /// </summary>
    public partial class PickAPhoneDialog : Window
    {
        DataTable _PhoneData;
        public PHONE? _selectedPhone;
        List<Page> PageOptions = new List<Page>();
        SqlConnection _conn;
        int _rowPerPage = 24;
        int _currentPage = 1;
        int _pageSize = 10;
        int totalPages = -1;
        int totalItems = -1;
        public PickAPhoneDialog(SqlConnection connection)
        {
            InitializeComponent();
            _conn = connection;
            LoadData(connection);
        }

        private void LoadData(SqlConnection connection)
        {
            int oldTotalPages = totalPages;
            (_PhoneData, totalItems, totalPages) = BUS_Phone.GetPhoneToDataTable(connection, _currentPage, _rowPerPage);
            PhoneDataGrid.ItemsSource = _PhoneData.DefaultView;
            if(oldTotalPages !=  totalPages)
            {
                for (int i = 1; i <= totalPages; i++)
                {
                    PageOptions.Add(new Page()
                    {
                        _pageNo = i,
                        _totalPages = totalPages
                    });
                }
                PageSelectComboBox.ItemsSource = PageOptions;
            }

            if(_currentPage == 1)
            {
                PrevButton.IsEnabled = false;
            }
            else
            {
                PrevButton.IsEnabled=true;
            }

            if(_currentPage == totalPages)
            {
                NextButton.IsEnabled = false;
            }
            else
            {
                NextButton.IsEnabled=true;
            }
        }

        private void PageSelectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(PageSelectComboBox.SelectedItem != null)
            {
                _currentPage = ((Page)PageSelectComboBox.SelectedItem)._pageNo;
                LoadData(_conn);
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
            PageSelectComboBox.SelectedIndex = 0;
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedRow = (DataRowView)PhoneDataGrid.SelectedItems[0]!;
            _selectedPhone = new PHONE()
            {
                ID = (int)selectedRow.Row.ItemArray[0]!,
                PhoneName = (string)selectedRow.Row.ItemArray[1]!,
                Price = double.Parse((string)selectedRow.Row.ItemArray[2]!),
                Thumbnail = (string)selectedRow.Row.ItemArray[3]!,
                Manufacturer = (string)selectedRow.Row.ItemArray[4]!
            };
            DialogResult = true;
        }

        private void NoneButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedPhone = null;
            DialogResult = false;
        }
    }
    internal class Page
    {
        public int _pageNo { get; set; }
        public int _totalPages { get; set; }

        public string Info { get { return $"{_pageNo}/{_totalPages}"; } }
    }
}
