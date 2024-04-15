using DocumentFormat.OpenXml.Wordprocessing;
using HW4.DAO;
using HW4.DTO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace HW4.BUS
{
    class BUS_Chart
    {
        static public DateTime NewestOrderDate(SqlConnection connection, string querystring)
        {
            return CHARTCONTROL.TakeMaxOrderDate(connection, querystring);
        }

        static public int AmountProducts(SqlConnection connection, DateTime beginDate, DateTime endDate, string querystring)
        {
            return CHARTCONTROL.AmountProducts(connection, beginDate, endDate, querystring);
        }

        static public int AmountOnSales(SqlConnection connection, string querystring)
        {
            return CHARTCONTROL.AmountOnSales(connection, querystring);
        }

        static public DataTable takeData(SqlConnection connection,
                                                           DateTime beginDate, DateTime endDate, string querystring)
        {
            //List<double> profits;
            return CHARTCONTROL.DataHandling(connection, beginDate, endDate, querystring);
        }
    }
}
