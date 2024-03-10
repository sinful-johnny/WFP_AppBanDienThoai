using System;
using System.Collections.Generic;
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
        public AddPhoneDialog()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NewPhone = new PHONE() {
                PhoneName = PhoneUserControl.PhoneNameTextBox.Text,
                Manufacturer = PhoneUserControl.PhoneManufacturerTextBox.Text,
                Thumbnail = "",
                Price = double.Parse(PhoneUserControl.PhonePriceTextBox.Text),
            };
            DialogResult = true;
        }
    }
}
