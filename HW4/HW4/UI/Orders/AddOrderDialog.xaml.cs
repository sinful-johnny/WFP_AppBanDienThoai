using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;
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
using System.ComponentModel;
using HW4.BUS;

namespace HW4
{
    /// <summary>
    /// Interaction logic for AddOrderDialog.xaml
    /// </summary>
    public partial class AddOrderDialog : Window
    {
        ObservableCollection<CUSTOMER> customer_list = new ObservableCollection<CUSTOMER>();
        ObservableCollection<PHONE> phone_list = new ObservableCollection<PHONE>();
        ObservableCollection<ORDEREDPHONE> cart = new ObservableCollection<ORDEREDPHONE>();
        ORDER NewOrder;
        SqlConnection connection;
        private void LoadCustomer()
        {
            string sql = """
                SELECT * FROM CUSTOMER
                """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            using var command = new SqlCommand(sql, connection);
            var read = command.ExecuteReader();
            while (read.Read())
            {
                customer_list.Add(new CUSTOMER() 
                { 
                    Cus_ID = (int)read["CUS_ID"],
                    Name = (string)read["FIRSTNAME"] + (string)read["LASTNAME"],
                });
            }

            Customer.ItemsSource = customer_list;
        }

        private void LoadPhone()
        {
            string sql = """
                SELECT PHONE.ID, PHONE.NAME, PHONE.PRICE, PHONE.THUMBNAIL, MANUFACTURER.NAME as MANUFACTURER_NAME
                FROM PHONE JOIN MANUFACTURER ON PHONE.MANUFACTURER_ID = MANUFACTURER.ID
                """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            using var command = new SqlCommand(sql, connection);
            var read = command.ExecuteReader();
            while (read.Read())
            {
                phone_list.Add(new PHONE()
                {
                    ID = (int)read["ID"],
                    PhoneName = (string)read["NAME"],
                    Price = (float)read["PRICE"],
                    Thumbnail = (string)read["THUMBNAIL"],
                    Manufacturer = (string)read["MANUFACTURER_NAME"]
                });
            }

            PhoneChooseView.ItemsSource = phone_list;
        }
        public AddOrderDialog(SqlConnection conn)
        {
            InitializeComponent();
            connection = conn;
            PhoneChooseView.ItemsSource = cart;
            LoadCustomer();
            LoadPhone();
        }

        private void AddToCart(object sender, RoutedEventArgs e)
        {
            PHONE selected = (PHONE)PhoneChooseView.SelectedItem;
            if (selected != null)
            {
                ORDEREDPHONE newPhone = new ORDEREDPHONE() 
                {
                    PhoneID = selected.ID,
                    Price = (float)selected.Price,
                    PhoneName = (string)selected.PhoneName,
                    quantity = 1
                };

                cart.Add(newPhone);
                PhoneChooseView.ItemsSource = cart;
            }
        }

        private void AddNewOrder(object sender, RoutedEventArgs e)
        {
            CUSTOMER selected_customer = (CUSTOMER)Customer.SelectedItem;
            var choice = MessageBox.Show("Do you want to add a new order?", "Add Order?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (choice  == MessageBoxResult.Yes)
            {
                int AddNewOrder = BUS_Order.NewOrder(connection, selected_customer.Cus_ID, cart);

                if (AddNewOrder == -1)
                {
                    MessageBox.Show("Can't add A new Order!", "Add Failed!", MessageBoxButton.OK);
                }
                else
                {
                    DialogResult = true;
                }
            }
        }
    }
}
