using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IBusChart;

namespace HW4
{
    /// <summary>
    /// Interaction logic for UCQuantitySoldChart.xaml
    /// </summary>
    public partial class UCQuantitySoldChart : UserControl
    {
        public UCQuantitySoldChart(IBus bus)
        {
            InitializeComponent();

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

            //var res = 
        }
    }
}
