using LiveCharts.Wpf;
using LiveCharts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HW4.DTO;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace HW4
{
    /// <summary>
    /// Interaction logic for ReportChart.xaml
    /// </summary>
    public partial class ReportChart : UserControl
    {

        private SqlConnection _connection;

        //time handling
        string current = "All time";
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
        public ReportChart()//SqlConnection connection)
        {
            InitializeComponent();
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //take password from config
            var cypherText = Convert.FromBase64String(ConfigurationManager.AppSettings["password"]);
            var entropy = Convert.FromBase64String(ConfigurationManager.AppSettings["entropy"]);
            var decryptedPassword = ProtectedData.Unprotect(cypherText, entropy, DataProtectionScope.CurrentUser);
            var realPassword = Encoding.UTF8.GetString(decryptedPassword);

            string password = realPassword;
            string username = config.AppSettings.Settings["username"].Value;
            string server = ConfigurationManager.AppSettings["Server"];
            string Database = ConfigurationManager.AppSettings["Database"];

            string connectionString = $"""
                                          Server={server};
                                          Database={Database};
                                          User ID= {username};
                                          Password= {password}; 
                                          TrustServerCertificate=True
                                          """;
            this._connection = new SqlConnection(connectionString);
        }

        private void ShowDateRange(object sender, RoutedEventArgs e)
        {
            current = "Time Range";
            start = (DateTime)startDatePicker.SelectedDate;
            end = (DateTime)endDatePicker.SelectedDate;
            if (start == null || end == null)
            {
                MessageBox.Show("Please choose both day range!", "Missing Limit!", MessageBoxButton.OK);
            }
            else if (start > end)
            {
                MessageBox.Show("Invalid Dates! Choose again", "Invalid Date!", MessageBoxButton.OK);
            }
        }

        private ObservableCollection<ORDER> ImcomeHandling(SqlConnection connection, DateTime begin, DateTime end)
        {
            //var orders = new ObservableCollection<ORDER>();
            //int skip = (1 - 1) * 10;
            //int take = 12;

            //string sql = """
            //                  select ORDERS.ORDER_ID, ORDERS.CREATED_DATE, ORDERS.TOTAL
            //                  WHERE ORDERS.CREATED_DATE BETWEEN @StartDate AND @EndDate
            //                  ORDER BY ORDERS.ORDER_ID
            //                  OFFSET @Skip ROWS
            //                  fetch next @Take rows only
            //             """;
            //if (connection.State == ConnectionState.Closed)
            //{
            //    connection.Open();
            //}

            //using (var command = new SqlCommand(sql, connection))
            //{
            //    //_connection.Open();
            //    command.Parameters.Add("@Skip", SqlDbType.Int).Value = skip;
            //    command.Parameters.Add("@Take", SqlDbType.Int).Value = take;
            //    command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = begin;
            //    command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = end;
            //    var reader = command.ExecuteReader();

            //    while (reader.Read())
            //    {
            //        int OrderID = (int)reader["ORDER_ID"];
            //        DateTime OrderDate = (DateTime)reader["CREATED_DATE"];
            //        double TotalPrice = (double)reader["TOTAL"];
            //        orders.Add(new ORDER()
            //        {
            //            OrderID = OrderID,
            //            CustomerName = null,
            //            OrderDate = OrderDate,
            //            OrderPromos = null,
            //            PromoList = null,
            //            TotalPrice = TotalPrice,
            //            status = null
            //        });
            //    }
            //}
            //connection.Close();
            //return orders;
            var result = ORDERControl.FromDateToDate(connection, 1, 12, begin, end);
            ObservableCollection<ORDER> orders = result.Item1;
            return orders;
        }

        private void chart_Loaded(object sender, RoutedEventArgs e)
        {
            var beginDate = new DateTime(2024, 4, 1);
            var endDate = end;
            ObservableCollection<ORDER> orders = ImcomeHandling(_connection, beginDate, endDate);

            TimeSpan rangeTimeSpan = endDate.Subtract(beginDate); //declared prior as TimeSpan object
            double[] incomeEachDay = new double [rangeTimeSpan.Days];
            DateTime[] timeRange = new DateTime[rangeTimeSpan.Days];

            foreach (var order in orders)
            {
                if (DateTime.Compare(order.OrderDate, beginDate) >= 0)
                {
                    TimeSpan rangeTime = order.OrderDate.Subtract(beginDate);
                    int Day = rangeTime.Days;
                    incomeEachDay[Day] += order.TotalPrice;
                }
            }

            chart.Series = new SeriesCollection()
            {
                new LineSeries()
                {
                    Title = "Doanh thu của cửa hàng",
                    Values = new ChartValues<double>(),
                    Stroke = Brushes.Red,
                    Fill = Brushes.Blue,
                    StrokeDashArray = new DoubleCollection{2}
                },
                new ColumnSeries()
                {
                    Title = "Lợi nhuận của cửa hàng",
                    Values = new ChartValues<decimal> {5, 6, 2, 7, 6},
                    Stroke = Brushes.Green,
                    StrokeThickness = 2,
                    Fill = Brushes.Yellow
                }
            };

            for (int Day = 0; Day < rangeTimeSpan.Days; Day++)
            {
                chart.Series[0].Values.Add(incomeEachDay[Day]);
            }

            chart.AxisX.Add(new Axis()
            {
                Labels = new List<string>()
            });
            
            for (int i = 0; i < rangeTimeSpan.Days; i++)
            {
                beginDate = beginDate.AddDays(1);
                timeRange[i] = beginDate;
                string haha = timeRange[i].ToString("dd/MM/yyyy");
                //chart.AxisX[0].Labels.Add(haha);
            }
        }

        private void ChartWithDateButton_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        private void ChartWithMonthButton_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        private void ChartWithYearButton_Click(object sender, RoutedEventArgs e)
        {
            //
        }
    }
}
