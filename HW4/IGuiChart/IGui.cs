
using System.Data;
using System.Windows.Controls;

namespace IGuiChart
{
    public interface IGui
    {
        public string queryString { get; }
        public void setData(DataTable dataTable);
        public void setDateMethod(DateTime beginDate, DateTime endDate, int method);
        UserControl display { get; }
    }

}
