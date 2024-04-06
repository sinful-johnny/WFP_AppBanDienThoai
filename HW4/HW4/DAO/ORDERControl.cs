using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using HW4.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HW4
{
    public class ORDERControl(SqlConnection connect)
    {
        static public Tuple<ObservableCollection<ORDER>, int, int> GetAllPaging(SqlConnection connection, int page, int rowsPerPage)
        {
            int totalItems = -1;
            int totalPages = -1;
            var orders = new ObservableCollection<ORDER>();
            int skip = (page - 1) * 10;
            int take = rowsPerPage;
            string sql = """
                              select ORDERS.ORDER_ID, CUSTOMER.FIRSTNAME, CUSTOMER.LASTNAME, ORDERS.CREATED_DATE, ORDERS.TOTAL, ORDERS.STATUS FROM ORDERS, CUSTOMER
                              WHERE ORDERS.CUSTOMER_ID = CUSTOMER.CUS_ID
                              ORDER BY ORDERS.ORDER_ID
                              OFFSET @Skip ROWS
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

                    int OrderID = (int)reader["ORDER_ID"];
                    string FullName = (string)reader["FIRSTNAME"] + " " + (string)reader["LASTNAME"];
                    DateTime OrderDate = (DateTime)reader["CREATED_DATE"];
                    double TotalPrice = (double)reader["TOTAL"];
                    string status = (string)reader["STATUS"];
                    string sql2 = """
                              select PHONE.ID, PHONE.NAME, ORDERS_PHONE.PHONE_COUNT, ORDERS_PHONE.TOTAL from ORDERS, ORDERS_PHONE JOIN PHONE ON PHONE.ID = ORDERS_PHONE.PHONE_ID
                              WHERE ORDERS_PHONE.ORDER_ID = @Id 
                              ORDER BY PHONE.ID
                         """;
                    ObservableCollection<ORDEREDPHONE> OrderedPhones = [];
                    using (var command2 = new SqlCommand(sql2, connection)) 
                    {
                        command2.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader2 = command.ExecuteReader();
                        while (reader2.Read()) 
                        {
                            OrderedPhones.Add(new ORDEREDPHONE()
                            {
                                PhoneID = (int)reader2["ID"],
                                PhoneName = (string)reader2["NAME"],
                                quantity = (int)reader2["PHONE_COUNT"],
                                Price = (float)reader2["TOTAL"]
                            });
                        }
                    }

                    ObservableCollection<PROMOTIONSINORDER> Promo_List = [];
                    string sql3 = """
                              select PROMOTIONS.PROMO_NAME, PROMOTIONS.PROMO_ID, PHONE.NAME AS PHONE_NAME, PROMOTIONS.DISCOUNT from PROMOTIONS 
                              JOIN PHONE ON PHONE.ID = PROMOTIONS.PROMO_PHONE_ID
                              , PROMO_ORDERS
                              WHERE PROMO_ORDERS.ORDER_ID = @Id AND PROMOTIONS.PROMO_ID = PROMO_ORDERS.PROMO_ID
                         """;
                    using (var command3 = new SqlCommand(sql3, connection))
                    {
                        command3.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader3 = command.ExecuteReader();
                        while (reader3.Read())
                        {
                            Promo_List.Add(new PROMOTIONSINORDER()
                            {
                                PromoID = (int)reader3["PROMO_ID"],
                                PromoName = (string)reader3["PROMO_NAME"],
                                forPhone = (string)reader3["PHONE_NAME"],
                                Discount = (float)reader3["DISCOUNT"]
                            });
                        }
                    }
                    orders.Add(new ORDER()
                    {
                        OrderID = OrderID,
                        CustomerName = FullName,
                        OrderDate = OrderDate,
                        OrderPromos = OrderedPhones,
                        PromoList = Promo_List,
                        TotalPrice = TotalPrice,
                        status = status
                    });
                }
            }

            connection.Close();
            var result = new Tuple<ObservableCollection<ORDER>, int, int>(orders, totalItems, totalPages);
            return result;
        }
        static public Tuple<ObservableCollection<ORDER>, int, int> GetAllTodayPaging(SqlConnection connection, int page, int rowsPerPage)
        {
            int totalItems = -1;
            int totalPages = -1;
            var orders = new ObservableCollection<ORDER>();
            int skip = (page - 1) * 10;
            int take = rowsPerPage;
            string sql = """
                              select ORDERS.ORDER_ID, CUSTOMER.FIRSTNAME, CUSTOMER.LASTNAME, ORDERS.CREATED_DATE, ORDERS.TOTAL, ORDERS.STATUS FROM ORDERS, CUSTOMER
                              WHERE ORDERS.CUSTOMER_ID = CUSTOMER.CUS_ID AND CONVERT(DATE, ORDERS.CREATED_DATE) = @Today
                              ORDER BY ORDERS.ORDER_ID
                              OFFSET @Skip ROWS
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
                command.Parameters.Add("@Today", SqlDbType.DateTime).Value = DateTime.Today;
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

                    int OrderID = (int)reader["ORDER_ID"];
                    string FullName = (string)reader["FIRSTNAME"] + " " + (string)reader["LASTNAME"];
                    DateTime OrderDate = (DateTime)reader["CREATED_DATE"];
                    double TotalPrice = (double)reader["TOTAL"];
                    string status = (string)reader["STATUS"];
                    string sql2 = """
                              select PHONE.ID, PHONE.NAME, ORDERS_PHONE.PHONE_COUNT, ORDERS_PHONE.TOTAL from ORDERS, ORDERS_PHONE JOIN PHONE ON PHONE.ID = ORDERS_PHONE.PHONE_ID
                              WHERE ORDERS_PHONE.ORDER_ID = @Id 
                              ORDER BY PHONE.ID
                         """;
                    ObservableCollection<ORDEREDPHONE> OrderedPhones = [];
                    using (var command2 = new SqlCommand(sql2, connection))
                    {
                        command2.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader2 = command.ExecuteReader();
                        while (reader2.Read())
                        {
                            OrderedPhones.Add(new ORDEREDPHONE()
                            {
                                PhoneID = (int)reader2["ID"],
                                PhoneName = (string)reader2["NAME"],
                                quantity = (int)reader2["PHONE_COUNT"],
                                Price = (float)reader2["TOTAL"]
                            });
                        }
                    }

                    ObservableCollection<PROMOTIONSINORDER> Promo_List = [];
                    string sql3 = """
                              select PROMOTIONS.PROMO_NAME, PROMOTIONS.PROMO_ID, PHONE.NAME AS PHONE_NAME, PROMOTIONS.DISCOUNT from PROMOTIONS 
                              JOIN PHONE ON PHONE.ID = PROMOTIONS.PROMO_PHONE_ID
                              , PROMO_ORDERS
                              WHERE PROMO_ORDERS.ORDER_ID = @Id AND PROMOTIONS.PROMO_ID = PROMO_ORDERS.PROMO_ID
                         """;
                    using (var command3 = new SqlCommand(sql3, connection))
                    {
                        command3.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader3 = command.ExecuteReader();
                        while (reader3.Read())
                        {
                            Promo_List.Add(new PROMOTIONSINORDER()
                            {
                                PromoID = (int)reader3["PROMO_ID"],
                                PromoName = (string)reader3["PROMO_NAME"],
                                forPhone = (string)reader3["PHONE_NAME"],
                                Discount = (float)reader3["DISCOUNT"]
                            });
                        }
                    }
                    orders.Add(new ORDER()
                    {
                        OrderID = OrderID,
                        CustomerName = FullName,
                        OrderDate = OrderDate,
                        OrderPromos = OrderedPhones,
                        PromoList = Promo_List,
                        TotalPrice = TotalPrice,
                        status = status
                    });
                }
            }

            connection.Close();
            var result = new Tuple<ObservableCollection<ORDER>, int, int>(orders, totalItems, totalPages);
            return result;
        }
        static public Tuple<ObservableCollection<ORDER>, int, int> GetAllThisWeekPaging(SqlConnection connection, int page, int rowsPerPage)
        {
            int totalItems = -1;
            int totalPages = -1;
            var orders = new ObservableCollection<ORDER>();
            int skip = (page - 1) * 10;
            int take = rowsPerPage;
            string sql = """
                              select ORDERS.ORDER_ID, CUSTOMER.FIRSTNAME, CUSTOMER.LASTNAME, ORDERS.CREATED_DATE, ORDERS.TOTAL, ORDERS.STATUS FROM ORDERS, CUSTOMER
                              WHERE ORDERS.CUSTOMER_ID = CUSTOMER.CUS_ID AND DATEPART(ISO_WEEK, ORDERS.CREATED_DATE) = DATEPART(ISO_WEEK, @Today) 
                              AND YEAR(ORDERS.CREATED_DATE) = YEAR(@Today)
                              ORDER BY ORDERS.ORDER_ID
                              OFFSET @Skip ROWS
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
                command.Parameters.Add("@Today", SqlDbType.DateTime).Value = DateTime.Today;
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

                    int OrderID = (int)reader["ORDER_ID"];
                    string FullName = (string)reader["FIRSTNAME"] + " " + (string)reader["LASTNAME"];
                    DateTime OrderDate = (DateTime)reader["CREATED_DATE"];
                    double TotalPrice = (double)reader["TOTAL"];
                    string status = (string)reader["STATUS"];
                    string sql2 = """
                              select PHONE.ID, PHONE.NAME, ORDERS_PHONE.PHONE_COUNT, ORDERS_PHONE.TOTAL from ORDERS, ORDERS_PHONE JOIN PHONE ON PHONE.ID = ORDERS_PHONE.PHONE_ID
                              WHERE ORDERS_PHONE.ORDER_ID = @Id 
                              ORDER BY PHONE.ID
                         """;
                    ObservableCollection<ORDEREDPHONE> OrderedPhones = [];
                    using (var command2 = new SqlCommand(sql2, connection))
                    {
                        command2.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader2 = command.ExecuteReader();
                        while (reader2.Read())
                        {
                            OrderedPhones.Add(new ORDEREDPHONE()
                            {
                                PhoneID = (int)reader2["ID"],
                                PhoneName = (string)reader2["NAME"],
                                quantity = (int)reader2["PHONE_COUNT"],
                                Price = (float)reader2["TOTAL"]
                            });
                        }
                    }

                    ObservableCollection<PROMOTIONSINORDER> Promo_List = [];
                    string sql3 = """
                              select PROMOTIONS.PROMO_NAME, PROMOTIONS.PROMO_ID, PHONE.NAME AS PHONE_NAME, PROMOTIONS.DISCOUNT from PROMOTIONS 
                              JOIN PHONE ON PHONE.ID = PROMOTIONS.PROMO_PHONE_ID
                              , PROMO_ORDERS
                              WHERE PROMO_ORDERS.ORDER_ID = @Id AND PROMOTIONS.PROMO_ID = PROMO_ORDERS.PROMO_ID
                         """;
                    using (var command3 = new SqlCommand(sql3, connection))
                    {
                        command3.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader3 = command.ExecuteReader();
                        while (reader3.Read())
                        {
                            Promo_List.Add(new PROMOTIONSINORDER()
                            {
                                PromoID = (int)reader3["PROMO_ID"],
                                PromoName = (string)reader3["PROMO_NAME"],
                                forPhone = (string)reader3["PHONE_NAME"],
                                Discount = (float)reader3["DISCOUNT"]
                            });
                        }
                    }
                    orders.Add(new ORDER()
                    {
                        OrderID = OrderID,
                        CustomerName = FullName,
                        OrderDate = OrderDate,
                        OrderPromos = OrderedPhones,
                        PromoList = Promo_List,
                        TotalPrice = TotalPrice,
                        status = status
                    });
                }
            }

            connection.Close();
            var result = new Tuple<ObservableCollection<ORDER>, int, int>(orders, totalItems, totalPages);
            return result;
        }
        static public Tuple<ObservableCollection<ORDER>, int, int> GetAllThisMonthPaging(SqlConnection connection, int page, int rowsPerPage)
        {
            int totalItems = -1;
            int totalPages = -1;
            var orders = new ObservableCollection<ORDER>();
            int skip = (page - 1) * 10;
            int take = rowsPerPage;
            string sql = """
                              select ORDERS.ORDER_ID, CUSTOMER.FIRSTNAME, CUSTOMER.LASTNAME, ORDERS.CREATED_DATE, ORDERS.TOTAL, ORDERS.STATUS FROM ORDERS, CUSTOMER
                              WHERE ORDERS.CUSTOMER_ID = CUSTOMER.CUS_ID AND MONTH(ORDERS.CREATED_DATE) = MONTH(@Today) AND YEAR(ORDERS.CREATED_DATE) = YEAR(@Today)
                              ORDER BY ORDERS.ORDER_ID
                              OFFSET @Skip ROWS
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
                command.Parameters.Add("@Today", SqlDbType.DateTime).Value = DateTime.Today;
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

                    int OrderID = (int)reader["ORDER_ID"];
                    string FullName = (string)reader["FIRSTNAME"] + " " + (string)reader["LASTNAME"];
                    DateTime OrderDate = (DateTime)reader["CREATED_DATE"];
                    double TotalPrice = (double)reader["TOTAL"];
                    string status = (string)reader["STATUS"];
                    string sql2 = """
                              select PHONE.ID, PHONE.NAME, ORDERS_PHONE.PHONE_COUNT, ORDERS_PHONE.TOTAL from ORDERS, ORDERS_PHONE JOIN PHONE ON PHONE.ID = ORDERS_PHONE.PHONE_ID
                              WHERE ORDERS_PHONE.ORDER_ID = @Id 
                              ORDER BY PHONE.ID
                         """;
                    ObservableCollection<ORDEREDPHONE> OrderedPhones = [];
                    using (var command2 = new SqlCommand(sql2, connection))
                    {
                        command2.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader2 = command.ExecuteReader();
                        while (reader2.Read())
                        {
                            OrderedPhones.Add(new ORDEREDPHONE()
                            {
                                PhoneID = (int)reader2["ID"],
                                PhoneName = (string)reader2["NAME"],
                                quantity = (int)reader2["PHONE_COUNT"],
                                Price = (float)reader2["TOTAL"]
                            });
                        }
                    }

                    ObservableCollection<PROMOTIONSINORDER> Promo_List = [];
                    string sql3 = """
                              select PROMOTIONS.PROMO_NAME, PROMOTIONS.PROMO_ID, PHONE.NAME AS PHONE_NAME, PROMOTIONS.DISCOUNT from PROMOTIONS 
                              JOIN PHONE ON PHONE.ID = PROMOTIONS.PROMO_PHONE_ID
                              , PROMO_ORDERS
                              WHERE PROMO_ORDERS.ORDER_ID = @Id AND PROMOTIONS.PROMO_ID = PROMO_ORDERS.PROMO_ID
                         """;
                    using (var command3 = new SqlCommand(sql3, connection))
                    {
                        command3.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader3 = command.ExecuteReader();
                        while (reader3.Read())
                        {
                            Promo_List.Add(new PROMOTIONSINORDER()
                            {
                                PromoID = (int)reader3["PROMO_ID"],
                                PromoName = (string)reader3["PROMO_NAME"],
                                forPhone = (string)reader3["PHONE_NAME"],
                                Discount = (float)reader3["DISCOUNT"]
                            });
                        }
                    }
                    orders.Add(new ORDER()
                    {
                        OrderID = OrderID,
                        CustomerName = FullName,
                        OrderDate = OrderDate,
                        OrderPromos = OrderedPhones,
                        PromoList = Promo_List,
                        TotalPrice = TotalPrice,
                        status = status
                    });
                }
            }

            connection.Close();
            var result = new Tuple<ObservableCollection<ORDER>, int, int>(orders, totalItems, totalPages);
            return result;
        }
        static public Tuple<ObservableCollection<ORDER>, int, int> FromDateToDate(SqlConnection connection, int page, int rowsPerPage, DateTime begin, DateTime end)
        {
            int totalItems = -1;
            int totalPages = -1;
            var orders = new ObservableCollection<ORDER>();
            int skip = (page - 1) * 10;
            int take = rowsPerPage;
            string sql = """
                              select ORDERS.ORDER_ID, CUSTOMER.FIRSTNAME, CUSTOMER.LASTNAME, ORDERS.CREATED_DATE, ORDERS.TOTAL, ORDERS.STATUS FROM ORDERS, CUSTOMER
                              WHERE ORDERS.CUSTOMER_ID = CUSTOMER.CUS_ID AND ORDERS.CREATED_DATE BETWEEN @StartDate AND @EndDate
                              ORDER BY ORDERS.ORDER_ID
                              OFFSET @Skip ROWS
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
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = begin;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = end;
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

                    int OrderID = (int)reader["ORDER_ID"];
                    string FullName = (string)reader["FIRSTNAME"] + " " + (string)reader["LASTNAME"];
                    DateTime OrderDate = (DateTime)reader["CREATED_DATE"];
                    double TotalPrice = (double)reader["TOTAL"];
                    string status = (string)reader["STATUS"];
                    string sql2 = """
                              select PHONE.ID, PHONE.NAME, ORDERS_PHONE.PHONE_COUNT, ORDERS_PHONE.TOTAL from ORDERS, ORDERS_PHONE JOIN PHONE ON PHONE.ID = ORDERS_PHONE.PHONE_ID
                              WHERE ORDERS_PHONE.ORDER_ID = @Id 
                              ORDER BY PHONE.ID
                         """;
                    ObservableCollection<ORDEREDPHONE> OrderedPhones = [];
                    using (var command2 = new SqlCommand(sql2, connection))
                    {
                        command2.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader2 = command.ExecuteReader();
                        while (reader2.Read())
                        {
                            OrderedPhones.Add(new ORDEREDPHONE()
                            {
                                PhoneID = (int)reader2["ID"],
                                PhoneName = (string)reader2["NAME"],
                                quantity = (int)reader2["PHONE_COUNT"],
                                Price = (float)reader2["TOTAL"]
                            });
                        }
                    }

                    ObservableCollection<PROMOTIONSINORDER> Promo_List = [];
                    string sql3 = """
                              select PROMOTIONS.PROMO_NAME, PROMOTIONS.PROMO_ID, PHONE.NAME AS PHONE_NAME, PROMOTIONS.DISCOUNT from PROMOTIONS 
                              JOIN PHONE ON PHONE.ID = PROMOTIONS.PROMO_PHONE_ID
                              , PROMO_ORDERS
                              WHERE PROMO_ORDERS.ORDER_ID = @Id AND PROMOTIONS.PROMO_ID = PROMO_ORDERS.PROMO_ID
                         """;
                    using (var command3 = new SqlCommand(sql3, connection))
                    {
                        command3.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader3 = command.ExecuteReader();
                        while (reader3.Read())
                        {
                            Promo_List.Add(new PROMOTIONSINORDER()
                            {
                                PromoID = (int)reader3["PROMO_ID"],
                                PromoName = (string)reader3["PROMO_NAME"],
                                forPhone = (string)reader3["PHONE_NAME"],
                                Discount = (float)reader3["DISCOUNT"]
                            });
                        }
                    }
                    orders.Add(new ORDER()
                    {
                        OrderID = OrderID,
                        CustomerName = FullName,
                        OrderDate = OrderDate,
                        OrderPromos = OrderedPhones,
                        PromoList = Promo_List,
                        TotalPrice = TotalPrice,
                        status = status
                    });
                }
            }

            connection.Close();
            var result = new Tuple<ObservableCollection<ORDER>, int, int>(orders, totalItems, totalPages);
            return result;
        }
        static public int AddOrder(SqlConnection connection, int CustomerID, ObservableCollection<ORDEREDPHONE> OrderedPhone)
        {
            string sql = """
                Insert into ORDERS(CUSTOMER_ID, CREATED_DATE, STATUS)
                values(@CustomerID, @Today, 'Pending')
                """;
            int id = -1;
            if (connection.State == ConnectionState.Closed) 
            {
                connection.Open();
            }
            double total = 0;
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = CustomerID;
                command.Parameters.Add("@Today", SqlDbType.DateTime).Value = DateTime.Now;
                List<int> PromoIDS = [];
                try
                {
                    id = (int)((decimal)command.ExecuteScalar());
                    string sql2 = """
                        INSERT INTO PROMO_ORDERS (PROMO_ID, ORDER_ID)
                        SELECT PROMO_CUSTOMER.PROMO_ID, ORDERS.ORDER_ID
                        FROM PROMO_CUSTOMER, ORDERS
                        WHERE PROMO_CUSTOMER.CUS_ID = ORDERS.CUSTOMER_ID AND ORDERS.ORDER_ID = @Id 
                        AND PROMO_CUSTOMER.USAGE_STATUS != 'Expired'
                        """;
                    using var command2 = new SqlCommand(sql2, connection);
                    command2.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    command2.ExecuteNonQuery();

                    foreach (var product in OrderedPhone)
                    {
                        double price = product.Price * product.quantity;
                        foreach (var promoID in PromoIDS)
                        {
                            
                            string query1 = """
                                SELECT PROMOTIONS.DISCOUNT
                                FROM PHONE JOIN PROMOTIONS ON PROMOTIONS.PROMO_PHONE_ID = PHONE.ID
                                WHERE PROMOTIONS.PROMO_ID = @PromoID AND PHONE.ID = @PhoneId
                                """;

                            using var tempComm = new SqlCommand(query1, connection);
                            tempComm.Parameters.Add("@PromoID", SqlDbType.Int).Value = promoID;
                            tempComm.Parameters.Add("@PhoneID", SqlDbType.Int).Value = product.PhoneID;
                            var reader3 = tempComm.ExecuteReader();
                            while (reader3.Read())
                            {
                                float discount = (float)reader3["DISCOUNT"];
                                price *= (float)(1 - (float)(discount/100));
                            }
                        }

                        total += price;
                        string query2 = """
                                INSERT INTO ORDERS_PHONE (ORDER_ID, PHONE_ID, PHONE_COUNT, TOTAL)
                                VALUES (@OrderID, @PhoneID, @count, @total)
                                """;
                        using var tempComm2 = new SqlCommand(query2, connection);
                        tempComm2.Parameters.Add("@OrderID", SqlDbType.Int).Value = id;
                        tempComm2.Parameters.Add("@PhoneID", SqlDbType.Int).Value = product.PhoneID;
                        tempComm2.Parameters.Add("@count", SqlDbType.Int).Value = product.quantity;
                        tempComm2.Parameters.Add("@total", SqlDbType.Float).Value = total;

                        tempComm2.ExecuteNonQuery();
                    }

                    string sqlFinal = """
                            UPDATE ORDERS
                            SET TOTAL = @total
                            WHERE ORDERS.ORDER_ID = @id
                        """;
                    using var finalComm = new SqlCommand(sqlFinal, connection);
                    finalComm.Parameters.Add("@total", SqlDbType.Float).Value = total;
                    finalComm.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    finalComm.ExecuteNonQuery();
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
        static public bool DeleteOrder(SqlConnection connection, int OrderID)
        {
            string sql1 = """
                DELETE FROM PROMO_ORDERS WHERE ORDER_ID = @OrderId
                """;
            string sql2 = """
                DELETE FROM ORDERS_PHONE WHERE ORDER_ID = @OrderId
                """;
            string sql3 = """
                DELETE FROM ORDERS WHERE ORDER_ID = @OrderId
                """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            using (var command1 = new SqlCommand(sql1, connection))
            {
                command1.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                try
                {
                    command1.ExecuteNonQuery();
                }
                catch (Exception ex1)
                {
                    connection.Close();
                    return false;
                }
            }

            using (var command2 = new SqlCommand(sql2, connection))
            {
                command2.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                try
                {
                    command2.ExecuteNonQuery();
                }
                catch (Exception ex2)
                {
                    connection.Close();
                    return false;
                }
            }
            using (var command3 = new SqlCommand(sql3, connection))
            {
                command3.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                try
                {
                    command3.ExecuteNonQuery();
                }
                catch (Exception ex2)
                {
                    connection.Close();
                    return false;
                }
            }
            return true;
        }
        static public bool UpDateStatus(SqlConnection connection, string status, int OrderID)
        {
            string sql = """
                    UPDATE ORDERS
                    SET STATUS = @status
                    WHERE ORDERS.ORDER_ID = @OrderID
                """;

            if (connection.State == ConnectionState.Closed) 
            {
                connection.Open();
            }
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@status", SqlDbType.Text).Value = status;
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    connection.Close();
                    return false;
                }
            }
            return true;
        }
        static public bool NewPhoneInOrder(SqlConnection connection, ORDEREDPHONE Phone, int OrderID)
        {
            string validation = """
                IF EXISTS(SELECT 1 FROM ORDERS_PHONE WHERE ORDER_ID = @OrderID AND PHONE_ID = @PhoneID)
                    SELECT 1; -- Returns 1 if exists
                ELSE
                    SELECT 0; -- Returns 0 if not exists
                """;

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using SqlCommand validCommand = new SqlCommand(validation, connection);
            validCommand.Parameters.Add("@PhoneID", SqlDbType.Int).Value = Phone.PhoneID;
            validCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
            try
            {
                int exist = (int)validCommand.ExecuteScalar();

                double price = Phone.quantity * Phone.Price;
                
                string getPromo = """
                        SELECT PROMOTIONS.DISCOUNT
                        FROM PROMO_ORDERS JOIN PROMOTIONS ON PROMOTIONS.PROMO_ID = PROMO_ORDERS.PROMO_ID
                        WHERE PROMOTIONS.PROMO_PHONE_ID = @PhoneId AND PROMO_ORDERS = @OrderID
                    """;

                using var discCommand = new SqlCommand(getPromo, connection);
                discCommand.Parameters.Add("@PhoneID", SqlDbType.Int).Value = Phone.PhoneID;
                discCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
        
                var readingDiscount = discCommand.ExecuteReader();
                while (readingDiscount.Read())
                {
                    float discount = (float)readingDiscount["DISCOUNT"];
                    price *= (float)(1 - (float)(discount/100));
                }

                if (exist == 0)
                {
                    string sql = """
                        UPDATE ORDERS_PHONE
                        SET PHONE_COUNT = PHONE_COUNT + @count, TOTAL = TOTAL + @price
                        WHERE ORDER_ID = @OrderID AND PHONE_ID = @PhoneID
                    """;
                    using var UpdCommand = new SqlCommand(sql, connection);
                    UpdCommand.Parameters.Add("@PhoneID", SqlDbType.Int).Value = Phone.PhoneID;
                    UpdCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                    UpdCommand.Parameters.Add("@count", SqlDbType.Int).Value = Phone.quantity;
                    UpdCommand.Parameters.Add("@price", SqlDbType.Float).Value = price;
                    UpdCommand.ExecuteNonQuery();
                }
                else
                {
                    string sql = """
                        INSERT INTO ORDERS_PHONE(ORDER_ID, PHONE_ID, PHONE_COUNT, TOTAL)
                        VALUES (@OrderID, @PhoneID, @count, @price)
                    """;
                    using var UpdCommand = new SqlCommand(sql, connection);
                    UpdCommand.Parameters.Add("@PhoneID", SqlDbType.Int).Value = Phone.PhoneID;
                    UpdCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                    UpdCommand.Parameters.Add("@count", SqlDbType.Int).Value = Phone.quantity;
                    UpdCommand.Parameters.Add("@price", SqlDbType.Float).Value = price;
                    UpdCommand.ExecuteNonQuery();
                }

                string updateOrder = """
                        UPDATE ORDERS
                        SET TOTAL = TOTAL + @price
                        WHERE ORDER_ID = @OrderID
                    """;
                using var orderCommand = new SqlCommand(updateOrder, connection);
                orderCommand.Parameters.Add("@price", SqlDbType.Float).Value = price;
                orderCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                orderCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                connection.Close();
                return false;
            }

            connection.Close();
            return true;
        }
        static public bool DeletePhoneInOrder(SqlConnection connection, int PhoneID, int OrderID)
        {
            string deletePhone = """
                DELETE FROM ORDERS_PHONE
                OUTPUT TOTAL
                WHERE ORDER_ID = @OrderID AND PHONE_ID = @PhoneID
                """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using SqlCommand DeleteCommand = new SqlCommand(deletePhone, connection);
            DeleteCommand.Parameters.Add("@PhoneID", SqlDbType.Int).Value = PhoneID;
            DeleteCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;

            try
            {
                float price = 0;
                var reader = DeleteCommand.ExecuteReader();
                while (reader.Read())
                {
                    price = (float)reader["TOTAL"]; 
                }

                string updateOrder = """
                        UPDATE ORDERS
                        SET TOTAL = TOTAL - @price
                        WHERE ORDER_ID = @OrderID
                    """;
                using var orderCommand = new SqlCommand(updateOrder, connection);
                orderCommand.Parameters.Add("@price", SqlDbType.Float).Value = price;
                orderCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                orderCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                connection.Close();
                return false;
            }
            return true;
        }
        static public bool ChangePhoneCount(SqlConnection connection, ORDEREDPHONE Phone, int OrderID)
        {
            string sql = """
                        UPDATE ORDERS_PHONE
                        SET PHONE_COUNT = @count, TOTAL = @price
                        OUTPUT TOTAL
                        WHERE ORDER_ID = @OrderID AND PHONE_ID = @PhoneID
                    """;
            string getPromo = """
                        SELECT PROMOTIONS.DISCOUNT
                        FROM PROMO_ORDERS JOIN PROMOTIONS ON PROMOTIONS.PROMO_ID = PROMO_ORDERS.PROMO_ID
                        WHERE PROMOTIONS.PROMO_PHONE_ID = @PhoneId AND PROMO_ORDERS = @OrderID
                    """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            double price = Phone.Price * Phone.quantity;
            double oldPrice = 0;
            using var discCommand = new SqlCommand(getPromo, connection);
            discCommand.Parameters.Add("@PhoneID", SqlDbType.Int).Value = Phone.PhoneID;
            discCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
            try
            {
                var readingDiscount = discCommand.ExecuteReader();
                while (readingDiscount.Read())
                {
                    float discount = (float)readingDiscount["DISCOUNT"];
                    price *= (float)(1 - (float)(discount/100));
                }

                using SqlCommand UpdateCommand = new SqlCommand(sql, connection);
                UpdateCommand.Parameters.Add("@PhoneID", SqlDbType.Int).Value = Phone.PhoneID;
                UpdateCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                UpdateCommand.Parameters.Add("@count", SqlDbType.Int).Value = Phone.quantity;
                UpdateCommand.Parameters.Add("@price", SqlDbType.Int).Value = price;

                var updateReader = UpdateCommand.ExecuteReader();
                while (updateReader.Read())
                {
                    oldPrice += (float)updateReader["TOTAL"];
                }

                string updateOrder = """
                        UPDATE ORDERS
                        SET TOTAL = TOTAL - @oldPrice + @price
                        WHERE ORDER_ID = @OrderID
                    """;
                using var orderCommand = new SqlCommand(updateOrder, connection);
                orderCommand.Parameters.Add("@price", SqlDbType.Float).Value = price;
                orderCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                orderCommand.Parameters.Add("@oldPrice", SqlDbType.Int).Value = oldPrice;
                orderCommand.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                connection.Close();
                return false;
            }
 
            return true;
        }
        static public bool AddPromo(SqlConnection connection, int promoID, int OrderID)
        {
            string sql = """
                        INSERT INTO PROMO_ORDERS (ORDER_ID, PROMO_ID)
                        VALUES (@OrderID, @promoID)
                    """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            using var addPromo = new SqlCommand(sql, connection);
            addPromo.Parameters.Add("promoID", SqlDbType.Int).Value = promoID;
            addPromo.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
            try
            {
                addPromo.ExecuteNonQuery();
                string getSQL = """
                    UPDATE ORDERS_PHONE
                    SET TOTAL = TOTAL * (1 - PROMOTION.DISCOUNT / 100)
                    OUTPUT deleted.TOTAL as OldPrice, inserted.TOTAL as NewPrice
                    FROM ORDERS_PHONE, PROMOTIONS
                    WHERE ORDERS_PHONE.PHONE_ID = PROMOTIONS.PROMO_PHONE_ID AND PROMOTIONS.PROMO_ID = @promoID AND ORDERS_PHONE.ORDER_ID = @OrderID
                    """;

                using var updatePrice = new SqlCommand(getSQL, connection);
                updatePrice.Parameters.Add("promoID", SqlDbType.Int).Value = promoID;
                updatePrice.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                float oldPrice = 0;
                float newPrice = 0;
                var reader = updatePrice.ExecuteReader();
                while (reader.Read())
                {
                    oldPrice = (float)reader["OldPrice"];
                    newPrice = (float)reader["NewPrice"];
                }

                string updateOrder = """
                    UPDATE ORDERS
                    SET TOTAL = TOTAL - @OldPrice + @NewPrice
                    WHERE ORDER_ID = @OrderID
                    """;

                using var orderCommand = new SqlCommand(updateOrder, connection);
                orderCommand.Parameters.Add("@OldPrice", SqlDbType.Float).Value = oldPrice;
                orderCommand.Parameters.Add("@NewPrice", SqlDbType.Float).Value = newPrice;
                orderCommand.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                connection.Close();
                return false;
            }
            return true;
        }
        static public bool DeletePromo(SqlConnection connection, int promoID, int OrderID)
        {
            string sql = """
                        DELETE FROM PROMO_ORDERS
                        WHERE PROMO_ID = @promoID AND ORDER_ID = @OrderID
                    """;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            using var deletePromo = new SqlCommand(sql, connection);
            deletePromo.Parameters.Add("promoID", SqlDbType.Int).Value = promoID;
            deletePromo.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
            try
            {
                deletePromo.ExecuteNonQuery();
                string getSQL = """
                    UPDATE ORDERS_PHONE
                    SET TOTAL = TOTAL * (1 + PROMOTION.DISCOUNT / 100)
                    OUTPUT deleted.TOTAL as OldPrice, inserted.TOTAL as NewPrice
                    FROM ORDERS_PHONE, PROMOTIONS
                    WHERE ORDERS_PHONE.PHONE_ID = PROMOTIONS.PROMO_PHONE_ID AND PROMOTIONS.PROMO_ID = @promoID AND ORDERS_PHONE.ORDER_ID = @OrderID
                    """;

                using var updatePrice = new SqlCommand(getSQL, connection);
                updatePrice.Parameters.Add("promoID", SqlDbType.Int).Value = promoID;
                updatePrice.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                float oldPrice = 0;
                float newPrice = 0;
                var reader = updatePrice.ExecuteReader();
                while (reader.Read())
                {
                    oldPrice = (float)reader["OldPrice"];
                    newPrice = (float)reader["NewPrice"];
                }

                string updateOrder = """
                    UPDATE ORDERS
                    SET TOTAL = TOTAL - @OldPrice + @NewPrice
                    WHERE ORDER_ID = @OrderID
                    """;

                using var orderCommand = new SqlCommand(updateOrder, connection);
                orderCommand.Parameters.Add("@OldPrice", SqlDbType.Float).Value = oldPrice;
                orderCommand.Parameters.Add("@NewPrice", SqlDbType.Float).Value = newPrice;
                orderCommand.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                connection.Close();
                return false;
            }
            return true;
        }

        static public System.Data.DataTable getCustomerOrder(SqlConnection connection, int CUS_ID)
        {
            string query = """
                select O.*
                from ORDERS as O
                	inner join CUSTOMER as C on C.CUS_ID = O.CUSTOMER_ID
                where C.CUS_ID = @CUS_ID
                """;

            System.Data.DataTable data = new System.Data.DataTable();
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@CUS_ID", SqlDbType.Int).Value = CUS_ID;
                SqlDataReader reader = cmd.ExecuteReader();
                data.Load(reader);
            }
            connection.Close();
            return data;
        }
    }
}
