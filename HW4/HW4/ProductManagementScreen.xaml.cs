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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HW4
{
    /// <summary>
    /// Interaction logic for ProductManagementScreen.xaml
    /// </summary>
    public partial class ProductManagementScreen : UserControl
    {
        BindingList<PHONE> _PhoneList;
        MANUFACTURERControl maNUFACTURERControl;
        int _rowPerPage = 10;
        int _currentPage = 1;
        int _pageSize = 10;
        int totalPages = -1;
        int totalItems = -1;
        string _keyword = "";
        string _manufacturerFilter = "";
        private SqlConnection _connection;
        private PHONEControl phoneControl;

        public ProductManagementScreen(SqlConnection con)
        {
            InitializeComponent();
            _connection = con;
            searchTextBox.DataContext = _keyword;
        }
        public BindingList<MANUFACTURER> _ManufacturerList { get; set; }
        private void LoadData()
        {
            phoneControl = new PHONEControl(_connection);

            try
            {
                (_PhoneList, totalItems, totalPages) = PHONEControl.GetAllPaging(_connection, _currentPage, _rowPerPage, _keyword, _manufacturerFilter);
                PhoneListView.ItemsSource = _PhoneList;
                PhoneListView.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (totalItems < _rowPerPage)
            {
                NextPageButton.IsEnabled = false;
            }
            else
            {
                NextPageButton.IsEnabled = true;
            }

            if (_currentPage == 1)
            {
                PreviousPageButton.IsEnabled = false;
            }
            else
            {
                PreviousPageButton.IsEnabled = true;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddPhoneDialog(_connection, _ManufacturerList);
            if (dialog.ShowDialog() == true)
            {
                PHONE newPhone = (PHONE)dialog.NewPhone.Clone();
                _PhoneList.Add(newPhone);
                MessageBox.Show($"Inserted {newPhone.PhoneName} with ID: {newPhone.ID}", "Insert", MessageBoxButton.OK);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            PHONE SelectedPhone = (PHONE)PhoneListView.SelectedItem;
            var choice = MessageBox.Show($"Do you want to delete {SelectedPhone.PhoneName}?", "Delete it fr?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (choice == MessageBoxResult.Yes)
            {
                var result = PHONEControl.DeletePHONE(_connection, SelectedPhone.ID);
                if (result == true)
                {
                    MessageBox.Show($"Deleted {SelectedPhone.PhoneName} with ID: {SelectedPhone.ID}!", "Deleted", MessageBoxButton.OK);
                    LoadData();
                }
                else
                {
                    MessageBox.Show($"Item {SelectedPhone.PhoneName} with ID: {SelectedPhone.ID} has not been deleted!", "Delet Failed", MessageBoxButton.OK);
                }
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var SelectedPhone = (PHONE)PhoneListView.SelectedItem;
            var dialog = new EditPhoneDialog(SelectedPhone, _connection, _ManufacturerList);
            if (dialog.ShowDialog() == true)
            {
                SelectedPhone.PhoneName = dialog.NewPhone.PhoneName;
                SelectedPhone.Manufacturer = dialog.NewPhone.Manufacturer;
                SelectedPhone.Thumbnail = dialog.NewPhone.Thumbnail;
                SelectedPhone.Price = dialog.NewPhone.Price;
            }
        }

        private void ManufacturerFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (MANUFACTURER)ManufacturerFilterComboBox.SelectedItem;
            _manufacturerFilter = selected.Name;
            _currentPage = 1;
            LoadData();
        }

        private void RefreshFilterButton_Click(object sender, RoutedEventArgs e)
        {
            _manufacturerFilter = "";
            _currentPage = 1;
            LoadData();
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadData();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < totalPages)
            {
                _currentPage++;
                LoadData();
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            _keyword = searchTextBox.Text;
            _currentPage = 1;
            LoadData();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(_connection.State == System.Data.ConnectionState.Closed) { _connection.Open();  }
            
            LoadData();
            maNUFACTURERControl = new MANUFACTURERControl(_connection);
            _ManufacturerList = maNUFACTURERControl.GetMANUFACTURERs();

            ManufacturerFilterComboBox.ItemsSource = _ManufacturerList;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _connection.Close();
        }
    }
}
