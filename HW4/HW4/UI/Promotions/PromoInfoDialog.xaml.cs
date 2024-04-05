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
using System.Windows.Shapes;

namespace HW4.UI.Promotions
{
    /// <summary>
    /// Interaction logic for PromoInfoDialog.xaml
    /// </summary>
    public partial class PromoInfoDialog : Window
    {
        int _mode;
        int _ID;
        string _Name = string.Empty;
        DateTime _startDate;
        DateTime _endDate;
        string _manufacturerName = string.Empty;
        string _phoneName = string.Empty;
        double _discount;
        string _status = string.Empty;
        PHONE? _selectedPhone = null;
        SqlConnection _conn;

        BindingList<MANUFACTURER> _manufacturerBindingList;
        public PromoInfoDialog(SqlConnection connection)
        {
            InitializeComponent();
            _mode = Mode.Insert;
            _conn = connection;
            _manufacturerBindingList = BUS_Manufacturer.getAllManufacturers(_conn);
            ManufacturerComboBox.ItemsSource = _manufacturerBindingList;
        }
        public PromoInfoDialog(SqlConnection connection,DataRowView promotion)
        {
            InitializeComponent();
            _mode = Mode.Update;
            _conn = connection;
            _manufacturerBindingList = BUS_Manufacturer.getAllManufacturers(_conn);
            ManufacturerComboBox.ItemsSource = _manufacturerBindingList;

            _ID = (int)promotion.Row.ItemArray[0]!;
            _Name = (string)promotion.Row.ItemArray[1]!;
            _startDate = (DateTime)promotion.Row.ItemArray[2]!;
            _endDate = (DateTime)promotion.Row.ItemArray[3]!;
            if (promotion.Row.ItemArray[4]!.GetType() != typeof(DBNull))
            {
                _manufacturerName = (string)promotion.Row.ItemArray[4]!;
            }
            if (promotion.Row.ItemArray[5]!.GetType() != typeof(DBNull))
            {
                _phoneName = (string)promotion.Row.ItemArray[5]!;
            }
            _discount = (double)promotion.Row.ItemArray[6]!;
            _status = (string)promotion.Row.ItemArray[7]!;

            PromoIDTextBlock.Text = _ID.ToString();
            PromotionNameTextBox.Text = _Name;
            StartDatePicker.Text = _startDate.ToString();
            EndDatePicker.Text = _endDate.ToString();
            DiscountTextBox.Text = _discount.ToString();
            if(_manufacturerName != string.Empty)
            {
                ManufacturerComboBox.SelectedItem = _manufacturerBindingList.FirstOrDefault(x => x.Name == _manufacturerName);
            }
            PhoneNameTextBlock.Text = _phoneName;
            StatusTextBox.Text = _status;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(_mode == Mode.Insert)
            {

            }else if(_mode == Mode.Update)
            {
                MANUFACTURER selectedManufacturer = (MANUFACTURER)ManufacturerComboBox.SelectedItem;
                _Name = PromotionNameTextBox.Text;
                _startDate = DateTime.Parse(StartDatePicker.Text);
                _endDate = DateTime.Parse(EndDatePicker.Text);
                _discount = double.Parse(DiscountTextBox.Text);
                _status = StatusTextBox.Text;
                if(_selectedPhone != null)
                {
                    var result = BUS_Promotions.updatePromotion(_conn, _ID, _Name, _startDate, _endDate, selectedManufacturer.ID, _selectedPhone.ID, _discount, _status);
                }
                else
                {
                    var result = BUS_Promotions.updatePromotion(_conn, _ID, _Name, _startDate, _endDate, selectedManufacturer.ID, -1, _discount, _status);
                }
            }
            DialogResult= true;
        }

        private void PickPhoneButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new PickAPhoneDialog(_conn);
            var result = screen.ShowDialog();
            if(result == true)
            {
                _selectedPhone = screen._selectedPhone;
                PhoneNameTextBlock.Text = _selectedPhone.PhoneName;
            }
            else
            {
                _selectedPhone = null;
                PhoneNameTextBlock.Text = "None";
            }
        }
    }
    internal class Mode
    {
        static public int Insert { get { return 1;} }
        static public int Update { get { return 2;} }
    }
}
