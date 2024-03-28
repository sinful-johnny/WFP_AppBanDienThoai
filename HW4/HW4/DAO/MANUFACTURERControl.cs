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
        SqlConnection _connection = connection;

        public BindingList<MANUFACTURER> GetMANUFACTURERs()
        {
            string query = """
                select *
                from MANUFACTURER
                """;

            BindingList<MANUFACTURER> mANUFACTURERs = new BindingList<MANUFACTURER>();

            using (SqlCommand cmd = new SqlCommand(query, _connection))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int ID = (int)reader["ID"];
                    string NAME = (string)reader["NAME"];

                    mANUFACTURERs.Add(new MANUFACTURER() { ID = ID, Name = NAME });
                }
            }
            return mANUFACTURERs;
        }
    }
}
