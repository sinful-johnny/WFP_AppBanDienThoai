using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4.DAO
{
    internal class CUSTOMERControl
    {
        static public Tuple<BindingList<CUSTOMER>, int, int> GetAllPaging(SqlConnection connection, int page, int rowsPerPage)
        {
            int totalItems = -1;
            int totalPages = -1;
            //var phones = new BindingList<PHONE>();
            int skip = (page - 1) * rowsPerPage;
            int take = rowsPerPage;
            string sql = """
                             select C.CUS_ID,C.FIRSTNAME,C.LASTNAME,C.GENDER,C.PHONENUM,C.CUS_ADDRESS, C.DOB,C.CUS_IMAGE, count(*) over() as TotalItems 
                             from CUSTOMER as C 
                             order by C.CUS_ID
                             offset @Skip rows 
                             fetch next @Take rows only
                             """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            //DataTable dataTable = new DataTable();
            BindingList<CUSTOMER> list = new BindingList<CUSTOMER>();
            using (var command = new SqlCommand(sql, connection))
            {
                //_connection.Open();

                command.Parameters.Add("@Skip", SqlDbType.Int).Value = skip;
                command.Parameters.Add("@Take", SqlDbType.Int).Value = take;
                var reader = command.ExecuteReader();
                
                int TotalItems = 0;
                //dataTable.Load(reader);
                while(reader.Read())
                {
                    TotalItems = (int)reader[8];
                    list.Add(new CUSTOMER()
                    {
                        Cus_ID = ((int)reader[0]).ToString(),
                        FirstName = (string)reader[1],
                        LastName = (string)reader[2],
                        Gender = (string)reader[3],
                        PhoneNum = (string)reader[4],
                        Address = (string)reader[5],
                        DOB = (DateTime)reader[6],
                        Pfp = (string)reader[7]
                    });
                }

                if (totalItems == -1 && list.Count > 0)
                {
                    totalItems = TotalItems;
                    totalPages = (totalItems / rowsPerPage);
                    if (totalItems % rowsPerPage == 0) totalPages = (totalItems / rowsPerPage);
                    else totalPages = (int)(totalItems / rowsPerPage) + 1;
                }
                reader.Close();
            }
            var result = new Tuple<BindingList<CUSTOMER>, int, int>(list, totalItems, totalPages);
            return result;
        }
        static public bool Update(SqlConnection connection, CUSTOMER info)
        {
            string query = """
                Update CUSTOMER 
                set FIRSTNAME=@firstName,
                    LASTNAME=@lastName,
                    GENDER=@gender,
                    PHONENUM=@phoneNum, 
                    CUS_ADDRESS=@address,
                    DOB=@dob,
                    CUS_IMAGE=@pfp
                where CUS_ID=@ID
                """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = int.Parse(info.Cus_ID);
                cmd.Parameters.Add("@firstName", SqlDbType.VarChar).Value = info.FirstName;
                cmd.Parameters.Add("@lastName", SqlDbType.VarChar).Value = info.LastName;
                cmd.Parameters.Add("@gender", SqlDbType.VarChar).Value = info.Gender;
                cmd.Parameters.Add("@phoneNum", SqlDbType.VarChar).Value = info.PhoneNum;
                cmd.Parameters.Add("@address", SqlDbType.VarChar).Value = info.Address;
                cmd.Parameters.Add("@dob", SqlDbType.DateTime).Value = info.DOB;
                cmd.Parameters.Add("@pfp", SqlDbType.VarChar).Value = info.Pfp;

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

        static public int Insert(SqlConnection connection, CUSTOMER info)
        {
            string query = """
                INSERT INTO CUSTOMER
                VALUES (@firstName,
                    @lastName,
                    @gender,
                    @phoneNum, 
                    @address,
                    @dob,
                    @pfp)
                """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            int id = -1;
            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@firstName", SqlDbType.VarChar).Value = info.FirstName;
                cmd.Parameters.Add("@lastName", SqlDbType.VarChar).Value = info.LastName;
                cmd.Parameters.Add("@gender", SqlDbType.VarChar).Value = info.Gender;
                cmd.Parameters.Add("@phoneNum", SqlDbType.VarChar).Value = info.PhoneNum;
                cmd.Parameters.Add("@address", SqlDbType.VarChar).Value = info.Address;
                cmd.Parameters.Add("@dob", SqlDbType.DateTime).Value = info.DOB;
                cmd.Parameters.Add("@pfp", SqlDbType.VarChar).Value = info.Pfp;

                try
                {
                    id = (int)((decimal)cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    connection.Close();
                    //throw new Exception(ex.ToString());
                }
            }
            connection.Close();
            return id;
        }

        static public bool Delete(SqlConnection connection, int ID)
        {
            string query = """
                DELETE FROM CUSTOMER WHERE CUS_ID=@ID
                """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID;

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
