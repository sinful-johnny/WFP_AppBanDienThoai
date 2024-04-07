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
using System.ComponentModel;

namespace HW4
{
    /// <summary>
    /// Interaction logic for ReportChart.xaml
    /// </summary>
    public partial class ReportChart : UserControl
    {

        private SqlConnection _connection;
        ObservableCollection<INCOMECHART> _IncomeChartList;

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

        private bool ShowDateRange()
        {
            current = "Time Range";
            if (startDatePicker.SelectedDate == null || endDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Please choose both day range!", "Missing Limit!", MessageBoxButton.OK);
                return false;
            }
            else if (start > end)
            {
                MessageBox.Show("Invalid Dates! Choose again", "Invalid Date!", MessageBoxButton.OK);
                return false;
            }

            start = (DateTime)startDatePicker.SelectedDate;
            end = (DateTime)endDatePicker.SelectedDate;
            return true;
        }

        private ObservableCollection<INCOMECHART> ImcomeHandling(SqlConnection connection, DateTime begin, DateTime end)
        {
            var incomechartlist = new ObservableCollection<INCOMECHART>();

            string sql = """
                              select O.ORDER_ID, O.CREATED_DATE, O.TOTAL, (O.TOTAL - P.ORIGINALPRICE*OP.PHONE_COUNT) AS 'PROFIT'
                              from ORDERS O, ORDERS_PHONE OP, PHONE P
                              where OP.PHONE_ID = P.ID
                         	    and OP.ORDER_ID = O.ORDER_ID
                                and CREATED_DATE BETWEEN @StartDate AND @EndDate
                         """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            using (var command = new SqlCommand(sql, connection))
            {
                //_connection.Open();
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = begin;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = end;

                //_connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int OrderID = (int)reader["ORDER_ID"];
                    DateTime OrderDate = (DateTime)reader["CREATED_DATE"];
                    double TotalPrice = 0;
                    double ProfitAll = 0;
                    if (reader["TOTAL"].GetType() != typeof(DBNull))
                        TotalPrice = (double)reader["TOTAL"];
                    if (reader["PROFIT"].GetType() != typeof(DBNull))
                    {
                        ProfitAll = (double)reader["PROFIT"];
                    }
                    incomechartlist.Add(new INCOMECHART()
                    {
                        OrderDate = OrderDate,
                        TotalPrice = TotalPrice,
                        Profit = ProfitAll
                    });
                }
            }
            connection.Close();
            return incomechartlist;
        }

        private void declareChartSeries()
        {
            chart.Series = new SeriesCollection()
            {
                new ColumnSeries()
                {
                    Title = "Doanh thu của cửa hàng",
                    Values = new ChartValues<double>(),
                    Stroke = Brushes.OrangeRed,
                    StrokeThickness = 2,
                    Fill = Brushes.OrangeRed
                },
                new LineSeries()
                {
                    Title = "Lợi nhuận của cửa hàng",
                    Values = new ChartValues<double>(),
                    Stroke = Brushes.DarkRed,
                    StrokeDashArray = new DoubleCollection{1}
                }
            };
        }

        private void chart_Loaded(object sender, RoutedEventArgs e)
        {
            var beginDate = new DateTime(2024, 4, 1);
            var endDate = end;
            List<double> profits;
            ObservableCollection<INCOMECHART> incomechartlists = new ObservableCollection<INCOMECHART>();
            incomechartlists = ImcomeHandling(_connection, beginDate, endDate);

            TimeSpan rangeTimeSpan = endDate.Subtract(beginDate); //declared prior as TimeSpan object
            double[] incomeEachDay = new double [rangeTimeSpan.Days];
            double[] profitEachDay = new double[rangeTimeSpan.Days];
            DateTime[] timeRange = new DateTime[rangeTimeSpan.Days];

            foreach (var incomechartlist in incomechartlists)
            {
                //if (DateTime.Compare(incomechartlist.OrderDate, beginDate) >= 0)
                //{
                //    TimeSpan rangeTime = incomechartlist.OrderDate.Subtract(beginDate);
                //    int Day = rangeTime.Days;
                //    incomeEachDay[Day] += incomechartlist.TotalPrice;
                //}
                //TimeSpan rangeTime = incomechartlist.OrderDate.Subtract(beginDate);
                //int Day = rangeTime.Days;
                //incomeEachDay[Day] += incomechartlist.TotalPrice;
                //profitEachDay[Day] += incomechartlist.Profit;
            }

            declareChartSeries();

            for (int Day = 0; Day < rangeTimeSpan.Days; Day++)
            {
                chart.Series[0].Values.Add(incomeEachDay[Day]);
                chart.Series[1].Values.Add(profitEachDay[Day]);
            }

            chart.AxisX.Add(new Axis()
            {
                Labels = new List<string>()
            });

            //Store a DateTimes String 
            var datetimeString = new List<string>();
            for (int Day = 0; Day < rangeTimeSpan.Days; Day++)
            {
                timeRange[Day] = beginDate;
                datetimeString.Add(timeRange[Day].ToString("dd/MM/yyyy"));
                beginDate = beginDate.AddDays(1);
            }
            chart.AxisX[0].Labels = datetimeString;
        }

