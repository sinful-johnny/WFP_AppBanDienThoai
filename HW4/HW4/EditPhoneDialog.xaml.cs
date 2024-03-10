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
    /// Interaction logic for EditPhoneDialog.xaml
    /// </summary>
    public partial class EditPhoneDialog : Window
    {
        public PHONE NewPhone { get; set; }
        public EditPhoneDialog(PHONE Phone)
        {
            InitializeComponent();
            if(Phone != null)
            {
                NewPhone = Phone.Clone() as PHONE;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (NewPhone != null)
            {
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
            DialogResult = true;
        }
    }
}
