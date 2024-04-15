using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data;

namespace IGui_OrderCustomer
{
    /// <summary>
    /// Interaction logic for UCOrderCustomerChart.xaml
    /// </summary>
    public partial class UCOrderCustomerChart : UserControl
    {
        private DataTable _dataTable;



        private int[] _numberOrder;
        private List<string> _customerName;
        public UCOrderCustomerChart(DataTable dataTable)
        {
            InitializeComponent();
            _dataTable = dataTable;
            declareIncomeChartSeries();
        }

        private Tuple<int[], List<string>> takenumberOrderCustomer()
        {
            var numberOrderCustomerchartlists = _dataTable;

            int numberOfCustomer = numberOrderCustomerchartlists.Rows.Count;

            int[] numberOrder = new int[numberOfCustomer];

            var customerName = new List<string>();
            int index = 0;
            foreach (DataRow row in numberOrderCustomerchartlists.Rows)
            {
                numberOrder[index] = (int)row["NUMBERORDER"];
                customerName.Add((string)row["FIRSTNAME"] + " " + (string)row["LASTNAME"]);
                index++;
            }

            var res = new Tuple<int[], List<string>>(numberOrder, customerName);

            return res;
        }

        private void declareIncomeChartSeries()
        {
            OrderCustomerchart.Series = new SeriesCollection()
            {
                new ColumnSeries()
                {
                    Title = "Số lượng đặt hàng",
                    Values = new ChartValues<int>(),
                    Stroke = Brushes.OrangeRed,
                    StrokeThickness = 2,
                    Fill = Brushes.OrangeRed
                }
            };

            OrderCustomerchart.AxisX.Add(new Axis()
            {
                Labels = new List<string>()
            });

            var res = takenumberOrderCustomer();
            _numberOrder = res.Item1;
            _customerName = res.Item2;

            foreach (int numberOrder in _numberOrder)
                OrderCustomerchart.Series[0].Values.Add(numberOrder);

            OrderCustomerchart.AxisX[0].Labels = _customerName;
        }
    }
}
