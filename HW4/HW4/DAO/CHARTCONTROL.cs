using HW4.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HW4.DAO
{
    public class CHARTCONTROL(SqlConnection connection)
    {
        static public DateTime TakeMaxOrderDate(SqlConnection connection, string querystring)
        {
            DateTime newestOrderedDate = new DateTime();

            string sql = querystring;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            using (var command = new SqlCommand(sql, connection))
            {
                //_connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    newestOrderedDate = (DateTime)reader["NEWESTORDEREDDATE"];
                }
                reader.Close();
            }
            connection.Close();

            var res = newestOrderedDate;

            return res;
        }

        static public int AmountProducts(SqlConnection connection, DateTime beginDate, DateTime endDate, string querystring)
        {
            string sql = querystring;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            int numberOfOrders = 0;

            using (var command = new SqlCommand(sql, connection))
            {
                //_connection.Open();
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = beginDate;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;

                //_connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                    numberOfOrders++;
                reader.Close();
            }
            return numberOfOrders;
        }

        static public int AmountOnSales(SqlConnection connection, string querystring)
        {
            string sql = querystring;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            int numberOnSales = 0;

            using (var command = new SqlCommand(sql, connection))
            {
                //_connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                    numberOnSales = (int)reader["NUMBER"];
                reader.Close();
            }
            return numberOnSales;
        }

        static public DataTable DataHandling(SqlConnection connection,
                                             DateTime begin, DateTime end, string querystring)
        {
            DataTable dataTable = new DataTable();

            string sql = querystring;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            using (var command = new SqlCommand(sql, connection))
            {
                //_connection.Open();
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = begin;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = end;

                //_connection.Open();
                var reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }

            connection.Close();

            var res = dataTable;

            return res;
        }
    }
}
