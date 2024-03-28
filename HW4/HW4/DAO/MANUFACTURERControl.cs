using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4
{
    internal class MANUFACTURERControl(SqlConnection connection)
    {
        static public BindingList<MANUFACTURER> GetMANUFACTURERs(SqlConnection connection)
        {
            string query = """
                select *
                from MANUFACTURER
                """;

            BindingList<MANUFACTURER> mANUFACTURERs = new BindingList<MANUFACTURER>();
            if(connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int ID = (int)reader["ID"];
                    string NAME = (string)reader["NAME"];

                    mANUFACTURERs.Add(new MANUFACTURER() { ID = ID, Name = NAME });
                }
            }
            connection.Close();
            return mANUFACTURERs;
        }
    }
}
