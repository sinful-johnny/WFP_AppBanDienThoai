using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4.BUS
{
    internal class BUS_Promotions
    {
        static public Tuple<DataTable, int, int> GetAllPaging(SqlConnection connection, int page, int rowsPerPage)
        {
            return PROMOTIONS_Control.GetAllPaging(connection, page, rowsPerPage);
        }
    }
}
