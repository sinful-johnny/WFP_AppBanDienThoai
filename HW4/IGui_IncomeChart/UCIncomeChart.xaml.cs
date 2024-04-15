using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data;

namespace IGui_IncomeChart
{
    /// <summary>
    /// Interaction logic for UCIncomeChart.xaml
    /// </summary>
    public partial class UCIncomeChart : UserControl
    {
        private DataTable _dataTable;
        private DateTime _beginDate;
        private DateTime _endDate;
        private int _method;


        private double[] _incomeEachDays;
        private double[] _profitEachDays;
        private List<string> _datetimeString;

        //private string _queryString;
        public UCIncomeChart(DataTable dataTable, DateTime beginDate, DateTime endDate, int method)
        {
            InitializeComponent();
            _dataTable = dataTable;
            _beginDate = beginDate;
            _endDate = endDate;
            _method = method;
            declareIncomeChartSeries();
        }

        private Tuple<double[], double[], List<string>> takeIncomeProfitDateStringWithDay()
        {
            var incomechartlists = _dataTable;

            TimeSpan rangeTimeSpan = _endDate.Subtract(_beginDate); //declared prior as TimeSpan object
            int DaysSpan = rangeTimeSpan.Days + 1;
            double[] incomeEachDay = new double[DaysSpan];
            double[] profitEachDay = new double[DaysSpan];
            DateTime[] timeRange = new DateTime[DaysSpan];

            foreach (DataRow row in incomechartlists.Rows)
            {
                var orderedDate = (DateTime)row["CREATED_DATE"];
                TimeSpan rangeTime = orderedDate.Subtract(_beginDate);
                int Day = rangeTime.Days;
                incomeEachDay[Day] += (double)row["TOTAL"];
                profitEachDay[Day] += (double)row["PROFIT"];
            }

            //Store a DateTimes String 
            var datetimeString = new List<string>();
            DateTime start = _beginDate;
            for (int Day = 0; Day < DaysSpan; Day++)
            {
                timeRange[Day] = start;
                datetimeString.Add(timeRange[Day].ToString("dd/MM/yyyy"));
                start = start.AddDays(1);
            }

            var res = new Tuple<double[], double[], List<string>>(incomeEachDay, profitEachDay, datetimeString);

            return res;
        }

        private Tuple<double[], double[], List<string>> takeIncomeProfitDateStringWithMonth()
        {
            var incomechartlists = _dataTable;

            int rangeMonthSpan = (_endDate.Month - _beginDate.Month)
                                 + 12 * (_endDate.Year - _beginDate.Year) + 1;
            double[] incomeEachDay = new double[rangeMonthSpan];
            double[] profitEachDay = new double[rangeMonthSpan];
            DateTime[] MonthRange = new DateTime[rangeMonthSpan];

            foreach (DataRow row in incomechartlists.Rows)
            {
                var orderedDate = (DateTime)row["CREATED_DATE"];
                int Month = (orderedDate.Month - _beginDate.Month)
                            + 12 * (orderedDate.Year - _beginDate.Year);
                incomeEachDay[Month] += (double)row["TOTAL"];
                profitEachDay[Month] += (double)row["PROFIT"];
            }


            //Store a DateTimes String 
            var datetimeMonthString = new List<string>();
            DateTime start = _beginDate;

            for (int Month = 0; Month < rangeMonthSpan; Month++)
            {
                MonthRange[Month] = start;
                datetimeMonthString.Add(MonthRange[Month].ToString("MM/yyyy"));
                start = start.AddMonths(1);
            }

            var res = new Tuple<double[], double[], List<string>>(incomeEachDay, profitEachDay, datetimeMonthString);

            return res;
        }

        private Tuple<double[], double[], List<string>> takeIncomeProfitDateStringWithYear()
        {
            var incomechartlists = _dataTable;

            int rangeYearSpan = _endDate.Year - _beginDate.Year + 1;
            double[] incomeEachDay = new double[rangeYearSpan];
            double[] profitEachDay = new double[rangeYearSpan];
            DateTime[] YearRange = new DateTime[rangeYearSpan];

            foreach (DataRow row in incomechartlists.Rows)
            {
                var orderedDate = (DateTime)row["CREATED_DATE"];
                int Year = orderedDate.Year - _beginDate.Year;
                incomeEachDay[Year] += (double)row["TOTAL"];
                profitEachDay[Year] += (double)row["PROFIT"];
            }

            //Store a DateTimes String 
            var datetimeYearString = new List<string>();
            DateTime start = _beginDate;

            for (int Year = 0; Year < rangeYearSpan; Year++)
            {
                YearRange[Year] = start;
                datetimeYearString.Add(YearRange[Year].ToString("yyyy"));
                start = start.AddYears(1);
            }

            var res = new Tuple<double[], double[], List<string>>(incomeEachDay, profitEachDay, datetimeYearString);

            return res;
        }

        private void declareIncomeChartSeries()
        {
            Incomechart.Series = new SeriesCollection()
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

            Incomechart.AxisX.Add(new Axis()
            {
                Labels = new List<string>()
            });

            if (_method == 0 || _method == 1)
            {
                var res = takeIncomeProfitDateStringWithDay();
                _incomeEachDays = res.Item1;
                _profitEachDays = res.Item2;
                _datetimeString = res.Item3;
            }
            else if (_method == 2)
            {
                var res = takeIncomeProfitDateStringWithMonth();
                _incomeEachDays = res.Item1;
                _profitEachDays = res.Item2;
                _datetimeString = res.Item3;
            }
            else if (_method == 3)
            {
                var res = takeIncomeProfitDateStringWithYear();
                _incomeEachDays = res.Item1;
                _profitEachDays = res.Item2;
                _datetimeString = res.Item3;
            }

            for (int timespans = 0; timespans < _incomeEachDays.Length; timespans++)
            {
                Incomechart.Series[0].Values.Add(_incomeEachDays[timespans]);
                Incomechart.Series[1].Values.Add(_profitEachDays[timespans]);
            }
            Incomechart.AxisX[0].Labels = _datetimeString;
        }

    }
}
