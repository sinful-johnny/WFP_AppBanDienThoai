using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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

        static public BindingList<MANUFACTURER> getByKeyWord(SqlConnection connection,string keyWord) {
            string query = """
                select *
                from MANUFACTURER
                where contains(NAME, @Keyword)
                """;

            BindingList<MANUFACTURER> mANUFACTURERs = new BindingList<MANUFACTURER>();
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@Keyword", SqlDbType.VarChar).Value = keyWord;
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

        static public bool delete(SqlConnection connection, string ManufacturerID) {
            string query = """
                Delete from MANUFACTURER where ID=@ID
                """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ManufacturerID;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    connection.Close();
                    return false;
                }
            }
            connection.Close();
            return true;
        }

        static public int insert(SqlConnection connection, string ManufacturerName)
        {
            string query = """
                Insert into MANUFACTURER
                values(@NAME)
                """;
            int id;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@NAME", SqlDbType.VarChar).Value = ManufacturerName;
                try
                {
                    id = (int)(decimal)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    connection.Close();
                    return -1;
                }
            }
            connection.Close();
            return id;
        }

        static public bool update(SqlConnection connection,string ManufacturerID, string ManufacturerName)
        {
            string query = """
                UPDATE MANUFACTURER set NAME=@NAME where ID=@ID
                """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ManufacturerID;
                cmd.Parameters.Add("@NAME", SqlDbType.VarChar).Value = ManufacturerName;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    connection.Close();
                    return false;
                }
            }
            connection.Close();
            return true;
        }
    }
}
