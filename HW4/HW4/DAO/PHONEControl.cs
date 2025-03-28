﻿using System;
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
        static public Tuple<BindingList<PHONE>, int, int> GetAllPagingWithKeywordAndManufacturer(SqlConnection connection,int page, int rowsPerPage, string keyword, string Manufacturer)
        {
            int totalItems = -1;
            int totalPages = -1;
            var phones = new BindingList<PHONE>();
            int skip = (page - 1 ) * rowsPerPage;
            int take = rowsPerPage;
            string sql = """
                                select PHONE.ID,PHONE.NAME,M.NAME as MANUFACTURER,PHONE.THUMBNAIL,PHONE.PRICE,PHONE.ORIGINALPRICE, count(*) over() as TotalItems 
                                from PHONE, MANUFACTURER as M 
                                where   PHONE.MANUFACTURER_ID = M.ID
                                        and M.NAME = @Manufacturer 
                                        and contains(PHONE.NAME, @Keyword)
                                order by PHONE.ID 
                                offset @Skip rows 
                                fetch next @Take rows only
                                """;
            if(connection.State == ConnectionState.Closed) {
                connection.Open();
            }
            
            using (var command = new SqlCommand(sql, connection))
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
                        Price = (double)reader["PRICE"],
                        OriginalPrice = (double)reader["ORIGINALPRICE"]
                    });
                }
                reader.Close();
            }
            var result = new Tuple<BindingList<PHONE>, int, int>(phones, totalItems, totalPages);
            return result;
        }

        static public Tuple<BindingList<PHONE>, int, int> GetAllPagingWithKeyword(SqlConnection connection, int page, int rowsPerPage, string keyword)
        {
            int totalItems = -1;
            int totalPages = -1;
            var phones = new BindingList<PHONE>();
            int skip = (page - 1) * rowsPerPage;
            int take = rowsPerPage;
            string    sql = """
                                select PHONE.ID,PHONE.NAME,M.NAME as MANUFACTURER,PHONE.THUMBNAIL,PHONE.PRICE, PHONE.ORIGINALPRICE, count(*) over() as TotalItems 
                                from PHONE, MANUFACTURER as M 
                                where PHONE.MANUFACTURER_ID = M.ID 
                                and contains(PHONE.NAME, @Keyword)
                                order by PHONE.ID 
                                offset @Skip rows 
                                fetch next @Take rows only
                                """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            using (var command = new SqlCommand(sql, connection))
            {
                //_connection.Open();

                command.Parameters.Add("@Skip", SqlDbType.Int).Value = skip;
                command.Parameters.Add("@Take", SqlDbType.Int).Value = take;
                command.Parameters.Add("@Keyword", SqlDbType.NVarChar).Value = keyword;
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (totalItems == -1)
                    {
                        totalItems = (int)reader["TotalItems"];
                        totalPages = (totalItems / rowsPerPage);
                        if (totalItems % rowsPerPage == 0) totalPages = (totalItems / rowsPerPage);
                        else totalPages = (int)(totalItems / rowsPerPage) + 1;
                    }
                    phones.Add(new PHONE()
                    {
                        ID = (int)reader["ID"],
                        PhoneName = (string)reader["NAME"],
                        Manufacturer = (string)reader["MANUFACTURER"],
                        Thumbnail = (string)reader["THUMBNAIL"],
                        Price = (double)reader["PRICE"],
                        OriginalPrice = (double)reader["ORIGINALPRICE"]
                    });
                }
                reader.Close();
            }
            var result = new Tuple<BindingList<PHONE>, int, int>(phones, totalItems, totalPages);
            return result;
        }

        static public Tuple<BindingList<PHONE>, int, int> GetAllPagingWithManufacturer(SqlConnection connection, int page, int rowsPerPage, string Manufacturer)
        {
            int totalItems = -1;
            int totalPages = -1;
            var phones = new BindingList<PHONE>();
            int skip = (page - 1) * rowsPerPage;
            int take = rowsPerPage;
            string sql = """
                                select PHONE.ID,PHONE.NAME,M.NAME as MANUFACTURER,PHONE.THUMBNAIL,PHONE.PRICE, PHONE.ORIGINALPRICE, count(*) over() as TotalItems 
                                from PHONE, MANUFACTURER as M 
                                where   PHONE.MANUFACTURER_ID = M.ID
                                        and M.NAME = @Manufacturer
                                order by PHONE.ID 
                                offset @Skip rows 
                                fetch next @Take rows only
                                """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            using (var command = new SqlCommand(sql, connection))
            {
                //_connection.Open();

                command.Parameters.Add("@Skip", SqlDbType.Int).Value = skip;
                command.Parameters.Add("@Take", SqlDbType.Int).Value = take;
                command.Parameters.Add("@Manufacturer", SqlDbType.VarChar).Value = Manufacturer;
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (totalItems == -1)
                    {
                        totalItems = (int)reader["TotalItems"];
                        totalPages = (totalItems / rowsPerPage);
                        if (totalItems % rowsPerPage == 0) totalPages = (totalItems / rowsPerPage);
                        else totalPages = (int)(totalItems / rowsPerPage) + 1;
                    }
                    phones.Add(new PHONE()
                    {
                        ID = (int)reader["ID"],
                        PhoneName = (string)reader["NAME"],
                        Manufacturer = (string)reader["MANUFACTURER"],
                        Thumbnail = (string)reader["THUMBNAIL"],
                        Price = (double)reader["PRICE"],
                        OriginalPrice = (double)reader["ORIGINALPRICE"]
                    });
                }
                reader.Close();
            }
            var result = new Tuple<BindingList<PHONE>, int, int>(phones, totalItems, totalPages);
            return result;
        }

        static public Tuple<BindingList<PHONE>, int, int> GetAllPaging(SqlConnection connection, int page, int rowsPerPage)
        {
            int totalItems = -1;
            int totalPages = -1;
            var phones = new BindingList<PHONE>();
            int skip = (page - 1) * rowsPerPage;
            int take = rowsPerPage;
            string sql = """
                             select PHONE.ID,PHONE.NAME,M.NAME as MANUFACTURER,PHONE.THUMBNAIL,PHONE.PRICE,PHONE.ORIGINALPRICE, count(*) over() as TotalItems 
                             from PHONE, MANUFACTURER as M 
                             where PHONE.MANUFACTURER_ID = M.ID
                             order by PHONE.ID 
                             offset @Skip rows 
                             fetch next @Take rows only
                             """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            using (var command = new SqlCommand(sql, connection))
            {
                //_connection.Open();

                command.Parameters.Add("@Skip", SqlDbType.Int).Value = skip;
                command.Parameters.Add("@Take", SqlDbType.Int).Value = take;
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (totalItems == -1)
                    {
                        totalItems = (int)reader["TotalItems"];
                        totalPages = (totalItems / rowsPerPage);
                        if (totalItems % rowsPerPage == 0) totalPages = (totalItems / rowsPerPage);
                        else totalPages = (int)(totalItems / rowsPerPage) + 1;
                    }
                    phones.Add(new PHONE()
                    {
                        ID = (int)reader["ID"],
                        PhoneName = (string)reader["NAME"],
                        Manufacturer = (string)reader["MANUFACTURER"],
                        Thumbnail = (string)reader["THUMBNAIL"],
                        Price = (double)reader["PRICE"],
                        OriginalPrice = (double)reader["ORIGINALPRICE"]
                    });
                }
                reader.Close();
            }
            var result = new Tuple<BindingList<PHONE>, int, int>(phones, totalItems, totalPages);
            return result;
        }

        static public Tuple<BindingList<PHONE>, int, int> GetAllPagingWithMinMaxPrice(SqlConnection connection, int page, int rowsPerPage,int Min, int Max)
        {
            int totalItems = -1;
            int totalPages = -1;
            var phones = new BindingList<PHONE>();
            int skip = (page - 1) * rowsPerPage;
            int take = rowsPerPage;
            string sql = """
                             select PHONE.ID,PHONE.NAME,M.NAME as MANUFACTURER,PHONE.THUMBNAIL,PHONE.PRICE, PHONE.ORIGINALPRICE, count(*) over() as TotalItems 
                             from PHONE, MANUFACTURER as M 
                             where PHONE.MANUFACTURER_ID = M.ID
                                    and PHONE.PRICE >= @Min
                                    and PHONE.PRICE <= @Max
                             order by PHONE.ID 
                             offset @Skip rows 
                             fetch next @Take rows only
                             """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            using (var command = new SqlCommand(sql, connection))
            {
                //_connection.Open();

                command.Parameters.Add("@Skip", SqlDbType.Int).Value = skip;
                command.Parameters.Add("@Take", SqlDbType.Int).Value = take;
                command.Parameters.Add("@Min", SqlDbType.Int).Value = Min;
                command.Parameters.Add("@Max", SqlDbType.Int).Value = Max;
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (totalItems == -1)
                    {
                        totalItems = (int)reader["TotalItems"];
                        totalPages = (totalItems / rowsPerPage);
                        if (totalItems % rowsPerPage == 0) totalPages = (totalItems / rowsPerPage);
                        else totalPages = (int)(totalItems / rowsPerPage) + 1;
                    }
                    phones.Add(new PHONE()
                    {
                        ID = (int)reader["ID"],
                        PhoneName = (string)reader["NAME"],
                        Manufacturer = (string)reader["MANUFACTURER"],
                        Thumbnail = (string)reader["THUMBNAIL"],
                        Price = (double)reader["PRICE"],
                        OriginalPrice = (double)reader["ORIGINALPRICE"]
                    });
                }
                reader.Close();
            }
            var result = new Tuple<BindingList<PHONE>, int, int>(phones, totalItems, totalPages);
            return result;
        }

        static public Tuple<BindingList<PHONE>, int, int> GetAllPagingWithMinPrice(SqlConnection connection, int page, int rowsPerPage, int Min)
        {
            int totalItems = -1;
            int totalPages = -1;
            var phones = new BindingList<PHONE>();
            int skip = (page - 1) * rowsPerPage;
            int take = rowsPerPage;
            string sql = """
                             select PHONE.ID,PHONE.NAME,M.NAME as MANUFACTURER,PHONE.THUMBNAIL,PHONE.PRICE, PHONE.ORIGINALPRICE, count(*) over() as TotalItems 
                             from PHONE, MANUFACTURER as M 
                             where PHONE.MANUFACTURER_ID = M.ID
                                    and PHONE.PRICE >= @Min
                             order by PHONE.ID 
                             offset @Skip rows 
                             fetch next @Take rows only
                             """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            using (var command = new SqlCommand(sql, connection))
            {
                //_connection.Open();

                command.Parameters.Add("@Skip", SqlDbType.Int).Value = skip;
                command.Parameters.Add("@Take", SqlDbType.Int).Value = take;
                command.Parameters.Add("@Min", SqlDbType.Int).Value = Min;
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (totalItems == -1)
                    {
                        totalItems = (int)reader["TotalItems"];
                        totalPages = (totalItems / rowsPerPage);
                        if (totalItems % rowsPerPage == 0) totalPages = (totalItems / rowsPerPage);
                        else totalPages = (int)(totalItems / rowsPerPage) + 1;
                    }
                    phones.Add(new PHONE()
                    {
                        ID = (int)reader["ID"],
                        PhoneName = (string)reader["NAME"],
                        Manufacturer = (string)reader["MANUFACTURER"],
                        Thumbnail = (string)reader["THUMBNAIL"],
                        Price = (double)reader["PRICE"],
                        OriginalPrice = (double)reader["ORIGINALPRICE"]
                    });
                }
                reader.Close();
            }
            var result = new Tuple<BindingList<PHONE>, int, int>(phones, totalItems, totalPages);
            return result;
        }

        static public int InsertPHONE(SqlConnection connection,string name, int ManufacturerID, string Thumbnail, double Price, double OriginalPrice)
        {
            string query = """
                Insert into PHONE(NAME,MANUFACTURER_ID,THUMBNAIL,PRICE, ORIGINALPRICE)
                values(@NAME,@MANUFACTURER_ID,@THUMBNAIL,@PRICE, @ORIGINALPRICE); SELECT SCOPE_IDENTITY()
                """;
            int id;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@NAME",SqlDbType.VarChar).Value = name;
                cmd.Parameters.Add("@MANUFACTURER_ID", SqlDbType.Int).Value = ManufacturerID;
                cmd.Parameters.Add("@THUMBNAIL", SqlDbType.VarChar).Value = Thumbnail;
                cmd.Parameters.Add("@PRICE", SqlDbType.Float).Value = Price;
                cmd.Parameters.Add("@ORIGINALPRICE", SqlDbType.Float).Value = OriginalPrice;

                try
                {
                    id = (int)((decimal)cmd.ExecuteScalar());
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

        static public bool DeletePHONE(SqlConnection connection,int ID)
        {
            string query = """
                Delete from PHONE where ID=@ID
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
                    Console.WriteLine(ex.ToString());
                    connection.Close();
                    return false;
                }
            }
            connection.Close();
            return true;
        }

        static public bool UpdatePHONE(SqlConnection connection, int ID,string name, int ManufacturerID, string Thumbnail, double Price, double OriginalPrice)
        {
            string query = """
                Update PHONE set NAME=@Name, MANUFACTURER_ID=@ManufacturerID, THUMBNAIL=@Thumbnail, PRICE=@Price, ORIGINALPRICE =@OriginalPrice where ID=@ID
                """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = name;
                cmd.Parameters.Add("@ManufacturerID", SqlDbType.Int).Value = ManufacturerID;
                cmd.Parameters.Add("@Thumbnail", SqlDbType.VarChar).Value = Thumbnail;
                cmd.Parameters.Add("@Price", SqlDbType.Float).Value = Price;
                cmd.Parameters.Add("@OriginalPrice", SqlDbType.Float).Value = OriginalPrice;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    connection.Close();
                    return false;
                }
            }
            connection.Close();
            return true;
        }
    }
}
