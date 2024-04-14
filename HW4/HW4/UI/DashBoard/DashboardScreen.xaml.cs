
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Data;
using HW4.BUS;
using System.IO;
using System.Reflection;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Media;

namespace HW4
{
    /// <summary>
    /// Interaction logic for DashboardScreen.xaml
    /// </summary>
    public partial class DashboardScreen : UserControl
    {
        SqlConnection _connection;
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
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

        private bool ShowDateRange()
        {
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

        private void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            //string folder = AppDomain.CurrentDomain.BaseDirectory;
            //var fis = new DirectoryInfo(folder).GetFiles("*.dll");

            //foreach (var fi in fis)
            //{
            //    if (fi.Name != "Microsoft.Data.SqlClient.dll")
            //    {
            //        var assembly = Assembly.LoadFrom(fi.FullName);
            //        var types = assembly.GetTypes();

            //        foreach (var type in types)
            //        {
            //            if ((type.IsClass) && typeof(IDao).IsAssignableFrom(type))
            //            {
            //                _readers.Add((IDao)Activator.CreateInstance(type)!);
            //            }
            //            else if ((type.IsClass) && typeof(IBus).IsAssignableFrom(type))
            //            {
            //                _bus.Add((IBus)Activator.CreateInstance(type)!);
            //            }
            //            else if ((type.IsClass) && typeof(IGui).IsAssignableFrom(type))
            //            {
            //                _gui.Add((IGui)Activator.CreateInstance(type)!);
            //            }
            //        }
            //    }
            //}

            var newestOderedDate = BUS_Chart.NewestOrderDate(_connection);

            // Amount of products on sales
            int amountonsales = BUS_Chart.AmountOnSales(_connection);
            InfoCard salesInforCard = new InfoCard()
            {
                Title = "On Sales",
                SubTitle = $"Số lượng sản phẩm đang bán là : {amountonsales}",
                Image = "/Images/plus.png"
            };

            //Number of Phone in week 

            var beginDate = newestOderedDate.AddDays(-7);
            var endDate = newestOderedDate;

            int numberOfPhoneInWeek = BUS_Chart.AmountProducts(_connection, beginDate, endDate);

            InfoCard purchasinginWeekInforCard = new InfoCard()
            {
                Title = "Purchasing in week",
                SubTitle = $"Số lượng đơn hàng mới trong tuần là: {numberOfPhoneInWeek}",
                Image = "/Images/plus.png"
            };

            //Number of Phone in month 

            beginDate = newestOderedDate.AddDays(-30);
            endDate = newestOderedDate;

            int numberOfPhoneInMonth = BUS_Chart.AmountProducts(_connection, beginDate, endDate);

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
        }

        private void declareQuantitySoldChartSeries()
        {
            QuantitySoldchart.Series = new SeriesCollection()
            {
                new ColumnSeries()
                {
                    Title = "Số lượng các sản phẩm đang bán",
                    Values = new ChartValues<int>(),
                    Stroke = Brushes.OrangeRed,
                    StrokeThickness = 2,
                    Fill = Brushes.OrangeRed
                }
            };

            QuantitySoldchart.AxisX.Add(new Axis()
            {
                Labels = new List<string>()
            });
        }

        private void chart_LoadedDoanhThuLoiNhuan(object sender, RoutedEventArgs e)
        {
            var newestOderedDate = BUS_Chart.NewestOrderDate(_connection);

            var beginDate = newestOderedDate.AddDays(-30);
            var endDate = newestOderedDate;


            var data = BUS_Chart.takeIncomeProfitDateStringWithDay(_connection, beginDate, endDate);
            var incomeEachDays = data.Item1;
            var profitEachDays = data.Item2;
            var dataTimestring = data.Item3;

            declareIncomeChartSeries();

            for (int Day = 0; Day < incomeEachDays.Length; Day++)
            {
                Incomechart.Series[0].Values.Add(incomeEachDays[Day]);
                Incomechart.Series[1].Values.Add(profitEachDays[Day]);
            }
            Incomechart.AxisX[0].Labels = dataTimestring;
        }

