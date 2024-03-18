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
    /// Interaction logic for AddPhoneDialog.xaml
    /// </summary>
    public partial class AddPhoneDialog : Window
    {
        public PHONE NewPhone { get; set; }
        public SqlConnection _connection { get; set; }  
        private BindingList<MANUFACTURER> manufacturers { get; set; }
        public AddPhoneDialog(SqlConnection connection, BindingList<MANUFACTURER> manufacturers)
        {
            InitializeComponent();
            _connection = connection;
            this.manufacturers = manufacturers;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string PhoneName = PhoneUserControl.PhoneNameTextBox.Text;
            string ManufacturerName = PhoneUserControl.PhoneManufacturerTextBox.Text;
            string Thumbnail = PhoneUserControl.ImgPathTextBox.Text;
            double Price = double.Parse(PhoneUserControl.PhonePriceTextBox.Text);

            int Manufacturer_ID = -1;
            try
            {
                MANUFACTURER Manufacturer = manufacturers.Single(x => x.Name == ManufacturerName);
                Manufacturer_ID = Manufacturer.ID;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Manufacturer does not exist");
            }

            if(Manufacturer_ID == -1)
            {
                return;
            }
            
            int PhoneID = PHONEControl.InsertPHONE(_connection,PhoneName, Manufacturer_ID,Thumbnail,Price);

            NewPhone = new PHONE() {
                ID = PhoneID,
                PhoneName = PhoneName,
                Manufacturer = ManufacturerName,
                Thumbnail = Thumbnail,
                Price = Price,
            };

            DialogResult = true;
        }
    }
}
