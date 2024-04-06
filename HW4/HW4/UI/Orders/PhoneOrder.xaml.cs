using HW4.BUS;
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
using System.Windows.Shapes;
using static HW4.ProductManagementScreen;

namespace HW4
{
    /// <summary>
    /// Interaction logic for PhoneOrder.xaml
    /// </summary>
    public partial class PhoneOrder : UserControl
    {
        ObservableCollection<ORDER> _OrderList;
        int _rowPerPage = 12;
        int _currentPage = 1;
        int totalPages = -1;
        int totalItems = -1;
        private SqlConnection _connection;
        string current = "All time";
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
        public PhoneOrder(SqlConnection con, OrderInfo orderInfo)
        {
            InitializeComponent();
            orderInfo.DataSent += ChildUserControl_DataSent;
            _connection = con;
            startDatePicker.SelectedDate = DateTime.Now;
            endDatePicker.SelectedDate = DateTime.Now;
        }

        public enum OrderManagementAction
        {
            AddOrder,
            DeleteOrder,
            DeliverOrder,
            CancelOrder
        }
        public void HandleParentsEvent(OrderManagementAction action)
        {
            switch (action)
            {
                case OrderManagementAction.AddOrder:
                    AddOrderHandler();
                    break;
                case OrderManagementAction.DeleteOrder:
                    DeleteOrderHandler();
                    break;
                case OrderManagementAction.DeliverOrder:
                    DeliverOrderHandler();
                    break;
            }
        }
        private void LoadData()
        {
            try
            {
                if (current == "All time" || current == "Today" || current == "This Week" || current == "This Month")
                {
                    (_OrderList, totalItems, totalPages) = BUS_Order.SortOrder(_connection, _currentPage, _rowPerPage, current);
                }
                if (current == "Time Range")
                {
                    (_OrderList, totalItems, totalPages) = BUS_Order.getInRange(_connection, _currentPage, _rowPerPage, start, end);
                }
                OrderView.ItemsSource = _OrderList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (totalItems < _rowPerPage)
            {
                Next.IsEnabled = false;
            }
            else
            {
                Next.IsEnabled = true;
            }

            if (_currentPage == 1)
            {
                Prev.IsEnabled = false;
            }
            else
            {
                Prev.IsEnabled = true;
            }
        }

        public void AddOrderHandler()
        {
            var OrderWin = new AddOrderDialog(_connection);
            if (OrderWin.DialogResult == true)
            {
                LoadData();
                MessageBox.Show("A new Order has been added!", "Add Success!", MessageBoxButton.OK);
            }
        }
        public void DeleteOrderHandler()
        {
            ORDER Selected = (ORDER)OrderView.SelectedItem;
            var choice = MessageBox.Show($"Do you want to delete Order {Selected.OrderID}?", "Delete the Order?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (choice == MessageBoxResult.Yes)
            {
                var deleted = BUS_Order.DeleteOrder(_connection, Selected.OrderID);
                if (deleted == true)
                {
                    MessageBox.Show($"Deleted Order {Selected.OrderID} successfully!", "Deleted Successfully", MessageBoxButton.OK);
                    LoadData();
                }
                else
                {
                    MessageBox.Show($"Failed to Delete Order {Selected.OrderID}. Please try again!", "Delete Failed!", MessageBoxButton.OK);
                }
            }
        }

        public void DeliverOrderHandler()
        {
            ORDER Selected = (ORDER)OrderView.SelectedItem;
            var choice = MessageBox.Show($"Do you want to Deliver Order {Selected.OrderID}?", "Deliver the Order?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (choice == MessageBoxResult.Yes)
            {
                var delivered = BUS_Order.DeliverOrder(_connection, Selected.OrderID);
                if (delivered == true)
                {
                    MessageBox.Show($"Order {Selected.OrderID} is being delivered!", "Under delivery!", MessageBoxButton.OK);
                    LoadData();
                }

                else
                {
                    MessageBox.Show($"Cannot deliver Order {Selected.OrderID} at the moment!", "Delivery Failed!", MessageBoxButton.OK);
                }
            }
        }

        public void CancelOrderHandler()
        {
            ORDER Selected = (ORDER)OrderView.SelectedItem;
            var choice = MessageBox.Show($"Do you want to Cancel Order {Selected.OrderID}?", "Cancel the Order?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (choice == MessageBoxResult.Yes)
            {
                var cancelled = BUS_Order.CancelOrder(_connection, Selected.OrderID);
                if (cancelled == true)
                {
                    MessageBox.Show($"Order {Selected.OrderID} is cancelled!", "Cancelled Successfully!", MessageBoxButton.OK);
                    LoadData();
                }
                else
                {
                    MessageBox.Show($"Cannot Cancel Order {Selected.OrderID} at the moment!", "Cancel Failed!", MessageBoxButton.OK);
                }
            }
        }

        private void AddOrder(object sender, RoutedEventArgs e)
        {
            AddOrderHandler();
        }

        private void DeleteOrder(object sender, RoutedEventArgs e)
        {
            DeleteOrderHandler();
        }

        private void ChildUserControl_DataSent(object sender, RoutedEventArgs e)
        {
            // Do something when data is sent from the child UserControl
            LoadData();
        }
        private void Page(object sender, RoutedEventArgs e)
        {
            string index = (string)(sender as Button).Content;
            if (index == "Next")
            {
                _currentPage += 1;
            }

            else
            {
                _currentPage -= 1;
            }

            LoadData();
        }

        private void SearchDateRange(object sender, RoutedEventArgs e)
        {
            current = "Time Range";
            start = (DateTime)startDatePicker.SelectedDate;
            end = (DateTime)endDatePicker.SelectedDate;
            if (start == null || end == null)
            {
                MessageBox.Show("Please choose both day range!", "Mssing Limit!", MessageBoxButton.OK);
            }
            else if (start > end)
            {
                MessageBox.Show("Invalid Dates! Choose againQ", "Invalid Date!", MessageBoxButton.OK);
            }

            else
            {
                LoadData();
            }
        }

        private void RefreshOrder(object sender, RoutedEventArgs e)
        {
            current = "All time";

            DateGet.SelectedItem = current;

            LoadData();
        }

        private void DateGet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            current = DateGet.SelectedItem.ToString();
            LoadData();
        }


    }
}
