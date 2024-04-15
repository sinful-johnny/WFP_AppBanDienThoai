using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.ComponentModel;
using HW4.BUS;
using IGuiChart;
using System.IO;
using System.Reflection;
using System.Data;

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

        public List<IGui> IGuiForChart = new List<IGui>();

        //Query String
        string _queryNewestDateString;
        string _queryAmountProductsString;
        string _queryAmountOnSalesString;
        string _queryGetTop10Products;

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

            string folder = AppDomain.CurrentDomain.BaseDirectory;
            var fis = new DirectoryInfo(folder).GetFiles("*.dll");

            foreach (var fi in fis)
            {
                if (fi.Name != "Microsoft.Data.SqlClient.dll" && fi.Name != "System.Data.SqlClient.dll")
                {
                    var assembly = Assembly.LoadFrom(fi.FullName);
                    var types = assembly.GetTypes();

                    foreach (var type in types)
                    {
                        if ((type.IsClass) && typeof(IGui).IsAssignableFrom(type))
                            IGuiForChart.Add((IGui)Activator.CreateInstance(type)!);
                    }
                }
            }

            _queryNewestDateString = """
                                        select MAX(CREATED_DATE) AS 'NEWESTORDEREDDATE'
                                        from ORDERS
                                     """;

            _queryAmountProductsString = """
                                            select ORDER_ID
                                            from ORDERS
                                            where CREATED_DATE BETWEEN @StartDate AND @EndDate
                                         """;

            _queryAmountOnSalesString = """
                                           select count(distinct PHONE_ID) AS 'NUMBER'
                                           from ORDERS_PHONE
                                        """;

            _queryGetTop10Products = """
                                        select top 10 PHONE.NAME AS 'PHONE_NAME', PHONE.THUMBNAIL, PHONE.PRICE, SUM(ORDERS_PHONE.PHONE_COUNT) AS 'SOLD'
                                        from ORDERS_PHONE JOIN PHONE ON  ORDERS_PHONE.PHONE_ID = PHONE.ID, ORDERS O
                                        where O.ORDER_ID = ORDERS_PHONE.ORDER_ID
                                            and O.CREATED_DATE BETWEEN @StartDate AND @EndDate
                                        group by PHONE.NAME, PHONE.PRICE,PHONE.THUMBNAIL
                                        order by SOLD desc
                                     """;
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

            var newestOderedDate = BUS_Chart.NewestOrderDate(_connection, _queryNewestDateString);

            // Amount of products on sales
            int amountonsales = BUS_Chart.AmountOnSales(_connection, _queryAmountOnSalesString);
            InfoCard salesInforCard = new InfoCard()
            {
                Title = "On Sales",
                SubTitle = $"Số lượng sản phẩm đang bán là : {amountonsales}",
                Image = "/Images/plus.png"
            };

            //Number of Phone in week 

            var beginDate = newestOderedDate.AddDays(-7);
            var endDate = newestOderedDate;

            int numberOfPhoneInWeek = BUS_Chart.AmountProducts(_connection, beginDate, endDate, _queryAmountProductsString);

            InfoCard purchasinginWeekInforCard = new InfoCard()
            {
                Title = "Purchasing in week",
                SubTitle = $"Số lượng đơn hàng mới trong tuần là: {numberOfPhoneInWeek}",
                Image = "/Images/plus.png"
            };

            //Number of Phone in month 

            beginDate = newestOderedDate.AddDays(-30);
            endDate = newestOderedDate;

            int numberOfPhoneInMonth = BUS_Chart.AmountProducts(_connection, beginDate, endDate, _queryAmountProductsString);

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

        private void chart_Loaded(object sender, RoutedEventArgs e)
        {
            int method = 0;
            
            var newestOderedDate = BUS_Chart.NewestOrderDate(_connection, _queryNewestDateString);

            var beginDate = newestOderedDate.AddDays(-30);
            var endDate = newestOderedDate;

            StackPanelChart.Children.RemoveRange(0, IGuiForChart.Count);

            foreach (var IGuiChart in IGuiForChart)
            {
                var data = BUS_Chart.takeData(_connection, beginDate, endDate, IGuiChart.queryString);
                IGuiChart.setData(data);
                IGuiChart.setDateMethod(beginDate, endDate, method);
                var myControl = IGuiChart.display;
                StackPanelChart.Children.Add(myControl);
            }

            //Display top 10 products on sales in 30days before
            var top10Products = BUS_Chart.takeData(_connection, beginDate, endDate, _queryGetTop10Products);

            var cart = new BindingList<ORDEREDPHONE>();

            foreach (DataRow row in top10Products.Rows)
            {
                ORDEREDPHONE newPhone = new ORDEREDPHONE()
                {
                    Price = (double)row["PRICE"],
                    PhoneName = (string)row["PHONE_NAME"],
                    quantity = (int)row["SOLD"],
                    Thumbnail = (string)row["THUMBNAIL"]
                };

                cart.Add(newPhone);
            }
            PhoneListView.ItemsSource = cart;
        }

        private void ChartWithDateButton_Click(object sender, RoutedEventArgs e)
        {
            int method = 1;

            if (ShowDateRange() == false) return;
            var beginDate = start;
            var endDate = end;

            StackPanelChart.Children.RemoveRange(0, IGuiForChart.Count);

            foreach (var IGuiChart in IGuiForChart)
            {
                var data = BUS_Chart.takeData(_connection, beginDate, endDate, IGuiChart.queryString);
                IGuiChart.setData(data);
                IGuiChart.setDateMethod(beginDate, endDate, method);
                var myControl = IGuiChart.display;
                StackPanelChart.Children.Add(myControl);
            }

            //Display top 10 products on sales day by day
            var top10Products = BUS_Chart.takeData(_connection, beginDate, endDate, _queryGetTop10Products);

            var cart = new BindingList<ORDEREDPHONE>();

            foreach (DataRow row in top10Products.Rows)
            {
                ORDEREDPHONE newPhone = new ORDEREDPHONE()
                {
                    Price = (double)row["PRICE"],
                    PhoneName = (string)row["PHONE_NAME"],
                    quantity = (int)row["SOLD"],
                    Thumbnail = (string)row["THUMBNAIL"]
                };

                cart.Add(newPhone);
            }
            PhoneListView.ItemsSource = cart;
        }

        private void ChartWithMonthButton_Click(object sender, RoutedEventArgs e)
        {
            int method = 2;

            if (ShowDateRange() == false) return;
            var beginDate = start;
            var endDate = end;

            StackPanelChart.Children.RemoveRange(0, IGuiForChart.Count);

            foreach (var IGuiChart in IGuiForChart)
            {
                var data = BUS_Chart.takeData(_connection, beginDate, endDate, IGuiChart.queryString);
                IGuiChart.setData(data);
                IGuiChart.setDateMethod(beginDate, endDate, method);
                var myControl = IGuiChart.display;
                StackPanelChart.Children.Add(myControl);
            }

            //Display top 10 products on sales month by month
            var top10Products = BUS_Chart.takeData(_connection, beginDate, endDate, _queryGetTop10Products);

            var cart = new BindingList<ORDEREDPHONE>();

            foreach (DataRow row in top10Products.Rows)
            {
                ORDEREDPHONE newPhone = new ORDEREDPHONE()
                {
                    Price = (double)row["PRICE"],
                    PhoneName = (string)row["PHONE_NAME"],
                    quantity = (int)row["SOLD"],
                    Thumbnail = (string)row["THUMBNAIL"]
                };

                cart.Add(newPhone);
            }
            PhoneListView.ItemsSource = cart;
        }

        private void ChartWithYearButton_Click(object sender, RoutedEventArgs e)
        {
            int method = 3;
            if (ShowDateRange() == false) return;
            var beginDate = start;
            var endDate = end;

            StackPanelChart.Children.RemoveRange(0, IGuiForChart.Count);

            foreach (var IGuiChart in IGuiForChart)
            {
                var data = BUS_Chart.takeData(_connection, beginDate, endDate, IGuiChart.queryString);
                IGuiChart.setData(data);
                IGuiChart.setDateMethod(beginDate, endDate, method);
                var myControl = IGuiChart.display;
                StackPanelChart.Children.Add(myControl);
            }

            //Display top 10 products on sales year by year
            var top10Products = BUS_Chart.takeData(_connection, beginDate, endDate, _queryGetTop10Products);

            var cart = new BindingList<ORDEREDPHONE>();

            foreach (DataRow row in top10Products.Rows)
            {
                ORDEREDPHONE newPhone = new ORDEREDPHONE()
                {
                    Price = (double)row["PRICE"],
                    PhoneName = (string)row["PHONE_NAME"],
                    quantity = (int)row["SOLD"],
                    Thumbnail = (string)row["THUMBNAIL"]
                };

                cart.Add(newPhone);
            }
            PhoneListView.ItemsSource = cart;
        }

    }
}
