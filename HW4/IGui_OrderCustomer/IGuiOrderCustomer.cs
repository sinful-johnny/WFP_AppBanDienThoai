using IGuiChart;
using System.Data;
using System.Windows.Controls;

namespace IGui_OrderCustomer
{
    public class IGuiOrderCustomer : IGui
    {
        private DataTable _dataTable;
        public UserControl display => new UCOrderCustomerChart(_dataTable);

        public string queryString => """
                                        select distinct C.FIRSTNAME, C.LASTNAME, count(O.CUSTOMER_ID) AS 'NUMBERORDER'
                                        from ORDERS O, CUSTOMER C
                                        where O.CUSTOMER_ID = C.CUS_ID
                                            and O.CREATED_DATE BETWEEN @StartDate AND @EndDate
                                        group by C.FIRSTNAME, C.LASTNAME, C.CUS_ID
                                     """;

        public void setData(DataTable dataTable)
        {
            _dataTable = dataTable;
        }
        public void setDateMethod(DateTime beginDate, DateTime endDate, int method)
        {

        }
    }

}
