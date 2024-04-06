using HW4.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace HW4.UI.Orders
{
    /// <summary>
    /// Interaction logic for AddPromo.xaml
    /// </summary>
    public partial class AddPromo : Window
    {
        public int OrderID;
        SqlConnection conn;
        public PROMOTIONSINORDER getPromo;
        ObservableCollection<PROMOTIONSINORDER> promoList; 
        public AddPromo(SqlConnection conn, int OrderID)
        {
            InitializeComponent();
            this.conn = conn;
            this.OrderID = OrderID;
        }

        private void LoadPromo()
        {

        }
    }
}
