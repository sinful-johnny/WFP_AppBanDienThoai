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

namespace HW4.UI.Manufacturers
{
    /// <summary>
    /// Interaction logic for ManufacturerNameInputScreen.xaml
    /// </summary>
    public partial class ManufacturerNameInputScreen : Window
    {
        public string Name = string.Empty;
        public ManufacturerNameInputScreen()
        {
            InitializeComponent();
        }
        public ManufacturerNameInputScreen(string ManufacturerName)
        {
            InitializeComponent();
            Name = ManufacturerName;
            ManufacturerNameTextBox.Text = Name;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Name = ManufacturerNameTextBox.Text;
            DialogResult = true;
        }
    }
}
