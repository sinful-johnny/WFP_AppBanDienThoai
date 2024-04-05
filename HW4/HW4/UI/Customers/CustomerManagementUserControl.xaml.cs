using DocumentFormat.OpenXml.Office2010.Excel;
using HW4.BUS;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static HW4.PromoManagementUserControl;

namespace HW4.UI.Customers
{

    /// <summary>
    /// Interaction logic for CustomerManagementUserControl.xaml
    /// </summary>
    public partial class CustomerManagementUserControl : UserControl
    {
        SqlConnection _con;
        int _rowPerPage = 32;
        int _currentPage = 1;
        int _pageSize = 10;
        int totalPages = -1;
        int totalItems = -1;
        BindingList<CUSTOMER> _customers;
        BindingList<Page> PageOptions = new BindingList<Page>();
        public CustomerManagementUserControl(SqlConnection connection)
        {
            InitializeComponent();
            _con = connection;
        }
        public enum CustomerManagementActions
        {
            AddCustomer,
            EditCustomer,
            DeleteCustomer,
        }

        public void HandleParentEvent(CustomerManagementActions action)
        {
            switch (action)
            {
                case CustomerManagementActions.AddCustomer:
                    AddCustomer();
                    break;
                case CustomerManagementActions.EditCustomer:
                    EditCustomer();
                    break;
                case CustomerManagementActions.DeleteCustomer:
                    DeleteCustomer();
                    break;
            }
        }

        void LoadData()
        {
            try
            {
                int oldTotalPages = totalPages;
                (_customers, totalItems, totalPages) = BUS_Customer.GetCustomerByPaging(_con,_currentPage,_rowPerPage);
                CustomerDataGrid.ItemsSource = _customers;
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
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();

            PageSelectComboBox.SelectedIndex = 0;
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

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if(_con.State == ConnectionState.Open)
            {
                _con.Close();
            }
        }

        private void AddCustomer()
        {
            var screen = new CustomerInfoDialog(_con);
            var result = screen.ShowDialog();
            if (result == true)
            {
                MessageBox.Show($"Customer created!", "Success", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show($"Cannot create customer!", "Failed", MessageBoxButton.OK);
            }
            LoadData();
        }
        private void EditCustomer()
        {
            var selectedCustomer = (CUSTOMER)CustomerDataGrid.SelectedItem;
            var screen = new CustomerInfoDialog(_con, selectedCustomer);
            var result = screen.ShowDialog();
            if (result == true)
            {
                LoadData();
            }
        }

        private void DeleteCustomer()
        {
            var selectedCustomer = (CUSTOMER)CustomerDataGrid.SelectedItem;
            int id = int.Parse(selectedCustomer.Cus_ID);
            try
            {
                var result = BUS_Customer.Delete(_con, id);
                if (result == true)
                {
                    MessageBox.Show($"Customer with ID: {id} has been deleted!", "Success", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show($"Cannot delete customer with ID: {id}", "Failed", MessageBoxButton.OK);
                }
                LoadData();
            }catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),"Error",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    internal class Page
    {
        public int _pageNo { get; set; }
        public int _totalPages { get; set; }

        public string Info { get { return $"{_pageNo}/{_totalPages}"; } }
    }
}
