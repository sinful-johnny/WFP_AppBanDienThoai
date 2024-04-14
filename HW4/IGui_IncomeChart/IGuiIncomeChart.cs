using IGuiChart;
using IBusChart;
using System.Data.SqlClient;
using System.Windows.Controls;
using HW4;

namespace IGui_IncomeChart
{
    public class IGuiIncomeChart : IGui
    {
        private SqlConnection _connection;
        private IBus _IBus;
        public string stringQuery { get; set; }
        public IGuiIncomeChart(SqlConnection connection)
        {
            _connection = connection;
        }
        public void setBus(IBus bus)
        {
            _IBus = bus;
            _IBus.stringQuery = stringQuery;
        }
        public IBus GetBus()
        {
            return _IBus;
        }
        public UserControl display => new UCIncomeChart(_IBus);
    }
}
