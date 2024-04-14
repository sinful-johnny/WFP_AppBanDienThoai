using IBusChart;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Controls;
using System.Windows.Media;

namespace HW4
{
    /// <summary>
    /// Interaction logic for UCIncomeChart.xaml
    /// </summary>
    public partial class UCIncomeChart : UserControl
    {
        public UCIncomeChart(IBus bus)
        {
            InitializeComponent();

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
    }
}