        private void chart_LoadedSanPhamSoLuong(object sender, RoutedEventArgs e)
        {
            var newestOderedDate = BUS_Chart.NewestOrderDate(_connection);

            var beginDate = newestOderedDate.AddDays(-30);
            var endDate = newestOderedDate;

            var data = BUS_Chart.takequantitysoldphonename(_connection, beginDate, endDate);

            var quantitysolds = data.Item1;
            var phoneName = data.Item2;


            declareQuantitySoldChartSeries();

            foreach (int quantitysold in quantitysolds)
                QuantitySoldchart.Series[0].Values.Add(quantitysold);

            QuantitySoldchart.AxisX[0].Labels = phoneName;
        }

        private void chartDoanhThuLoiNhuanTheoNgay()
        {
            var beginDate = start;
            var endDate = end;
            
            var data = BUS_Chart.takeIncomeProfitDateStringWithDay(_connection, beginDate, endDate);
            var incomeEachDays = data.Item1;
            var profitEachDays = data.Item2;
            var dataTimestring = data.Item3;

            declareIncomeChartSeries();

            for (int Day = 0; Day < incomeEachDays.Length; Day++)
            {
                Incomechart.Series[0].Values.Add(incomeEachDays[Day]);
                Incomechart.Series[1].Values.Add(profitEachDays[Day]);
            }
            Incomechart.AxisX[0].Labels = dataTimestring;
        }

        private void chartSanPhamSoLuongTheoNgay()
        {
            var beginDate = start;
            var endDate = end;

            var data = BUS_Chart.takequantitysoldphonename(_connection, beginDate, endDate);

            var quantitysolds = data.Item1;
            var phoneName = data.Item2;


            declareQuantitySoldChartSeries();

            foreach (int quantitysold in quantitysolds)
                QuantitySoldchart.Series[0].Values.Add(quantitysold);

            QuantitySoldchart.AxisX[0].Labels = phoneName;
        }

        private void ChartWithDateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShowDateRange() == false) return;
            chartDoanhThuLoiNhuanTheoNgay();
            chartSanPhamSoLuongTheoNgay();
        }

        private void chartDoanhThuLoiNhuanTheoThang()
        {
            var beginDate = start;
            var endDate = end;

            var data = BUS_Chart.takeIncomeProfitDateStringWithMonth(_connection, beginDate, endDate);
            var incomeEachMonths = data.Item1;
            var profitEachMonths = data.Item2;
            var dataTimestring = data.Item3;

            declareIncomeChartSeries();

            for (int Month = 0; Month < incomeEachMonths.Length; Month++)
            {
                Incomechart.Series[0].Values.Add(incomeEachMonths[Month]);
                Incomechart.Series[1].Values.Add(profitEachMonths[Month]);
            }
            Incomechart.AxisX[0].Labels = dataTimestring;
        }

        private void ChartWithMonthButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShowDateRange() == false) return;
            chartDoanhThuLoiNhuanTheoThang();
            chartSanPhamSoLuongTheoNgay();
        }

        private void chartDoanhThuLoiNhuanTheoNam()
        {
            var beginDate = start;
            var endDate = end;

            var data = BUS_Chart.takeIncomeProfitDateStringWithYear(_connection, beginDate, endDate);
            var incomeEachYears = data.Item1;
            var profitEachYears = data.Item2;
            var dataTimestring = data.Item3;

            declareIncomeChartSeries();

            for (int Year = 0; Year < incomeEachYears.Length; Year++)
            {
                Incomechart.Series[0].Values.Add(incomeEachYears[Year]);
                Incomechart.Series[1].Values.Add(profitEachYears[Year]);
            }
            Incomechart.AxisX[0].Labels = dataTimestring;
        }

        private void ChartWithYearButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShowDateRange() == false) return;
            chartDoanhThuLoiNhuanTheoNam();
            chartSanPhamSoLuongTheoNgay();
        }

    }
}
