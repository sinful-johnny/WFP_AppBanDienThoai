using System.Text;
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
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace HW4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Fluent.RibbonWindow
    {
        BindingList<PHONE> _PhoneList;
        int _rowPerPage = 10;
        int _currentPage = 1;
        int _pageSize = 10;
        int totalPages = -1;
        int totalItems = -1;
        string _keyword = "";
        string _manufacturerFilter = "";
        private SqlConnection _connection;

        public BindingList<MANUFACTURER> _ManufacturerList { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            
            //searchTextBox.DataContext = _keyword;
        }


        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            LoginScreen loginScreen = new LoginScreen();
            var result = loginScreen.ShowDialog();
            if (result == true)
            {
                _connection = loginScreen._connection;
            }
            else
            {
                this.Close();
                return;
            }
            this.Show();
            var screens = new ObservableCollection<TabItem>()
                {
                    new TabItem() { Content = new DashboardScreen(), Header= "Dashboard"},
                    new TabItem() { Content = new ProductManagementScreen(_connection), Header= "Products"},
                    new TabItem() {Content = new ManufacturerManagementUserControl(_connection), Header = "Manufacturer"},
                    new TabItem(),
                    new TabItem(),
                    new TabItem() {Content = new PromoManagementUserControl(_connection), Header = "Promotions"}
                };
            tabs.ItemsSource = screens;
        }

        private void AddProductRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = (TabItem)tabs.SelectedItem;
            ProductManagementScreen? userControl = (ProductManagementScreen) tabItem.Content;
            userControl.HandleParentEvent(
                ProductManagementScreen.ProductManagementAction.AddProduct
            );

        }

        private void UpdateProductRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = (TabItem)tabs.SelectedItem;
            ProductManagementScreen? userControl = (ProductManagementScreen)tabItem.Content;
            userControl.HandleParentEvent(
                ProductManagementScreen.ProductManagementAction.EditProduct
            );
        }

        private void DeleteProductRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = (TabItem)tabs.SelectedItem;
            ProductManagementScreen? userControl = (ProductManagementScreen)tabItem.Content;
            userControl.HandleParentEvent(
                ProductManagementScreen.ProductManagementAction.DeleteProduct
            );
        }

        private void AddManufacturerRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = (TabItem)tabs.SelectedItem;
            ManufacturerManagementUserControl? userControl = (ManufacturerManagementUserControl)tabItem.Content;
            userControl.HandleParentEvent(
                ManufacturerManagementUserControl.ManufacturerManagementAction.AddManufacturer
            );
        }

        private void UpdateManufacturerRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = (TabItem)tabs.SelectedItem;
            ManufacturerManagementUserControl? userControl = (ManufacturerManagementUserControl)tabItem.Content;
            userControl.HandleParentEvent(
                ManufacturerManagementUserControl.ManufacturerManagementAction.UpdateManufacturer
            );
        }

        private void DeleteManufacturerRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = (TabItem)tabs.SelectedItem;
            ManufacturerManagementUserControl? userControl = (ManufacturerManagementUserControl)tabItem.Content;
            userControl.HandleParentEvent(
                ManufacturerManagementUserControl.ManufacturerManagementAction.DeleteManufacturer
            );
        }

        private void ExcelImportRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = (TabItem)tabs.SelectedItem;
            ProductManagementScreen? userControl = (ProductManagementScreen)tabItem.Content;
            userControl.HandleParentEvent(
                ProductManagementScreen.ProductManagementAction.ExcelImport
            );
        }

        private void AddPromotionRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = (TabItem)tabs.SelectedItem;
            PromoManagementUserControl? userControl = (PromoManagementUserControl)tabItem.Content;
            userControl.HandleParentEvent(
                PromoManagementUserControl.PromotionManagementActions.AddPromo
            );
        }

        private void UpdatePromotionRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = (TabItem)tabs.SelectedItem;
            PromoManagementUserControl? userControl = (PromoManagementUserControl)tabItem.Content;
            userControl.HandleParentEvent(
                PromoManagementUserControl.PromotionManagementActions.EditPromo
            );
        }

        private void DeletePromotionRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = (TabItem)tabs.SelectedItem;
            PromoManagementUserControl? userControl = (PromoManagementUserControl)tabItem.Content;
            userControl.HandleParentEvent(
                PromoManagementUserControl.PromotionManagementActions.DeletePromo
            );
        }

        private void AvailablePromosRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = (TabItem)tabs.SelectedItem;
            PromoManagementUserControl? userControl = (PromoManagementUserControl)tabItem.Content;
            userControl.HandleParentEvent(
                PromoManagementUserControl.PromotionManagementActions.SeeAvailablePromo
            );
        }
    }
}