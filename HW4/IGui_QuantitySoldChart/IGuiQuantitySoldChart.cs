using IGuiChart;
using System.Data;
using System.Windows.Controls;

namespace IGui_QuantitySoldChart
{
    public class IGuiQuantitySoldChart : IGui
    {
        private DataTable _dataTable;
        public UserControl display => new UCQuantitySoldChart(_dataTable);

        public string queryString => """
                                        select distinct P.NAME, SUM(OP.PHONE_COUNT) AS 'QUANTITY_SOLD'
                                        from ORDERS O, ORDERS_PHONE OP, PHONE P
                                        where P.ID = OP.PHONE_ID
                                            and OP.ORDER_ID = O.ORDER_ID
                                            and O.CREATED_DATE BETWEEN @StartDate AND @EndDate
                                        group by P.NAME
                                        order by 'QUANTITY_SOLD'
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
