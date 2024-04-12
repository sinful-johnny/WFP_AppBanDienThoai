using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;

namespace ChartPlugin
{
    public interface ReportChart:ICloneable
    {
        Tuple<SeriesCollection, Axis> declareChartSeries();
        string Name { get; }
    }

}
