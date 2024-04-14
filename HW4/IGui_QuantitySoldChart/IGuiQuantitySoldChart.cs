using IGuiChart;
using HW4;
using System.Data;
using System.Windows.Controls;

namespace IGui_QuantitySoldChart
{
    public class IGuiQuantitySoldChart : IGui
    {
        public string stringQuery { get; } //query
        public void SetData(DataTable data)
        {

        }
        public UserControl display => new UCQuantitySoldChart();
    }

}
