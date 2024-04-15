using System.Windows;
using System.Windows.Controls;
using HW4.DTO;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using Incomechart;
using QuantitySoldchart;

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

        private bool ShowDateRange()
        {
            current = "Time Range";
            if (startDatePicker.SelectedDate == null || endDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Please choose both day range!", "Missing Limit!", MessageBoxButton.OK);
                return false;
            }
            else if (startDatePicker.SelectedDate > endDatePicker.SelectedDate)
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
        private Tuple<ObservableCollection<INCOMECHART>, ObservableCollection<QUANTITYSOLDCHART>, int> ChartDataHandling(SqlConnection connection, DateTime begin, DateTime end)
        {
            var incomechartlist = new ObservableCollection<INCOMECHART>();
            var quantitysoldchartlist = new ObservableCollection<QUANTITYSOLDCHART>();

            string sql = """
                              select O.ORDER_ID, O.CREATED_DATE, O.TOTAL, 
                                     (O.TOTAL - P.ORIGINALPRICE*OP.PHONE_COUNT) AS 'PROFIT'
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
                        ProfitAll = (double)reader["PROFIT"];

                    incomechartlist.Add(new INCOMECHART()
                    {
                        OrderDate = OrderDate,
                        TotalPrice = TotalPrice,
                        Profit = ProfitAll
                    });
                }
                reader.Close();
            }

            string sql2 = """
                             select distinct P.NAME, SUM(OP.PHONE_COUNT) AS 'QUANTITY_SOLD'
                             from ORDERS O, ORDERS_PHONE OP, PHONE P
                             where P.ID = OP.PHONE_ID
                                 and OP.ORDER_ID = O.ORDER_ID
                                 and O.CREATED_DATE BETWEEN @StartDate AND @EndDate
                             group by P.NAME
                             order by 'QUANTITY_SOLD'
                          """;

            int numberOfPhone = 0;

            using (var command2 = new SqlCommand(sql2, connection))
            {
                //_connection.Open();
                command2.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = begin;
                command2.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = end;

                //_connection.Open();
                var reader2 = command2.ExecuteReader();

                while (reader2.Read())
                {
                    string PhoneName = (string)reader2["NAME"];

                    int QuantitySold = (int)reader2["QUANTITY_SOLD"];

                    quantitysoldchartlist.Add(new QUANTITYSOLDCHART()
                    {
                        PhoneName = PhoneName,
                        QuantitySold = QuantitySold
                    });
                    numberOfPhone++;
                }
                reader2.Close();
            }
            connection.Close();

            var res = new Tuple<ObservableCollection<INCOMECHART>, 
                                ObservableCollection<QUANTITYSOLDCHART>, int>
                                (incomechartlist, quantitysoldchartlist, numberOfPhone);

            return res;
        }

        private ObservableCollection<INCOMECHART> ImcomeHandling(SqlConnection connection, DateTime begin, DateTime end)
        {
            var selected = ChartDataHandling(connection, begin, end);
            return selected.Item1;
        }

        private Tuple<ObservableCollection<QUANTITYSOLDCHART>, int> QuantitySoldHandling(SqlConnection connection, DateTime begin, DateTime end)
        {
            var selected = ChartDataHandling(connection, begin, end);
            var res = new Tuple<ObservableCollection<QUANTITYSOLDCHART>, int>
                                (selected.Item2, selected.Item3);
            return res;
        }

        private void declareIncomeChartSeries()
        {
            var incomeChart = new IncomeChart();
            Incomechart.Series = incomeChart.declareChartSeries().Item1;
            Incomechart.AxisX.Add(incomeChart.declareChartSeries().Item2);
        }

        private void declareQuantitySoldChartSeries()
        {
            var quantitysoldChart = new QuantitySoldChart();
            QuantitySoldchart.Series = quantitysoldChart.declareChartSeries().Item1;
            QuantitySoldchart.AxisX.Add(quantitysoldChart.declareChartSeries().Item2);
        }

        private void chart_LoadedDoanhThuLoiNhuan(object sender, RoutedEventArgs e)
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

            declareIncomeChartSeries();

            for (int Day = 0; Day < DaysSpan; Day++)
            {
                Incomechart.Series[0].Values.Add(incomeEachDay[Day]);
                Incomechart.Series[1].Values.Add(profitEachDay[Day]);
            }

            //Store a DateTimes String 
            var datetimeString = new List<string>();
            for (int Day = 0; Day < DaysSpan; Day++)
            {
                timeRange[Day] = beginDate;
                datetimeString.Add(timeRange[Day].ToString("dd/MM/yyyy"));
                beginDate = beginDate.AddDays(1);
            }
            Incomechart.AxisX[0].Labels = datetimeString;
        }

        private void chartDoanhThuLoiNhuanTheoNgay()
        {
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

            declareIncomeChartSeries();

            for (int Day = 0; Day < DaysSpan; Day++)
            {
                Incomechart.Series[0].Values.Add(incomeEachDay[Day]);
                Incomechart.Series[1].Values.Add(profitEachDay[Day]);
            }


            //Store a DateTimes String 
            var datetimeString = new List<string>();
            for (int Day = 0; Day < DaysSpan; Day++)
            {
                timeRange[Day] = beginDate;
                datetimeString.Add(timeRange[Day].ToString("dd/MM/yyyy"));
                beginDate = beginDate.AddDays(1);
            }
            Incomechart.AxisX[0].Labels = datetimeString;
        }

        private void chartSanPhamSoLuongTheoNgay()
        {
            if (ShowDateRange() == false) return;

            var beginDate = start;
            var endDate = end;

            var newestOderedDate = TakeMinMaxOrderDate(_connection);

            var selected = QuantitySoldHandling(_connection, beginDate, endDate);

            var quantitysoldchartlists = new ObservableCollection
                                            <QUANTITYSOLDCHART>();

            quantitysoldchartlists = selected.Item1;
            int numberOfPhone = selected.Item2;

            int[] quantitysold = new int[numberOfPhone];



            declareQuantitySoldChartSeries();

            var phoneName = new List<string>();

            for (int i = 0; i < numberOfPhone; i++)
            {
                QuantitySoldchart.Series[0].Values.Add(quantitysoldchartlists[i].QuantitySold);
                phoneName.Add(quantitysoldchartlists[i].PhoneName);
            }

            QuantitySoldchart.AxisX[0].Labels = phoneName;
        }

        private void ChartWithDateButton_Click(object sender, RoutedEventArgs e)
        {
            chartDoanhThuLoiNhuanTheoNgay();
            chartSanPhamSoLuongTheoNgay();
        }

        private void chartDoanhThuLoiNhuanTheoThang()
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

            declareIncomeChartSeries();

            for (int Month = 0; Month < rangeMonthSpan; Month++)
            {
                Incomechart.Series[0].Values.Add(incomeEachDay[Month]);
                Incomechart.Series[1].Values.Add(profitEachDay[Month]);
            }


            //Store a DateTimes String 
            var datetimeMonthString = new List<string>();
            for (int Month = 0; Month < rangeMonthSpan; Month++)
            {
                MonthRange[Month] = beginDate;
                datetimeMonthString.Add(MonthRange[Month].ToString("MM/yyyy"));
                beginDate = beginDate.AddMonths(1);
            }
            Incomechart.AxisX[0].Labels = datetimeMonthString;
        }

        private void ChartWithMonthButton_Click(object sender, RoutedEventArgs e)
        {
            chartDoanhThuLoiNhuanTheoThang();
            chartSanPhamSoLuongTheoNgay();
        }

        private void chartDoanhThuLoiNhuanTheoNam()
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

            declareIncomeChartSeries();

            for (int Year = 0; Year < rangeYearSpan; Year++)
            {
                Incomechart.Series[0].Values.Add(incomeEachDay[Year]);
                Incomechart.Series[1].Values.Add(profitEachDay[Year]);
            }

            //Store a DateTimes String 
            var datetimeYearString = new List<string>();
            for (int Year = 0; Year < rangeYearSpan; Year++)
            {
                YearRange[Year] = beginDate;
                datetimeYearString.Add(YearRange[Year].ToString("MM/yyyy"));
                beginDate = beginDate.AddMonths(1);
            }
            Incomechart.AxisX[0].Labels = datetimeYearString;
        }

        private void ChartWithYearButton_Click(object sender, RoutedEventArgs e)
        {
            chartDoanhThuLoiNhuanTheoNam();
            chartSanPhamSoLuongTheoNgay();
        }

        private void chart_LoadedSanPhamSoLuong(object sender, RoutedEventArgs e)
        {
            var newestOderedDate = TakeMinMaxOrderDate(_connection);

            var beginDate = newestOderedDate.AddDays(-30);
            var endDate = newestOderedDate;

            var selected = QuantitySoldHandling(_connection, beginDate, endDate);

            var quantitysoldchartlists = new ObservableCollection
                                            <QUANTITYSOLDCHART>();
            
            quantitysoldchartlists = selected.Item1;
            int numberOfPhone = selected.Item2;

            int[] quantitysold = new int[numberOfPhone];

            declareQuantitySoldChartSeries();

            var phoneName = new List<string>();

            for (int i = 0; i < numberOfPhone; i++)
            {
                QuantitySoldchart.Series[0].Values.Add(quantitysoldchartlists[i].QuantitySold);
                phoneName.Add(quantitysoldchartlists[i].PhoneName);
            }
            QuantitySoldchart.AxisX[0].Labels = phoneName;
        }
    }
}
