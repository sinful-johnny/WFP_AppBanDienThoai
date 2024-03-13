﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;
using System.Data.SqlClient;


namespace HW4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string username = "sa";
        string password = "123";  
        System.ComponentModel.BindingList<PHONE> _PhoneList;
        PHONEControl phoneControl;
        int _rowPerPage = 10;
        int _currentPage = 1;
        int _pageSize = 10;
        int totalPages = -1;
        int totalItems = -1;
        string _keyword = "";
        private SqlConnection _connection;
        private static System.ComponentModel.BindingList<PHONE> GetPhoneList()
        {
            return new System.ComponentModel.BindingList<PHONE>()
        {
            new PHONE()
            {
                ID = 0,
                PhoneName = "W41+",
                Thumbnail = "./Images/Phone00.jpg",
                Manufacturer = "LG",
                Price = 66.6F,
            },
            new PHONE()
            {
                ID = 1,
                PhoneName = "K62",
                Thumbnail = "./Images/Phone01.jpg",
                Manufacturer = "LG",
                Price = 69.69F
            },
            new PHONE()
            {
                ID = 2,
                PhoneName = "Xiaomi 14",
                Thumbnail = "./Images/Phone02.jpg",
                Manufacturer = "Xiaomi",
                Price = 2
            },
            new PHONE()
            {
                ID = 3,
                PhoneName = "Xiaomi Pad 6S Pro 12.4",
                Thumbnail = "./Images/Phone03.jpg",
                Manufacturer = "Xiaomi",
                Price= 37
            },
            new PHONE()
            {
                ID = 4,
                PhoneName = "Xiaomi Poco X6",
                Thumbnail = "./Images/Phone04.jpg",
                Manufacturer = "Xiaomi",
                Price = 50
            },
            new PHONE()
            {
                ID = 5,
                PhoneName = "iPhone 15 Pro Max",
                Thumbnail = "./Images/Phone05.jpg",
                Manufacturer = "Apple",
                Price = 669
            },
            new PHONE()
            {
                ID = 6,
                PhoneName = "iPhone SE (2022)",
                Thumbnail = "./Images/Phone06.jpg",
                Manufacturer = "Apple",
                Price = 669
            },
            new PHONE()
            {
                ID = 7,
                PhoneName = "Samsung Galaxy S24 Ultra",
                Thumbnail = "./Images/Phone07.jpg",
                Manufacturer = "Samsung",
                Price = 669
            },
            new PHONE()
            {
                ID = 8,
                PhoneName = "Samsung Galaxy Xcover7",
                Thumbnail = "./Images/Phone08.jpg",
                Manufacturer = "Samsung",
                Price = 669
            },
            new PHONE()
            {
                ID = 9,
                PhoneName = "Samsung Galaxy Tab S9 FE+",
                Thumbnail = "./Images/Phone09.jpg",
                Manufacturer = "Samsung",
                Price = 669
            }
        };
        }

        System.ComponentModel.BindingList<string> _ManufacturerList = new System.ComponentModel.BindingList<string>()
        {
            "Samsung",
            "Apple",
            "Xiaomi",
            "LG"
        };
        public MainWindow()
        {
            InitializeComponent();

            searchTextBox.DataContext = _keyword;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            LoginScreen loginScreen = new LoginScreen();
            var result = loginScreen.ShowDialog();
            if (result == true) {
                _connection = loginScreen._connection;
            }
            else
            {
                this.Close();
            }
            this.Show();
            LoadData();
            ManufacturerFilterComboBox.ItemsSource = _ManufacturerList;
        }
        private void LoadData()
        {
            phoneControl = new PHONEControl(_connection);
            try
            {
                (_PhoneList, totalItems, totalPages) = phoneControl.GetAllPaging(_currentPage, _rowPerPage, _keyword);
                PhoneListView.ItemsSource = _PhoneList;
                PhoneListView.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddPhoneDialog();
            if(dialog.ShowDialog() == true) {
                _PhoneList.Add((PHONE)dialog.NewPhone.Clone());
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            PHONE SelectedPhone = (PHONE)PhoneListView.SelectedItem;
            var Result = MessageBox.Show($"Do you want to delete {SelectedPhone.PhoneName}?", "Delete it fr?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(Result == MessageBoxResult.Yes)
            {
                _PhoneList.Remove((PHONE)PhoneListView.SelectedItem);
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var SelectedPhone = (PHONE)PhoneListView.SelectedItem;
            var dialog = new EditPhoneDialog(SelectedPhone);
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
            var filteredList = _PhoneList.Where(phone => phone.Manufacturer == (string)ManufacturerFilterComboBox.SelectedItem);
            //var filteredList = from phone in _PhoneList
            //                   where phone.Manufacturer == (string)ManufacturerFilterComboBox.SelectedItem
            //                   select phone;
            PhoneListView.ItemsSource = filteredList;
            if (filteredList.Count() >= 1)
            {
                PhoneListView.SelectedIndex = 0;
            }
        }

        private void RefreshFilterButton_Click(object sender, RoutedEventArgs e)
        {
            PhoneListView.ItemsSource = _PhoneList;
            PhoneListView.SelectedIndex = 0;
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if(_currentPage > 1)
            {
                _currentPage--;
                LoadData();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if(_currentPage < totalPages) {
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

        void test()
        {
            var list = new List<string>{"sds adsada", "ecxhz uodsa" } ;
            var filtered = from str in list
                           let words = str.Split(' ')
                           from word in words
                           where word.StartsWith('a') || word.StartsWith("e") || word.StartsWith("u")
                           select word;
        }


    }
}