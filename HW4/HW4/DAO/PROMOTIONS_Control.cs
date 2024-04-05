using DocumentFormat.OpenXml.Wordprocessing;
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
                             select PR.PROMO_ID, PR.PROMO_NAME, PR.STARTDATE, PR.ENDDATE, M.NAME as APPLIED_MANUFACTURER, P.NAME as APPLIED_PHONE, PR.DISCOUNTS, PR.PROMO_STATUS, count(*) over() as TotalItems 
                             from PROMOTIONS as PR 
                             		left join PHONE as P on P.ID = PR.PROMO_PHONE_ID
                             		left join MANUFACTURER as M on M.ID = PR.PROMO_MANUFACTURER_ID
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

        static public bool Update(SqlConnection connection, int ID, string PromoName, DateTime StartDate, DateTime EndDate, int ManufacturerID, int PhoneID, double Discount, string Status) {
            string query = """
                Update PROMOTIONS 
                set PROMO_NAME=@Name,
                    STARTDATE=@StartDate,
                    ENDDATE=@EndDate,
                    PROMO_MANUFACTURER_ID=@ManufacturerID, 
                    PROMO_PHONE_ID=@PhoneID,
                    DISCOUNTS=@Discount,
                    PROMO_STATUS=@Status
                where PROMO_ID=@ID
                """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = PromoName;
                cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                cmd.Parameters.Add("@ManufacturerID", SqlDbType.Int).Value = ManufacturerID;
                if(PhoneID == -1)
                {
                    cmd.Parameters.Add("@PhoneID", SqlDbType.Int).Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("@PhoneID", SqlDbType.Int).Value = PhoneID;
                }
                cmd.Parameters.Add("@Discount", SqlDbType.Float).Value = Discount;
                cmd.Parameters.Add("@Status", SqlDbType.VarChar).Value = Status;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    connection.Close();
                    throw new Exception(ex.ToString());
                }
            }
            connection.Close();
            return true;
        }
    }
}
