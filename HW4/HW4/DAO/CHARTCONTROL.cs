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
        static public DateTime TakeMaxOrderDate(SqlConnection connection)
        {
            DateTime newestOrderedDate = new DateTime();

            string sql = """
                              select MAX(CREATED_DATE) AS 'NEWESTORDEREDDATE'
                              from ORDERS
                         """;
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

        static public int AmountProducts(SqlConnection connection, DateTime beginDate, DateTime endDate)
        {
            string sql = """
                              select ORDER_ID
                              from ORDERS
                              where CREATED_DATE BETWEEN @StartDate AND @EndDate
                         """;
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

        static public int AmountOnSales(SqlConnection connection)
        {
            string sql = """
                              select count(distinct PHONE_ID) AS 'NUMBER'
                              from ORDERS_PHONE
                         """;
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

        static public ObservableCollection<INCOMECHART> incomeChartDataHandling(SqlConnection connection, DateTime begin, DateTime end)
        {
            var incomechartlist = new ObservableCollection<INCOMECHART>();

            string sql = """
                              select O.ORDER_ID, O.CREATED_DATE, O.TOTAL, 
                                     (O.TOTAL - P.ORIGINALPRICE*OP.PHONE_COUNT) AS 'PROFIT'
                              from ORDERS O, ORDERS_PHONE OP, PHONE P
                              where OP.PHONE_ID = P.ID
                         	    and OP.ORDER_ID = O.ORDER_ID
                                and CREATED_DATE BETWEEN @StartDate AND @EndDate
                         """;
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

                while (reader.Read())
                {
                    int OrderID = (int)reader["ORDER_ID"];
                    DateTime OrderDate = (DateTime)reader["CREATED_DATE"];
                    double TotalPrice = 0;
                    double ProfitAll = 0;

                    if (reader["TOTAL"].GetType() != typeof(DBNull))
                        TotalPrice = (double)reader["TOTAL"];
                    if (reader["PROFIT"].GetType() != typeof(DBNull))
                        ProfitAll = (double)reader["PROFIT"];

                    incomechartlist.Add(new INCOMECHART()
                    {
                        OrderDate = OrderDate,
                        TotalPrice = TotalPrice,
                        Profit = ProfitAll
                    });
                }
                reader.Close();
            }

            var res = new ObservableCollection<INCOMECHART>(incomechartlist);

            return res;
        }

        static public Tuple<ObservableCollection<QUANTITYSOLDCHART>, int> quantitysoldChartDataHandling(SqlConnection connection, DateTime begin, DateTime end)
        {
            var quantitysoldchartlist = new ObservableCollection<QUANTITYSOLDCHART>();

            string sql = """
                             select distinct P.NAME, SUM(OP.PHONE_COUNT) AS 'QUANTITY_SOLD'
                             from ORDERS O, ORDERS_PHONE OP, PHONE P
                             where P.ID = OP.PHONE_ID
                                 and OP.ORDER_ID = O.ORDER_ID
                                 and O.CREATED_DATE BETWEEN @StartDate AND @EndDate
                             group by P.NAME
                             order by 'QUANTITY_SOLD'
                          """;

            int numberOfPhone = 0;

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

                while (reader.Read())
                {
                    string PhoneName = (string)reader["NAME"];

                    int QuantitySold = (int)reader["QUANTITY_SOLD"];

                    quantitysoldchartlist.Add(new QUANTITYSOLDCHART()
                    {
                        PhoneName = PhoneName,
                        QuantitySold = QuantitySold
                    });
                    numberOfPhone++;
                }
                reader.Close();
            }
            connection.Close();

            var res = new Tuple<ObservableCollection<QUANTITYSOLDCHART>, int>
                                (quantitysoldchartlist, numberOfPhone);

            return res;
        }
    }
}
