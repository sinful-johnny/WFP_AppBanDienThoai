using HW4.BUS;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class EditPhoneQuantity : Window
    {
        SqlConnection conn;
        public ORDEREDPHONE phone;
        Info info;
        public EditPhoneQuantity(SqlConnection conn, ORDEREDPHONE phone, int OrderID)
        {
            InitializeComponent();
            this.conn = conn;
            this.phone = phone;
            info = new Info()
            {
                OrderID = OrderID,
                PhoneName = phone.PhoneName,
                Price = phone.Price,
            };
            OrderInfoBasic.DataContext = info;
            quantity.DataContext = phone.quantity;
        }

        internal class Info
        {
            public int OrderID { get; set; }
            public string PhoneName { get; set; }
            public double Price{ get; set; }
        }

        private void UpdateQuantity(object sender, RoutedEventArgs e)
        {

            try
            {
                int getQuantity = int.Parse(quantity.Text);
                ORDEREDPHONE oRDEREDPHONE = new ORDEREDPHONE()
                {
                    quantity = getQuantity,
                    PhoneID = phone.PhoneID,
                    Price = phone.Price,
                    PhoneName = phone.PhoneName
                };

                bool updated = BUS_Order.EditPhoneInOrder(conn, oRDEREDPHONE, info.OrderID, "Update Phone Quantity");
                if (updated == true)
                {
                    DialogResult = true;
                }
                else
                {
                    DialogResult= false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("You must input number!");
            }
        }
    }
}
