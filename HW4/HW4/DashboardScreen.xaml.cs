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
using System.Data.SqlClient;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.ComponentModel;
using HW4.DTO;
using System.Collections.ObjectModel;
using System.Data;
using DocumentFormat.OpenXml.Wordprocessing;

namespace HW4
{
    /// <summary>
    /// Interaction logic for DashboardScreen.xaml
    /// </summary>
    public partial class DashboardScreen : UserControl
    {
        SqlConnection _connection;
        private class InfoCard : INotifyPropertyChanged
        {
            public string Title { get; set; }
            public string SubTitle { get; set; }
            public string Image { get; set; }

            public event PropertyChangedEventHandler? PropertyChanged;
        }
        public DashboardScreen(SqlConnection conn)
        {
            InitializeComponent();
            this._connection = conn;
        }

        private DateTime TakeMinMaxOrderDate(SqlConnection connection)
        {
            DateTime newestOrderedDate = new DateTime();
            //DateTime oldestOrderedDate = new DateTime();

            string sql = """
                              select MAX(CREATED_DATE) AS 'NEWESTORDEREDDATE', 
                                     MIN(CREATED_DATE) AS 'OLDESTORDEREDDATE'
                              from ORDERS
                         """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            using (var command = new SqlCommand(sql, connection))
            {
                //_connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    newestOrderedDate = (DateTime)reader["NEWESTORDEREDDATE"];
                    //oldestOrderedDate = (DateTime)reader["OLDESTORDEREDDATE"];
                }
                reader.Close();
            }
            connection.Close();

            //var res = new Tuple<DateTime, DateTime>(newestOrderedDate, oldestOrderedDate);
            var res = newestOrderedDate;

            return res;
        }

        private int AmountProducts(DateTime beginDate, DateTime endDate)
        {
            string sql = """
                              select ORDER_ID
                              from ORDERS
                              where CREATED_DATE BETWEEN @StartDate AND @EndDate
                         """;
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            int numberOfOrders = 0;

            using (var command = new SqlCommand(sql, _connection))
            {
                //_connection.Open();
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = beginDate;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;

                //_connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                    numberOfOrders++;
                reader.Close();
            }
            return numberOfOrders;
        }

        private int AmountOnSales()
        {
            string sql = """
                              select count(distinct PHONE_ID) AS 'NUMBER'
                              from ORDERS_PHONE
                         """;
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            int numberOnSales = 0;

            using (var command = new SqlCommand(sql, _connection))
            {
                //_connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                    numberOnSales = (int)reader["NUMBER"];
                reader.Close();
            }
            return numberOnSales;
        }

        private void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            var newestOderedDate = TakeMinMaxOrderDate(_connection);

            var beginDate = newestOderedDate.AddDays(-7);
            var endDate = newestOderedDate;

            int numberOfPhoneInWeek = AmountProducts(beginDate, endDate);

            beginDate = newestOderedDate.AddDays(-30);
            endDate = newestOderedDate;

            int numberOfPhoneInMonth = AmountProducts(beginDate, endDate);

            int amountonsales = AmountOnSales();
            InfoCard salesInforCard = new InfoCard()
            {
                Title = "On Sales",
                SubTitle = $"Số lượng sản phẩm đang bán là : {amountonsales}",
                Image = "/Images/plus.png"
            };

            InfoCard purchasinginWeekInforCard = new InfoCard()
            {
                Title = "Purchasing in week",
                SubTitle = $"Số lượng đơn hàng mới trong tuần là: {numberOfPhoneInWeek}",
                Image = "/Images/plus.png"
            };

            InfoCard purchasinginMonthInforCard = new InfoCard()
            {
                Title = "Purchasing in week",
                SubTitle = $"Số lượng đơn hàng mới trong tháng là: {numberOfPhoneInMonth}",
                Image = "/Images/plus.png"
            };

            SalesInfor.DataContext = salesInforCard;
            PurchasinginWeekInfor.DataContext = purchasinginWeekInforCard;
            PurchasinginMonthInfor.DataContext = purchasinginMonthInforCard;
        }
        private void Dashboard_Unloaded(object sender, RoutedEventArgs e)
        {
            //
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                //this.DragMove();
            }
        }

        bool IsMaximized = false;

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (IsMaximized)
                {
                    //this.WindowState = WindowState.Normal;
                    //this.Width = 1280;
                    //this.Height = 780;

                    bool IsMaximized = false;
                }
                else
                {
                    //this.WindowState = WindowState.Maximized;
                    bool IsMaximized = true;
                }
            }
        }

    }
}
