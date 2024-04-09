﻿using LiveCharts.Wpf;
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
        string LegendChart;
        private SqlConnection _connection;

        //time handling
        string current = "All time";
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
        string _timeType = "Ngày";
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

        private DateTime TakeMinMaxOrderDate(SqlConnection connection)
        {
            DateTime newestOrderedDate = new DateTime();

            string sql = """
                              select MAX(CREATED_DATE) AS 'NEWESTORDEREDDATE'
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
                }
                reader.Close();
            }
            connection.Close();

            var res = newestOrderedDate;

            return res;
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
            chart.AxisX.Add(new Axis()
            {
                Title = _timeType,
                Labels = new List<string>()
            });
        }

        private void chart_Loaded(object sender, RoutedEventArgs e)
        {
            var newestOderedDate = TakeMinMaxOrderDate(_connection);

            var beginDate = newestOderedDate.AddDays(-30);
            var endDate = newestOderedDate;
            List<double> profits;
            ObservableCollection<INCOMECHART> incomechartlists = new ObservableCollection<INCOMECHART>();
            incomechartlists = ImcomeHandling(_connection, beginDate, endDate);

            TimeSpan rangeTimeSpan = endDate.Subtract(beginDate); //declared prior as TimeSpan object
            int DaysSpan = rangeTimeSpan.Days + 1;
            double[] incomeEachDay = new double [DaysSpan];
            double[] profitEachDay = new double[DaysSpan];
            DateTime[] timeRange = new DateTime[DaysSpan];

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

            for (int Day = 0; Day < DaysSpan; Day++)
            {
                chart.Series[0].Values.Add(incomeEachDay[Day]);
                chart.Series[1].Values.Add(profitEachDay[Day]);
            }

            //Store a DateTimes String 
            var datetimeString = new List<string>();
            for (int Day = 0; Day < DaysSpan; Day++)
            {
                timeRange[Day] = beginDate;
                datetimeString.Add(timeRange[Day].ToString("dd/MM/yyyy"));
                beginDate = beginDate.AddDays(1);
            }
            chart.AxisX[0].Labels = datetimeString;
            LegendChart = "Đây là bảng báo cáo về thu nhập và lợi nhuận của cửa hàng trong 30 ngày gần đây";
        }

        private void ChartWithDateButton_Click(object sender, RoutedEventArgs e)
        {
            _timeType = "Ngày";
            if (ShowDateRange() == false) return;

            var beginDate = start;
            var endDate = end;

            List<double> profits;
            ObservableCollection<INCOMECHART> incomechartlists = new ObservableCollection<INCOMECHART>();
            incomechartlists = ImcomeHandling(_connection, beginDate, endDate);

            TimeSpan rangeTimeSpan = endDate.Subtract(beginDate); //declared prior as TimeSpan object
            int DaysSpan = rangeTimeSpan.Days + 1;

            double[] incomeEachDay = new double[DaysSpan];
            double[] profitEachDay = new double[DaysSpan];
            DateTime[] timeRange = new DateTime[DaysSpan];

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

            for (int Day = 0; Day < DaysSpan; Day++)
            {
                chart.Series[0].Values.Add(incomeEachDay[Day]);
                chart.Series[1].Values.Add(profitEachDay[Day]);
            }


            //Store a DateTimes String 
            var datetimeString = new List<string>();
            for (int Day = 0; Day < DaysSpan; Day++)
            {
                timeRange[Day] = beginDate;
                datetimeString.Add(timeRange[Day].ToString("dd/MM/yyyy"));
                beginDate = beginDate.AddDays(1);
            }
            chart.AxisX[0].Labels = datetimeString;
        }

        private void ChartWithMonthButton_Click(object sender, RoutedEventArgs e)
        {
            _timeType = "Tháng";
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
            _timeType = "Năm";
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
                Title = _timeType,
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
