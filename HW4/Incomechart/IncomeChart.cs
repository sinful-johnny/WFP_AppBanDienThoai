using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using ChartPlugin;
using System.Windows.Shapes;

namespace Incomechart
{
    public class IncomeChart:ReportChart
    {
        public Tuple<SeriesCollection, Axis> declareChartSeries()
        {
            var Chart = new SeriesCollection()
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

            var AxisLable = new Axis()
            {
                Labels = new List<string>()
            };

            var res = new Tuple<SeriesCollection, Axis>(Chart, AxisLable);
            return res;
        }
        public object Clone()
        {
            return new IncomeChart();
        }

        public string Name => "Income and Profit chart";
    }

}
