using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data;

namespace IGui_QuantitySoldChart
{
    /// <summary>
    /// Interaction logic for UCQuantitySoldChart.xaml
    /// </summary>
    public partial class UCQuantitySoldChart : UserControl
    {
        private DataTable _dataTable;



        private int[] _quantitySold;
        private List<string> _phoneName;

        public UCQuantitySoldChart(DataTable dataTable)
        {
            InitializeComponent();
            _dataTable = dataTable;
            declareQuantitySoldChartSeries();
        }

        private Tuple<int[], List<string>> takequantitysoldphonename()
        {
            var quantitysoldchartlists = _dataTable;

            int numberOfPhone = quantitysoldchartlists.Rows.Count;

            int[] quantitysold = new int[numberOfPhone];

            var phoneName = new List<string>();
            int index = 0;
            foreach (DataRow row in quantitysoldchartlists.Rows)
            {
                quantitysold[index] = (int)row["QUANTITY_SOLD"];
                phoneName.Add((string)row["NAME"]);
                index++;
            }

            var res = new Tuple<int[], List<string>>(quantitysold, phoneName);

            return res;
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

            var res = takequantitysoldphonename();
            _quantitySold = res.Item1;
            _phoneName = res.Item2;

            foreach (int quantitysold in _quantitySold)
                QuantitySoldchart.Series[0].Values.Add(quantitysold);

            QuantitySoldchart.AxisX[0].Labels = _phoneName;
        }
    }
}
