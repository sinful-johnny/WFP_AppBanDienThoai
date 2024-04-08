using HW4.BUS;
using HW4.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for OrderInfo.xaml
    /// </summary>
    public partial class OrderInfo : Window
    {
        public ORDER Order;
        SqlConnection conn;
        BindingList<ORDEREDPHONE> phoneList = new BindingList<ORDEREDPHONE>();
        BindingList<PROMOTIONSINORDER> promoList = new BindingList<PROMOTIONSINORDER>();
        private class Info
        {
            public int OrderID { get; set; }
            public string CustomerName { get; set; }
            public string OrderDate { get; set; }
        }
        public OrderInfo(SqlConnection conn, ORDER OrderInfo)
        {
            InitializeComponent();
            this.conn = conn;
            Order = OrderInfo;
            Info info = new Info()
            {
                OrderID = Order.OrderID,
                CustomerName = Order.CustomerName,
                OrderDate = Order.OrderDate.ToShortDateString(),
            };
            OrderInfoBasic.DataContext = info;
            LoadData();
        }

        private void LoadData()
        {
            (phoneList, promoList) = BUS_Order.getOrderData(conn, Order.OrderID);
            OrderPhones.ItemsSource = phoneList;
            OrderPromos.ItemsSource = promoList;
        }
        private void UpdatePhoneCount(object sender, RoutedEventArgs e)
        {
            ORDEREDPHONE selected = (ORDEREDPHONE)OrderPhones.SelectedItem;
            if (selected == null) return;
            var UpdatePhoneCount = new EditPhoneQuantity(conn, selected, Order.OrderID);
            if (UpdatePhoneCount.ShowDialog() == true)
            {
                MessageBox.Show("Updated Phone Successfully!", "Updated Successfully", MessageBoxButton.OK);
                LoadData();
            }
            else
            {
                MessageBox.Show("Updated Phone Failed!", "Update Failed!", MessageBoxButton.OK);
            }
        }
        private void DeletePhone(object sender, RoutedEventArgs e)
        {
            ORDEREDPHONE selected = (ORDEREDPHONE)OrderPhones.SelectedItem;

            var deleteOption = MessageBox.Show("Are you sure you want to delete this phone?", "Delete Phone?", MessageBoxButton.YesNo);
            if (deleteOption == MessageBoxResult.Yes)
            {
                bool deleted = BUS_Order.EditPhoneInOrder(conn, selected, Order.OrderID, "Delete Phone");
                if (deleted ==  true)
                {
                    MessageBox.Show($"Deleted Phone {selected.PhoneName} successfully!", "Delete Success!", MessageBoxButton.OK);
                    LoadData();
                }

                else
                {
                    MessageBox.Show($"Cannot Delete Phone {selected.PhoneName}!", "Delete Failed!", MessageBoxButton.OK);
                }
            }
        }

        private void AddPhone(object sender, RoutedEventArgs e)
        {
            var AddPhone = new NewPhoneInOrder(conn, Order);
            if (AddPhone.ShowDialog() == true)
            {
                MessageBox.Show("Added New Phone Successfully!", "Add Successfully", MessageBoxButton.OK);
                LoadData();
            }

            else
            {
                MessageBox.Show("Failed to Added New Phone!", "Add Failed", MessageBoxButton.OK);
            }
        }

        private void AddPromo(object sender, RoutedEventArgs e)
        {
            var AddPromo = new AddPromo(conn, Order);
            if (AddPromo.ShowDialog() == true)
            {
                MessageBox.Show("Added New Promo Successfully!", "Add Successfully", MessageBoxButton.OK);
                LoadData();
            }

            else
            {
                MessageBox.Show("Failed to Added New Promo!", "Add Failed", MessageBoxButton.OK);
            }
        }

        private void DeletePromo(object sender, RoutedEventArgs e)
        {
            PROMOTIONSINORDER selected = (PROMOTIONSINORDER)OrderPromos.SelectedItem;

            var deleteOption = MessageBox.Show("Are you sure you want to delete this promotion?", "Delete Promotion?", MessageBoxButton.YesNo);
            if (deleteOption == MessageBoxResult.Yes)
            {
                bool deleted = BUS_Order.EditPromoInOrder(conn, selected.PromoID, Order.OrderID, "Delete Promo");
                if (deleted == true)
                {
                    MessageBox.Show($"Deleted Promotion {selected.PromoName} successfully!", "Delete Success!", MessageBoxButton.OK);
                    LoadData();
                }

                else
                {
                    MessageBox.Show($"Cannot Delete Promotion {selected.PromoName}!", "Delete Failed!", MessageBoxButton.OK);
                }
            }
        }
    }
}
