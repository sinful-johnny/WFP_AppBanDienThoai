using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4
{
    internal class PROMOTIONS_Control
    {
        static public Tuple<DataTable, int, int> GetAllPaging(SqlConnection connection, int page, int rowsPerPage)
        {
            int totalItems = -1;
            int totalPages = -1;
            //var phones = new BindingList<PHONE>();
            int skip = (page - 1) * 10;
            int take = rowsPerPage;
            string sql = """
                             select PR.PROMO_ID, PR.PROMO_NAME, PR.STARTDATE, PR.ENDDATE, M.NAME, PR.DISCOUNTS, PR.PROMO_STATUS, count(*) over() as TotalItems 
                             from MANUFACTURER as M, PROMOTIONS as PR 
                             where M.ID = PR.PROMO_MANUFACTURER_ID
                             order by PR.PROMO_ID
                             offset @Skip rows 
                             fetch next @Take rows only
                             """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            DataTable dataTable = new DataTable();
            using (var command = new SqlCommand(sql, connection))
            {
                //_connection.Open();

                command.Parameters.Add("@Skip", SqlDbType.Int).Value = skip;
                command.Parameters.Add("@Take", SqlDbType.Int).Value = take;
                var reader = command.ExecuteReader();
                
                //while (reader.Read())
                //{
                //    if (totalItems == -1)
                //    {
                //        totalItems = (int)reader["TotalItems"];
                //        totalPages = (totalItems / rowsPerPage);
                //        if (totalItems % rowsPerPage == 0) totalPages = (totalItems / rowsPerPage);
                //        else totalPages = (int)(totalItems / rowsPerPage) + 1;
                //    }
                //    phones.Add(new PHONE()
                //    {
                //        ID = (int)reader["ID"],
                //        PhoneName = (string)reader["NAME"],
                //        Manufacturer = (string)reader["MANUFACTURER"],
                //        Thumbnail = (string)reader["THUMBNAIL"],
                //        Price = (double)reader["PRICE"]
                //    });
                //}
                dataTable.Load(reader);
                if (totalItems == -1 && dataTable.Rows.Count > 0)
                {
                    totalItems = int.Parse(dataTable.Rows[0]["TotalItems"].ToString());
                    totalPages = (totalItems / rowsPerPage);
                    if (totalItems % rowsPerPage == 0) totalPages = (totalItems / rowsPerPage);
                    else totalPages = (int)(totalItems / rowsPerPage) + 1;
                }
                dataTable.Columns.Remove("TotalItems");
                reader.Close();
            }
            var result = new Tuple<DataTable, int, int>(dataTable, totalItems, totalPages);
            return result;
        }
    }
}
