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
using System.Windows.Shapes;

namespace HW4
{
    /// <summary>
    /// Interaction logic for EditPhoneDialog.xaml
    /// </summary>
    public partial class EditPhoneDialog : Window
    {
        public PHONE OldPhone { get; set; }
        public PHONE NewPhone { get; set; }
        public SqlConnection _connection { get; set; }
        public BindingList<MANUFACTURER> _manufacturers { get; set; }
        public EditPhoneDialog(PHONE Phone, SqlConnection connection, BindingList<MANUFACTURER> manufacturers)
        {
            InitializeComponent();
            if(Phone != null)
            {
                OldPhone = Phone.Clone() as PHONE;
                _connection = connection;
                _manufacturers = manufacturers;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (OldPhone != null)
            {
                NewPhone = (PHONE)OldPhone.Clone();
                DataContext = NewPhone;
            }
            else
            {
                MessageBox.Show("Choose a phone to edit!", "You haven't choose a phone!", MessageBoxButton.OK);
                DialogResult = false;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            int ID = NewPhone.ID;
            string PhoneName = NewPhone.PhoneName;
            string ManufacturerName = NewPhone.Manufacturer;
            string Thumbnail = NewPhone.Thumbnail;
            double Price = NewPhone.Price;

            int Manufacturer_ID = -1;
            try
            {
                MANUFACTURER Manufacturer = _manufacturers.Single(x => x.Name == ManufacturerName);
                Manufacturer_ID = Manufacturer.ID;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Manufacturer does not exist");
            }

            if (Manufacturer_ID == -1)
            {
                return;
            }

            bool result = false;
            try
            {
                result = PHONEControl.UpdatePHONE(_connection,ID, PhoneName, Manufacturer_ID, Thumbnail, Price);
            }
            catch(Exception ex) { 
                MessageBox.Show(ex.ToString());
            }
            
            if (!result)
            {
                return;
            }

            DialogResult = true;
        }
    }
}
