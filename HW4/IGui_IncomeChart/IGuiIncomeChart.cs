using IGuiChart;
using HW4;
using System.Data;
using System.Windows.Controls;

namespace IGui_IncomeChart
{
    public class IGuiIncomeChart : IGui
    {
        public string stringQuery { get; } //query
        public void SetData(DataTable data)
        {

        }
        public UserControl display => new UCIncomeChart();
    }

}
