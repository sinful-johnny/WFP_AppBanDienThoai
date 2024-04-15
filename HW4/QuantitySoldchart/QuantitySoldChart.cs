using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using ChartPlugin;
using System.Windows.Shapes;

namespace QuantitySoldchart
{
    public class QuantitySoldChart:ReportChart
    {
        public Tuple<SeriesCollection, Axis> declareChartSeries()
        {
            var Chart = new SeriesCollection()
            {
                new ColumnSeries()
                {
                    Title = "S? l??ng các s?n ph?m ???c bán",
                    Values = new ChartValues<int>(),
                    Stroke = Brushes.OrangeRed,
                    StrokeThickness = 2,
                    Fill = Brushes.OrangeRed
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
            return new QuantitySoldChart();
        }

        public string Name => "Quantity sold chart";
    }

}
