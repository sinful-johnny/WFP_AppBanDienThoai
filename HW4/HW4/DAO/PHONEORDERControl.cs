using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace HW4
{
    public class PHONEORDERControl(SqlConnection connect)
    {
        //static public Tuple<BindingList<PHONE>, int, int> GetAllPaging(SqlConnection connection, int page, int rowsPerPage, string keyword, string Manufacturer)
        //{
        //    int totalItems = -1;
        //    int totalPages = -1;
        //    var orders = new BindingList<PHONEORDER>();
        //    int skip = (page - 1) * 10;
        //    int take = rowsPerPage;
        //    string sql = """select ORDERS.ORDER_ID, CUSTOMER.FIRSTNAME, CUSTOMER.LASTNAME,  in """;
        //}
    }
}
