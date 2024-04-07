using HW4.BUS;
using HW4.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace HW4
{
    /// <summary>
    /// Interaction logic for AddPromo.xaml
    /// </summary>
    public partial class AddPromo : Window
    {
        public int OrderID;
        SqlConnection conn;
        public PROMOTIONSINORDER getPromo;
        BindingList<PROMOTIONSINORDER> promoList = new BindingList<PROMOTIONSINORDER>(); 
        public AddPromo(SqlConnection conn, int OrderID)
        {
            InitializeComponent();
            this.conn = conn;
            this.OrderID = OrderID;
            LoadPromo();
            Order.DataContext = OrderID;
        }

        private void LoadPromo()
        {
            string sql = """
                SELECT * FROM PROMOTIONS
                """;
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            using var command = new SqlCommand(sql, conn);
            var read = command.ExecuteReader();
            while (read.Read())
            {
                promoList.Add(new PROMOTIONSINORDER()
                {
                    PromoID = (int)read["PROMO_ID"],
                    PromoName = (string)read["PROMO_NAME"],
                });
            }

            Promo.ItemsSource = promoList;
        }

        private void AddNewPromo(object sender, RoutedEventArgs e)
        {
            var selected = (PROMOTIONSINORDER)Promo.SelectedItem;

            var addnew = BUS_Order.EditPromoInOrder(conn, selected.PromoID, OrderID, "Add Promo");
            if (addnew == true)
            {
                DialogResult = true;
            }

            else
            {
                DialogResult = false;
            }
        }
    }
}
