using HW4.BUS;
using HW4.UI.Products;
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
        int _rowPerPage = 12;
        int _currentPage = 1;
        int _pageSize = 10;
        int totalPages = -1;
        int totalItems = -1;
        string _keyword = "";
        string _manufacturerFilter = "";
        private SqlConnection _connection;
        PriceRange _priceRange = new PriceRange() { 
            Min = 0,
            Max = 99999,
        };
        string filter = "Manufacturer_Keyword";

        List<int> ItemPerPageOptions = new List<int>()
        {
            4,16,32,64,128,512,1024
        };

        public ProductManagementScreen(SqlConnection con)
        {
            InitializeComponent();
            _connection = con;
            searchTextBox.DataContext = _keyword;
            MinPriceTextBox.DataContext = _priceRange;
            MaxPriceTextBox.DataContext = _priceRange;
            ItemPerPageComboBox.ItemsSource = ItemPerPageOptions;
        }

        public enum ProductManagementAction
        {
            AddProduct,
            DeleteProduct,
            EditProduct,
            ExcelImport
        }

        public void HandleParentEvent(ProductManagementAction action)
        {
            switch(action)
            {
                case ProductManagementAction.AddProduct:
                    AddProductHandler();
                break;

                case ProductManagementAction.DeleteProduct:
                    DeleteProductHandler();
                break;

                case ProductManagementAction.EditProduct:
                    UpdateProductHandler();
                break;
                case ProductManagementAction.ExcelImport:
                    ImportProduct();
                break;
            }
        }
        public BindingList<MANUFACTURER> _ManufacturerList { get; set; }
        private void LoadData()
        {
            try
            {
                if(filter == "Manufacturer_Keyword")
                {
                    (_PhoneList, totalItems, totalPages) = BUS_Phone.GetPHONEs(_connection, _currentPage, _rowPerPage, _keyword, _manufacturerFilter);
                }
                if(filter == "Price")
                {
                    (_PhoneList, totalItems, totalPages) = BUS_Phone.GetByPrice(_connection, _currentPage, _rowPerPage, _priceRange.Min,_priceRange.Max);
                }
                PhoneListView.ItemsSource = _PhoneList;
                PhoneListView.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (_currentPage == totalPages)
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
            AddProductHandler();
        }

        private void AddProductHandler()
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
            DeleteProductHandler();
        }

        private void DeleteProductHandler()
        {
            PHONE SelectedPhone = (PHONE)PhoneListView.SelectedItem;
            var choice = MessageBox.Show($"Do you want to delete {SelectedPhone.PhoneName}?", "Delete it fr?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (choice == MessageBoxResult.Yes)
            {
                var result = BUS_Phone.delete(_connection, SelectedPhone.ID);
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
            UpdateProductHandler();
        }

        private void UpdateProductHandler()
        {
            var SelectedPhone = (PHONE)PhoneListView.SelectedItem;
            var dialog = new EditPhoneDialog(SelectedPhone, _connection, _ManufacturerList);
            if (dialog.ShowDialog() == true)
            {
                SelectedPhone.PhoneName = dialog.NewPhone.PhoneName;
                SelectedPhone.Manufacturer = dialog.NewPhone.Manufacturer;
                SelectedPhone.Thumbnail = dialog.NewPhone.Thumbnail;
                SelectedPhone.Price = dialog.NewPhone.Price;
                SelectedPhone.Price = dialog.NewPhone.OriginalPrice;
            }
        }

        private void ImportProduct()
        {
            var screen = new ExcelImportDialog(_connection);
            var result = screen.ShowDialog();
            LoadData();
        }

        private void ManufacturerFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filter = "Manufacturer_Keyword";
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
            filter = "Manufacturer_Keyword";
            _keyword = searchTextBox.Text;
            _currentPage = 1;
            LoadData();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(_connection.State == System.Data.ConnectionState.Closed) { _connection.Open();  }
            
            LoadData();
            _ManufacturerList = BUS_Manufacturer.getAllManufacturers(_connection);
            ManufacturerFilterComboBox.ItemsSource = _ManufacturerList;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if(_connection.State == System.Data.ConnectionState.Open) { _connection.Close(); }
        }

        private void PriceFilterButton_Click(object sender, RoutedEventArgs e)
        {
            filter = "Price";
            LoadData();
        }

        private void ItemPerPageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _rowPerPage = (int)ItemPerPageComboBox.SelectedItem;
            LoadData();
        }
    }
    internal class PriceRange : INotifyPropertyChanged
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
