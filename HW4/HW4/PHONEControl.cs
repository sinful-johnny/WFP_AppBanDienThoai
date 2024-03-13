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
    public class PHONEControl(SqlConnection connection)
    {
        SqlConnection _connection = connection;

        //public BindingList<PHONE> GetPHONEsByManufacturer(string manufacturer)
        //{

        //    using (var connection = new SqlConnection(_ConnectionString))
        //    {

        //        string sql = $"select PHONE.ID,PHONE.NAME,M.NAME as MANUFACTURER,PHONE.THUMBNAIL,PHONE.PRICE from PHONE, MANUFACTURER as M where PHONE.MANUFACTURER_ID = M.ID and M.NAME=@Manufacturer";
        //        connection.Open();
        //        var command = new SqlCommand(sql, connection);
        //        command.Parameters.Add("@Manufacturer", SqlDbType.Text).Value = manufacturer;
        //        var reader = command.ExecuteReader();

        //        var phones = new BindingList<PHONE>();
        //        while (reader.Read())
        //        {
        //            phones.Add(new PHONE()
        //            {
        //                ID = (int)reader["ID"],
        //                PhoneName = (string)reader["NAME"],
        //                Manufacturer = (string)reader["MANUFACTURER"],
        //                Thumbnail = (string)reader["THUMBNAIL"],
        //                Price = (int)reader["PRICE"]
        //            });
        //        }
        //        return phones;
        //    }     
        //}

        //public BindingList<PHONE> GetPHONEs()
        //{
        //    using (var connection = new SqlConnection(_ConnectionString))
        //    {
        //        string sql = $"select PHONE.ID,PHONE.NAME,M.NAME as MANUFACTURER,PHONE.THUMBNAIL,PHONE.PRICE from PHONE, MANUFACTURER as M where PHONE.MANUFACTURER_ID = M.ID";
        //        connection.Open();
        //        var command = new SqlCommand(sql, connection);
        //        var reader = command.ExecuteReader();

        //        var phones = new BindingList<PHONE>();
        //        while (reader.Read())
        //        {
        //            phones.Add(new PHONE()
        //            {
        //                ID = (int)reader["ID"],
        //                PhoneName = (string)reader["NAME"],
        //                Manufacturer = (string)reader["MANUFACTURER"],
        //                Thumbnail = (string)reader["THUMBNAIL"],
        //                Price = (double)reader["PRICE"]
        //            });
        //        }
        //        return phones;
        //    }
        //}

        public Tuple<BindingList<PHONE>, int, int> GetAllPaging(int page, int rowsPerPage, string keyword, string Manufacturer)
        {
            int totalItems = -1;
            int totalPages = -1;
            var phones = new BindingList<PHONE>();
            int skip = (page - 1 ) * 10;
            int take = rowsPerPage;
            string sql = """
                             select PHONE.ID,PHONE.NAME,M.NAME as MANUFACTURER,PHONE.THUMBNAIL,PHONE.PRICE, count(*) over() as TotalItems 
                             from PHONE, MANUFACTURER as M 
                             where PHONE.MANUFACTURER_ID = M.ID
                             order by PHONE.ID 
                             offset @Skip rows 
                             fetch next @Take rows only
                             """;
            if ((keyword != null && keyword != " " && keyword != "") && (Manufacturer == null || Manufacturer == " " || Manufacturer == ""))
            {
                sql = """
                                select PHONE.ID,PHONE.NAME,M.NAME as MANUFACTURER,PHONE.THUMBNAIL,PHONE.PRICE, count(*) over() as TotalItems 
                                from PHONE, MANUFACTURER as M 
                                where PHONE.MANUFACTURER_ID = M.ID 
                                and contains(PHONE.NAME, @Keyword)
                                order by PHONE.ID 
                                offset @Skip rows 
                                fetch next @Take rows only
                                """;
                
            }
            else if((keyword != null && keyword != " " && keyword != "") && (Manufacturer != null && Manufacturer != " " && Manufacturer != ""))
            {
                sql = """
                                select PHONE.ID,PHONE.NAME,M.NAME as MANUFACTURER,PHONE.THUMBNAIL,PHONE.PRICE, count(*) over() as TotalItems 
                                from PHONE, MANUFACTURER as M 
                                where   PHONE.MANUFACTURER_ID = M.ID
                                        and M.NAME = @Manufacturer 
                                        and contains(PHONE.NAME, @Keyword)
                                order by PHONE.ID 
                                offset @Skip rows 
                                fetch next @Take rows only
                                """;
            }
            else if((keyword == null || keyword == " " || keyword == "") && (Manufacturer != null && Manufacturer != " " && Manufacturer != ""))
            {
                sql = """
                                select PHONE.ID,PHONE.NAME,M.NAME as MANUFACTURER,PHONE.THUMBNAIL,PHONE.PRICE, count(*) over() as TotalItems 
                                from PHONE, MANUFACTURER as M 
                                where   PHONE.MANUFACTURER_ID = M.ID
                                        and M.NAME = @Manufacturer
                                order by PHONE.ID 
                                offset @Skip rows 
                                fetch next @Take rows only
                                """;
            }

            using (var command = new SqlCommand(sql, _connection))
            {
                //_connection.Open();

                command.Parameters.Add("@Skip", SqlDbType.Int).Value = skip;
                command.Parameters.Add("@Take", SqlDbType.Int).Value = take;
                if(keyword != null || keyword != " " || keyword != "")
                {
                    command.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
                }
                if (Manufacturer != null || Manufacturer != " " || Manufacturer != "")
                {
                    command.Parameters.Add("@Manufacturer", SqlDbType.VarChar).Value = Manufacturer;
                }
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if(totalItems == -1)
                    {
                        totalItems = (int)reader["TotalItems"];
                        totalPages = (totalItems/rowsPerPage);
                        if (totalItems % rowsPerPage == 0) totalPages = (totalItems / rowsPerPage);
                        else totalPages = (int)(totalItems / rowsPerPage) + 1;
                    }
                    phones.Add(new PHONE()
                    {
                        ID = (int)reader["ID"],
                        PhoneName = (string)reader["NAME"],
                        Manufacturer = (string)reader["MANUFACTURER"],
                        Thumbnail = (string)reader["THUMBNAIL"],
                        Price = (double)reader["PRICE"]
                    });
                }
                reader.Close();
            }
            var result = new Tuple<BindingList<PHONE>, int, int>(phones, totalItems, totalPages);
            return result;
        }

        public int InsertPHONE(string name, int ManufacturerID, string Thumbnail, double Price)
        {
            string query = """
                Insert into PHONE(NAME,MANUFACTURER_ID,THUMBNAIL,PRICE)
                values(@NAME,@MANUFACTURER_ID,@THUMBNAIL,@PRICE)
                """;
            int id;

            using (var cmd = new SqlCommand(query, _connection))
            {
                cmd.Parameters.Add("@NAME",SqlDbType.VarChar).Value = name;
                cmd.Parameters.Add("@MANUFACTURER_ID", SqlDbType.Int).Value = ManufacturerID;
                cmd.Parameters.Add("@THUMBNAIL", SqlDbType.VarChar).Value = Thumbnail;
                cmd.Parameters.Add("@PRICE", SqlDbType.Float).Value = Price;
                
                try
                {
                    id = (int)((decimal)cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return -1;
                }
            }
            return id;
        }

        public bool DeletePHONE(int ID)
        {
            string query = """
                Delete from PHONE where ID=@ID
                """;

            using (var cmd = new SqlCommand(query, _connection))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            return true;
        }

        //public Tuple<BindingList<PHONE>, int, int> GetByManufacturer(int page, int rowsPerPage, string keyword)
        //{
        //    int totalItems = -1;
        //    int totalPages = -1;
        //    var phones = new BindingList<PHONE>();
        //    int skip = (page - 1) * 10;
        //    int take = rowsPerPage;
        //    string sql = """
        //                        select PHONE.ID,PHONE.NAME,M.NAME as MANUFACTURER,PHONE.THUMBNAIL,PHONE.PRICE, count(*) over() as TotalItems 
        //                        from PHONE, MANUFACTURER as M 
        //                        where   PHONE.MANUFACTURER_ID = M.ID
        //                                and M.NAME = @Manufacturer 
        //                                and contains(PHONE.NAME, @Keyword)
        //                        order by PHONE.ID 
        //                        offset @Skip rows 
        //                        fetch next @Take rows only
        //                        """;
        //    if (keyword == null || keyword == " " || keyword == "")
        //    {
        //        sql = """
        //                        select PHONE.ID,PHONE.NAME,M.NAME as MANUFACTURER,PHONE.THUMBNAIL,PHONE.PRICE, count(*) over() as TotalItems 
        //                        from PHONE, MANUFACTURER as M 
        //                        where   PHONE.MANUFACTURER_ID = M.ID
        //                                and M.NAME = @Manufacturer
        //                        order by PHONE.ID 
        //                        offset @Skip rows 
        //                        fetch next @Take rows only
        //                        """;
        //    }

        //    using (var command = new SqlCommand(sql, _connection))
        //    {
        //        //_connection.Open();

        //        command.Parameters.Add("@Skip", SqlDbType.Int).Value = skip;
        //        command.Parameters.Add("@Take", SqlDbType.Int).Value = take;
        //        command.Parameters.Add("@Manufacturer", SqlDbType.VarChar).Value = Manufacturer;
        //        if (keyword != null || keyword != " " || keyword != "")
        //        {
        //            command.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
        //        }
        //        var reader = command.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            if (totalItems == -1)
        //            {
        //                totalItems = (int)reader["TotalItems"];
        //                totalPages = (totalItems / rowsPerPage);
        //                if (totalItems % rowsPerPage == 0) totalPages = (totalItems / rowsPerPage);
        //                else totalPages = (int)(totalItems / rowsPerPage) + 1;
        //            }
        //            phones.Add(new PHONE()
        //            {
        //                ID = (int)reader["ID"],
        //                PhoneName = (string)reader["NAME"],
        //                Manufacturer = (string)reader["MANUFACTURER"],
        //                Thumbnail = (string)reader["THUMBNAIL"],
        //                Price = (double)reader["PRICE"]
        //            });
        //        }
        //        reader.Close();
        //    }
        //    var result = new Tuple<BindingList<PHONE>, int, int>(phones, totalItems, totalPages);
        //    return result;
        //}
    }
}
