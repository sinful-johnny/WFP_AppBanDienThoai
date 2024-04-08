using HW4.BUS;
using HW4.UI.Manufacturers;
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
using static HW4.ProductManagementScreen;

namespace HW4
{
    /// <summary>
    /// Interaction logic for ManufacturerManagementUserControl.xaml
    /// </summary>
    public partial class ManufacturerManagementUserControl : System.Windows.Controls.UserControl
    {
        BindingList<MANUFACTURER> _manufacturers;
        SqlConnection _conn;
        string keyWord = string.Empty;
        int _queryMode = QueryMode.None;
        DataTable dt;
        public enum ManufacturerManagementAction
        {
            AddManufacturer,
            DeleteManufacturer,
            UpdateManufacturer
        }
        public void HandleParentEvent(ManufacturerManagementAction action)
        {
            switch (action)
            {
                case ManufacturerManagementAction.AddManufacturer:
                    AddManufacturers();
                    break;

                case ManufacturerManagementAction.DeleteManufacturer:
                    deleteManufacturer();
                    break;

                case ManufacturerManagementAction.UpdateManufacturer:
                    UpdateManufacturers();
                    break;
            }
        }
        public ManufacturerManagementUserControl(SqlConnection connection)
        {
            InitializeComponent();
            _conn = connection;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadManufacturers();
            dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("NAME");
            foreach (var manufacturer in _manufacturers)
            {
                dt.Rows.Add(manufacturer.ID, manufacturer.Name);
            }
            ManufacturerDataGrid.ItemsSource = dt.DefaultView;
        }

        private void LoadManufacturers()
        {
            try
            {
                if(_queryMode == QueryMode.None)
                {
                    _manufacturers = BUS_Manufacturer.getAllManufacturers(_conn);
                }else if(_queryMode == QueryMode.Keyword)
                {
                    _manufacturers = BUS_Manufacturer.getAllManufacturersByKeyword(_conn, keyWord);
                }
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private async void AddManufacturers() { 
            string ManufacturerName = string.Empty;
            var screen = new ManufacturerNameInputScreen();
            var result = screen.ShowDialog();
            if (result == true)
            {
                ManufacturerName = screen.Name;
                BUS_Manufacturer.insertManufacturer(_conn, ManufacturerName);

                await Task.Run(LoadManufacturers);

                dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("NAME");
                foreach (var manufacturer in _manufacturers)
                {
                    dt.Rows.Add(manufacturer.ID, manufacturer.Name);
                }
                ManufacturerDataGrid.ItemsSource = dt.DefaultView;
            }
        }

        private async void UpdateManufacturers()
        {
            DataRowView row = (DataRowView)ManufacturerDataGrid.SelectedItems[0]!;
            string ManufacturerID = (string)row.Row.ItemArray[0]!;
            string ManufacturerName = (string)row.Row.ItemArray[1]!;

            var screen = new ManufacturerNameInputScreen();
            var result = screen.ShowDialog();

            if (result == true)
            {
                ManufacturerName = screen.Name;
                BUS_Manufacturer.updateManufacturer(_conn ,ManufacturerID!, ManufacturerName);

                await Task.Run(LoadManufacturers);

                dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("NAME");
                foreach (var manufacturer in _manufacturers)
                {
                    dt.Rows.Add(manufacturer.ID, manufacturer.Name);
                }
                ManufacturerDataGrid.ItemsSource = dt.DefaultView;
            }
        }

        private async void deleteManufacturer()
        {
            DataRowView row = (DataRowView)ManufacturerDataGrid.SelectedItems[0];
            string ManufacturerID = (string)row.Row.ItemArray[0];
            string name = (string)row.Row.ItemArray[1];

            var result = System.Windows.MessageBox.Show($"Are you sure want to delete Manufacturer {name}?", $"Deleting {name}", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                BUS_Manufacturer.deleteManufacturer(_conn,ManufacturerID!);

                await Task.Run(LoadManufacturers);

                dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("NAME");
                foreach (var manufacturer in _manufacturers)
                {
                    dt.Rows.Add(manufacturer.ID, manufacturer.Name);
                }
                ManufacturerDataGrid.ItemsSource = dt.DefaultView;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if(_conn.State == System.Data.ConnectionState.Open)
            {
                _conn.Close();
            }
        }

        private async void SeachButton_Click(object sender, RoutedEventArgs e)
        {
            keyWord = SearchTextBox.Text;
            if(keyWord == null || keyWord == "" || keyWord == " ") { 
                _queryMode = QueryMode.None;
            }
            else
            {
                _queryMode = QueryMode.Keyword;
            }
            await Task.Run(LoadManufacturers);
            dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("NAME");
            foreach (var manufacturer in _manufacturers)
            {
                dt.Rows.Add(manufacturer.ID, manufacturer.Name);
            }
            ManufacturerDataGrid.ItemsSource = dt.DefaultView;
        }
    }
    internal class QueryMode
    {
        static public int None { get { return 0; } }
        static public int Keyword { get { return 1; }}
    }
}
