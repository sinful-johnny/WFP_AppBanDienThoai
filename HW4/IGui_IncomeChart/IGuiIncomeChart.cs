
using IGuiChart;
using System.Data;
using System.Windows.Controls;

namespace IGui_IncomeChart
{
    public class IGuiIncomeChart : IGui
    {
        private DataTable _dataTable;
        private DateTime _beginDate;
        private DateTime _endDate;
        private int _method;
        public void setDateMethod(DateTime beginDate, DateTime endDate, int method)
        {
            _beginDate = beginDate;
            _endDate = endDate;
            _method = method;
        }
        public UserControl display => new UCIncomeChart(_dataTable, _beginDate, _endDate, _method);

        public string queryString => """
                                         select O.ORDER_ID, O.CREATED_DATE, O.TOTAL, 
                                               (O.TOTAL - P.ORIGINALPRICE*OP.PHONE_COUNT) AS 'PROFIT'
                                         from ORDERS O, ORDERS_PHONE OP, PHONE P
                                         where OP.PHONE_ID = P.ID
                                             and OP.ORDER_ID = O.ORDER_ID
                                             and CREATED_DATE BETWEEN @StartDate AND @EndDate
                                      """;

        public void setData(DataTable dataTable)
        {
            _dataTable = dataTable;
        }
    }

}