        private void ChartWithDateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShowDateRange() == false) return;

            var beginDate = start;
            var endDate = end;

            List<double> profits;
            ObservableCollection<INCOMECHART> incomechartlists = new ObservableCollection<INCOMECHART>();
            incomechartlists = ImcomeHandling(_connection, beginDate, endDate);

            TimeSpan rangeTimeSpan = endDate.Subtract(beginDate); //declared prior as TimeSpan object
            double[] incomeEachDay = new double[rangeTimeSpan.Days];
            double[] profitEachDay = new double[rangeTimeSpan.Days];
            DateTime[] timeRange = new DateTime[rangeTimeSpan.Days];

            foreach (var incomechartlist in incomechartlists)
            {
                //if (DateTime.Compare(incomechartlist.OrderDate, beginDate) >= 0)
                //{
                //    TimeSpan rangeTime = incomechartlist.OrderDate.Subtract(beginDate);
                //    int Day = rangeTime.Days;
                //    incomeEachDay[Day] += incomechartlist.TotalPrice;
                //}
                TimeSpan rangeTime = incomechartlist.OrderDate.Subtract(beginDate);
                int Day = rangeTime.Days;
                incomeEachDay[Day] += incomechartlist.TotalPrice;
                profitEachDay[Day] += incomechartlist.Profit;
            }

            declareChartSeries();

            for (int Day = 0; Day < rangeTimeSpan.Days; Day++)
            {
                chart.Series[0].Values.Add(incomeEachDay[Day]);
                chart.Series[1].Values.Add(profitEachDay[Day]);
            }

            chart.AxisX.Add(new Axis()
            {
                Labels = new List<string>()
            });

            //Store a DateTimes String 
            var datetimeString = new List<string>();
            for (int Day = 0; Day < rangeTimeSpan.Days; Day++)
            {
                timeRange[Day] = beginDate;
                datetimeString.Add(timeRange[Day].ToString("dd/MM/yyyy"));
                beginDate = beginDate.AddDays(1);
            }
            chart.AxisX[0].Labels = datetimeString;
        }

        private void ChartWithMonthButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShowDateRange() == false) return;

            var beginDate = start;
            var endDate = end;

            List<double> profits;
            ObservableCollection<INCOMECHART> incomechartlists = new ObservableCollection<INCOMECHART>();
            incomechartlists = ImcomeHandling(_connection, beginDate, endDate);

            int rangeMonthSpan = (endDate.Month - beginDate.Month) 
                                 + 12 * (endDate.Year - beginDate.Year) + 1;
            double[] incomeEachDay = new double[rangeMonthSpan];
            double[] profitEachDay = new double[rangeMonthSpan];
            DateTime[] MonthRange = new DateTime[rangeMonthSpan];

            foreach (var incomechartlist in incomechartlists)
            {
                //if (DateTime.Compare(incomechartlist.OrderDate, beginDate) >= 0)
                //{
                //    TimeSpan rangeTime = incomechartlist.OrderDate.Subtract(beginDate);
                //    int Day = rangeTime.Days;
                //    incomeEachDay[Day] += incomechartlist.TotalPrice;
                //}
                int Month = (incomechartlist.OrderDate.Month - beginDate.Month)
                            + 12 * (incomechartlist.OrderDate.Year - beginDate.Year);
                incomeEachDay[Month] += incomechartlist.TotalPrice;
                profitEachDay[Month] += incomechartlist.Profit;
            }

            declareChartSeries();

            for (int Month = 0; Month < rangeMonthSpan; Month++)
            {
                chart.Series[0].Values.Add(incomeEachDay[Month]);
                chart.Series[1].Values.Add(profitEachDay[Month]);
            }

            chart.AxisX.Add(new Axis()
            {
                Labels = new List<string>()
            });

            //Store a DateTimes String 
            var datetimeMonthString = new List<string>();
            for (int Month = 0; Month < rangeMonthSpan; Month++)
            {
                MonthRange[Month] = beginDate;
                datetimeMonthString.Add(MonthRange[Month].ToString("MM/yyyy"));
                beginDate = beginDate.AddMonths(1);
            }
            chart.AxisX[0].Labels = datetimeMonthString;
        }

        private void ChartWithYearButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShowDateRange() == false) return;

            var beginDate = start;
            var endDate = end;

            List<double> profits;
            ObservableCollection<INCOMECHART> incomechartlists = new ObservableCollection<INCOMECHART>();
            incomechartlists = ImcomeHandling(_connection, beginDate, endDate);

            int rangeYearSpan = endDate.Year - beginDate.Year + 1;
            double[] incomeEachDay = new double[rangeYearSpan];
            double[] profitEachDay = new double[rangeYearSpan];
            DateTime[] YearRange = new DateTime[rangeYearSpan];

            foreach (var incomechartlist in incomechartlists)
            {
                //if (DateTime.Compare(incomechartlist.OrderDate, beginDate) >= 0)
                //{
                //    TimeSpan rangeTime = incomechartlist.OrderDate.Subtract(beginDate);
                //    int Day = rangeTime.Days;
                //    incomeEachDay[Day] += incomechartlist.TotalPrice;
                //}
                int Year = incomechartlist.OrderDate.Year - beginDate.Year;
                incomeEachDay[Year] += incomechartlist.TotalPrice;
                profitEachDay[Year] += incomechartlist.Profit;
            }

            declareChartSeries();

            for (int Year = 0; Year < rangeYearSpan; Year++)
            {
                chart.Series[0].Values.Add(incomeEachDay[Year]);
                chart.Series[1].Values.Add(profitEachDay[Year]);
            }

            chart.AxisX.Add(new Axis()
            {
                Labels = new List<string>()
            });

            //Store a DateTimes String 
            var datetimeYearString = new List<string>();
            for (int Year = 0; Year < rangeYearSpan; Year++)
            {
                YearRange[Year] = beginDate;
                datetimeYearString.Add(YearRange[Year].ToString("MM/yyyy"));
                beginDate = beginDate.AddMonths(1);
            }
            chart.AxisX[0].Labels = datetimeYearString;
        }
    }
}
