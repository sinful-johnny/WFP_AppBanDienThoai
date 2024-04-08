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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace HW4
{
    /// <summary>
    /// Interaction logic for PhoneOrder.xaml
    /// </summary>
    public partial class PhoneOrder : UserControl
    {
        BindingList<ORDER> _OrderList;
        BindingList<string> timeSelection = new BindingList<string>()
        {
            "Today",
            "This Week",
            "This Month",
            "All Time"
        };
        int _rowPerPage = 12;
        int _currentPage = 1;
        int totalPages = -1;
        int totalItems = -1;
        private SqlConnection _connection;
        public string current = "All Time";
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
        public PhoneOrder(SqlConnection con)
        {
            InitializeComponent();
            _connection = con;
            DateGet.ItemsSource = timeSelection;
            DateGet.SelectedItem = "All Time";
 
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
                case OrderManagementAction.CancelOrder:
                    CancelOrderHandler();
                    break;
            }
        }
        private void LoadData()
        {
            try
            {
                if (current == "All Time" || current == "Today" || current == "This Week" || current == "This Month")
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
            if (OrderWin.ShowDialog() == true)
            {
                MessageBox.Show("Inserted Order Complete!", "Insert", MessageBoxButton.OK);
                LoadData();
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

        private void SearchDateRange(object sender, RoutedEventArgs e)
        {       
            if (startDatePicker.SelectedDate != null)
            {
                start = startDatePicker.SelectedDate.Value;
            }
            else
            {
                MessageBox.Show("Please choose starting day range!", "Missing Start!", MessageBoxButton.OK);
                return;
            }

            if (endDatePicker.SelectedDate != null)
            {
                end = endDatePicker.SelectedDate.Value;

                if (end < start)
                {
                    MessageBox.Show("Start Date must be earlier than End Date!", "Missing Limit!", MessageBoxButton.OK);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please choose ending day range!", "Missing End!", MessageBoxButton.OK);
                return;
            }

            current = "Time Range";
            LoadData();
        }

        private void RefreshOrder(object sender, RoutedEventArgs e)
        {
            current = "All Time";

            DateGet.SelectedItem = current;
            startDatePicker.SelectedDate = DateTime.Now;
            endDatePicker.SelectedDate = DateTime.Now;
            LoadData();
        }

        private void DateGet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            current = DateGet.SelectedItem.ToString();
            _currentPage = 1;
            LoadData();
        }

        private void OrderInfo(object sender, RoutedEventArgs e)
        {
            ORDER selected = (ORDER)OrderView.SelectedItem;
            if (selected != null)
            {
                var info = new OrderInfo(_connection, selected.OrderID);
                info.ShowDialog();

                LoadData();
            }
        }

        private void PagePrev(object sender, RoutedEventArgs e)
        {
            _currentPage -= 1;
            LoadData();
        }

        private void PageNext(object sender, RoutedEventArgs e)
        {
            _currentPage += 1;
            LoadData();
        }
    }
}
