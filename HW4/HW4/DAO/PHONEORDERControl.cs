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
    public class PHONEORDERControl(SqlConnection connect)
    {
        static public Tuple<BindingList<ORDER>, int, int> GetAllPaging(SqlConnection connection, int page, int rowsPerPage)
        {
            int totalItems = -1;
            int totalPages = -1;
            var orders = new BindingList<ORDER>();
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
                    DateOnly OrderDate = (DateOnly)reader["CREATED_DATE"];
                    string sql2 = """
                              select PHONE.NAME, ORDERS_PHONE.PHONE_COUNT from ORDERS, ORDERS_PHONE
                              WHERE ORDERS_PHONE.ORDER_ID = @Id AND PHONE.ID = ORDERS_PHONE.PHONE_ID
                              ORDER BY PHONE.ID
                         """;
                    BindingList<ORDEREDPHONE> OrderedPhones = [];
                    using (var command2 = new SqlCommand(sql2, connection)) 
                    {
                        command2.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader2 = command.ExecuteReader();
                        while (reader2.Read()) 
                        {
                            OrderedPhones.Add(new ORDEREDPHONE()
                            {
                                boughtPhone = (string)reader2["NAME"],
                                quantity = (int)reader2["PHONE_COUNT"]
                            });
                        }
                    }

                    BindingList<String> Promo_List = [];
                    string sql3 = """
                              select PROMOTIONS.PROMO_NAME from PROMOTIONS, PROMO_ORDERS
                              WHERE PROMO_ORDERS.ORDER_ID = @Id AND PROMOTIONS.PROMO_ID = PROMO_ORDERS.PROMO_ID
                              ORDER BY PHONE.ID
                         """;
                    using (var command3 = new SqlCommand(sql3, connection))
                    {
                        command3.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader3 = command.ExecuteReader();
                        while (reader3.Read())
                        {
                            Promo_List.Add((string)reader3["PROMO_NAME"]);
                        }
                    }
                    orders.Add(new ORDER()
                    {
                        OrderID = OrderID,
                        CustomerName = FullName,
                        OrderDate = OrderDate,
                        OrderedPhone = OrderedPhones,
                        Promo_List = Promo_List
                    });
                }
            }

            var result = new Tuple<BindingList<ORDER>, int, int>(orders, totalItems, totalPages);
            return result;
        }
        static public Tuple<BindingList<ORDER>, int, int> GetAllTodayPaging(SqlConnection connection, int page, int rowsPerPage)
        {
            int totalItems = -1;
            int totalPages = -1;
            var orders = new BindingList<ORDER>();
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
                    DateOnly OrderDate = (DateOnly)reader["CREATED_DATE"];
                    string sql2 = """
                              select PHONE.NAME, ORDERS_PHONE.PHONE_COUNT from ORDERS, ORDERS_PHONE
                              WHERE ORDERS_PHONE.ORDER_ID = @Id AND PHONE.ID = ORDERS_PHONE.PHONE_ID
                              ORDER BY PHONE.ID
                         """;
                    BindingList<ORDEREDPHONE> OrderedPhones = [];
                    using (var command2 = new SqlCommand(sql2, connection))
                    {
                        command2.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader2 = command.ExecuteReader();
                        while (reader2.Read())
                        {
                            OrderedPhones.Add(new ORDEREDPHONE()
                            {
                                boughtPhone = (string)reader2["NAME"],
                                quantity = (int)reader2["PHONE_COUNT"]
                            });
                        }
                    }

                    BindingList<String> Promo_List = [];
                    string sql3 = """
                              select PROMOTIONS.PROMO_NAME from PROMOTIONS, PROMO_ORDERS
                              WHERE PROMO_ORDERS.ORDER_ID = @Id AND PROMOTIONS.PROMO_ID = PROMO_ORDERS.PROMO_ID
                              ORDER BY PHONE.ID
                         """;
                    using (var command3 = new SqlCommand(sql3, connection))
                    {
                        command3.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader3 = command.ExecuteReader();
                        while (reader3.Read())
                        {
                            Promo_List.Add((string)reader3["PROMO_NAME"]);
                        }
                    }
                    orders.Add(new ORDER()
                    {
                        OrderID = OrderID,
                        CustomerName = FullName,
                        OrderDate = OrderDate,
                        OrderedPhone = OrderedPhones,
                        Promo_List = Promo_List
                    });
                }
            }

            var result = new Tuple<BindingList<ORDER>, int, int>(orders, totalItems, totalPages);
            return result;
        }
        static public Tuple<BindingList<ORDER>, int, int> GetAllThisWeekPaging(SqlConnection connection, int page, int rowsPerPage)
        {
            int totalItems = -1;
            int totalPages = -1;
            var orders = new BindingList<ORDER>();
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
                    DateOnly OrderDate = (DateOnly)reader["CREATED_DATE"];
                    string sql2 = """
                              select PHONE.NAME, ORDERS_PHONE.PHONE_COUNT from ORDERS, ORDERS_PHONE
                              WHERE ORDERS_PHONE.ORDER_ID = @Id AND PHONE.ID = ORDERS_PHONE.PHONE_ID
                              ORDER BY PHONE.ID
                         """;
                    BindingList<ORDEREDPHONE> OrderedPhones = [];
                    using (var command2 = new SqlCommand(sql2, connection))
                    {
                        command2.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader2 = command.ExecuteReader();
                        while (reader2.Read())
                        {
                            OrderedPhones.Add(new ORDEREDPHONE()
                            {
                                boughtPhone = (string)reader2["NAME"],
                                quantity = (int)reader2["PHONE_COUNT"]
                            });
                        }
                    }

                    BindingList<String> Promo_List = [];
                    string sql3 = """
                              select PROMOTIONS.PROMO_NAME from PROMOTIONS, PROMO_ORDERS
                              WHERE PROMO_ORDERS.ORDER_ID = @Id AND PROMOTIONS.PROMO_ID = PROMO_ORDERS.PROMO_ID
                              ORDER BY PHONE.ID
                         """;
                    using (var command3 = new SqlCommand(sql3, connection))
                    {
                        command3.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader3 = command.ExecuteReader();
                        while (reader3.Read())
                        {
                            Promo_List.Add((string)reader3["PROMO_NAME"]);
                        }
                    }
                    orders.Add(new ORDER()
                    {
                        OrderID = OrderID,
                        CustomerName = FullName,
                        OrderDate = OrderDate,
                        OrderedPhone = OrderedPhones,
                        Promo_List = Promo_List
                    });
                }
            }

            var result = new Tuple<BindingList<ORDER>, int, int>(orders, totalItems, totalPages);
            return result;
        }
        static public Tuple<BindingList<ORDER>, int, int> GetAllThisMonthPaging(SqlConnection connection, int page, int rowsPerPage)
        {
            int totalItems = -1;
            int totalPages = -1;
            var orders = new BindingList<ORDER>();
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
                    DateOnly OrderDate = (DateOnly)reader["CREATED_DATE"];
                    string sql2 = """
                              select PHONE.NAME, ORDERS_PHONE.PHONE_COUNT from ORDERS, ORDERS_PHONE
                              WHERE ORDERS_PHONE.ORDER_ID = @Id AND PHONE.ID = ORDERS_PHONE.PHONE_ID
                              ORDER BY PHONE.ID
                         """;
                    BindingList<ORDEREDPHONE> OrderedPhones = [];
                    using (var command2 = new SqlCommand(sql2, connection))
                    {
                        command2.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader2 = command.ExecuteReader();
                        while (reader2.Read())
                        {
                            OrderedPhones.Add(new ORDEREDPHONE()
                            {
                                boughtPhone = (string)reader2["NAME"],
                                quantity = (int)reader2["PHONE_COUNT"]
                            });
                        }
                    }

                    BindingList<String> Promo_List = [];
                    string sql3 = """
                              select PROMOTIONS.PROMO_NAME from PROMOTIONS, PROMO_ORDERS
                              WHERE PROMO_ORDERS.ORDER_ID = @Id AND PROMOTIONS.PROMO_ID = PROMO_ORDERS.PROMO_ID
                              ORDER BY PHONE.ID
                         """;
                    using (var command3 = new SqlCommand(sql3, connection))
                    {
                        command3.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader3 = command.ExecuteReader();
                        while (reader3.Read())
                        {
                            Promo_List.Add((string)reader3["PROMO_NAME"]);
                        }
                    }
                    orders.Add(new ORDER()
                    {
                        OrderID = OrderID,
                        CustomerName = FullName,
                        OrderDate = OrderDate,
                        OrderedPhone = OrderedPhones,
                        Promo_List = Promo_List
                    });
                }
            }

            var result = new Tuple<BindingList<ORDER>, int, int>(orders, totalItems, totalPages);
            return result;
        }
        static public Tuple<BindingList<ORDER>, int, int> GetKeyWordPaging(SqlConnection connection, int page, int rowsPerPage, string keyword)
        {
            int totalItems = -1;
            int totalPages = -1;
            var orders = new BindingList<ORDER>();
            int skip = (page - 1) * 10;
            int take = rowsPerPage;
            string sql = """
                              select ORDERS.ORDER_ID, CUSTOMER.FIRSTNAME, CUSTOMER.LASTNAME, ORDERS.CREATED_DATE, ORDERS.TOTAL, ORDERS.STATUS FROM ORDERS, CUSTOMER
                              WHERE ORDERS.CUSTOMER_ID = CUSTOMER.CUS_ID AND (CUSTOMER.FIRSTNAME LIKE '%' + @keyword + '%' OR CUSTOMER.LASTNAME LIKE '%' + @keyword + '%')
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
                command.Parameters.Add("@keyword", SqlDbType.Text).Value = keyword;
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
                    DateOnly OrderDate = (DateOnly)reader["CREATED_DATE"];
                    string sql2 = """
                              select PHONE.NAME, ORDERS_PHONE.PHONE_COUNT from ORDERS, ORDERS_PHONE
                              WHERE ORDERS_PHONE.ORDER_ID = @Id AND PHONE.ID = ORDERS_PHONE.PHONE_ID
                              ORDER BY PHONE.ID
                         """;
                    BindingList<ORDEREDPHONE> OrderedPhones = [];
                    using (var command2 = new SqlCommand(sql2, connection))
                    {
                        command2.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader2 = command.ExecuteReader();
                        while (reader2.Read())
                        {
                            OrderedPhones.Add(new ORDEREDPHONE()
                            {
                                boughtPhone = (string)reader2["NAME"],
                                quantity = (int)reader2["PHONE_COUNT"]
                            });
                        }
                    }

                    BindingList<String> Promo_List = [];
                    string sql3 = """
                              select PROMOTIONS.PROMO_NAME from PROMOTIONS, PROMO_ORDERS
                              WHERE PROMO_ORDERS.ORDER_ID = @Id AND PROMOTIONS.PROMO_ID = PROMO_ORDERS.PROMO_ID
                              ORDER BY PHONE.ID
                         """;
                    using (var command3 = new SqlCommand(sql3, connection))
                    {
                        command3.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader3 = command.ExecuteReader();
                        while (reader3.Read())
                        {
                            Promo_List.Add((string)reader3["PROMO_NAME"]);
                        }
                    }
                    orders.Add(new ORDER()
                    {
                        OrderID = OrderID,
                        CustomerName = FullName,
                        OrderDate = OrderDate,
                        OrderedPhone = OrderedPhones,
                        Promo_List = Promo_List
                    });
                }
            }

            var result = new Tuple<BindingList<ORDER>, int, int>(orders, totalItems, totalPages);
            return result;
        }
        static public Tuple<BindingList<ORDER>, int, int> GetKeyWordTodayPaging(SqlConnection connection, int page, int rowsPerPage, string keyword)
        {
            int totalItems = -1;
            int totalPages = -1;
            var orders = new BindingList<ORDER>();
            int skip = (page - 1) * 10;
            int take = rowsPerPage;
            string sql = """
                              select ORDERS.ORDER_ID, CUSTOMER.FIRSTNAME, CUSTOMER.LASTNAME, ORDERS.CREATED_DATE, ORDERS.TOTAL, ORDERS.STATUS FROM ORDERS, CUSTOMER
                              WHERE ORDERS.CUSTOMER_ID = CUSTOMER.CUS_ID AND CONVERT(DATE, ORDERS.CREATED_DATE) = @Today 
                              AND (CUSTOMER.FIRSTNAME LIKE '%' + @keyword + '%' OR CUSTOMER.LASTNAME LIKE '%' + @keyword + '%')
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
                command.Parameters.Add("@keyword", SqlDbType.Text).Value = keyword;
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
                    DateOnly OrderDate = (DateOnly)reader["CREATED_DATE"];
                    string sql2 = """
                              select PHONE.NAME, ORDERS_PHONE.PHONE_COUNT from ORDERS, ORDERS_PHONE
                              WHERE ORDERS_PHONE.ORDER_ID = @Id AND PHONE.ID = ORDERS_PHONE.PHONE_ID
                              ORDER BY PHONE.ID
                         """;
                    BindingList<ORDEREDPHONE> OrderedPhones = [];
                    using (var command2 = new SqlCommand(sql2, connection))
                    {
                        command2.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader2 = command.ExecuteReader();
                        while (reader2.Read())
                        {
                            OrderedPhones.Add(new ORDEREDPHONE()
                            {
                                boughtPhone = (string)reader2["NAME"],
                                quantity = (int)reader2["PHONE_COUNT"]
                            });
                        }
                    }

                    BindingList<String> Promo_List = [];
                    string sql3 = """
                              select PROMOTIONS.PROMO_NAME from PROMOTIONS, PROMO_ORDERS
                              WHERE PROMO_ORDERS.ORDER_ID = @Id AND PROMOTIONS.PROMO_ID = PROMO_ORDERS.PROMO_ID
                              ORDER BY PHONE.ID
                         """;
                    using (var command3 = new SqlCommand(sql3, connection))
                    {
                        command3.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader3 = command.ExecuteReader();
                        while (reader3.Read())
                        {
                            Promo_List.Add((string)reader3["PROMO_NAME"]);
                        }
                    }
                    orders.Add(new ORDER()
                    {
                        OrderID = OrderID,
                        CustomerName = FullName,
                        OrderDate = OrderDate,
                        OrderedPhone = OrderedPhones,
                        Promo_List = Promo_List
                    });
                }
            }

            var result = new Tuple<BindingList<ORDER>, int, int>(orders, totalItems, totalPages);
            return result;
        }
        static public Tuple<BindingList<ORDER>, int, int> GetKeyWordWeekPaging(SqlConnection connection, int page, int rowsPerPage, string keyword)
        {
            int totalItems = -1;
            int totalPages = -1;
            var orders = new BindingList<ORDER>();
            int skip = (page - 1) * 10;
            int take = rowsPerPage;
            string sql = """
                              select ORDERS.ORDER_ID, CUSTOMER.FIRSTNAME, CUSTOMER.LASTNAME, ORDERS.CREATED_DATE, ORDERS.TOTAL, ORDERS.STATUS FROM ORDERS, CUSTOMER
                              WHERE ORDERS.CUSTOMER_ID = CUSTOMER.CUS_ID AND DATEPART(ISO_WEEK, ORDERS.CREATED_DATE) = DATEPART(ISO_WEEK, @Today) 
                              AND YEAR(ORDERS.CREATED_DATE) = YEAR(@Today)
                              AND (CUSTOMER.FIRSTNAME LIKE '%' + @keyword + '%' OR CUSTOMER.LASTNAME LIKE '%' + @keyword + '%')
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
                    DateOnly OrderDate = (DateOnly)reader["CREATED_DATE"];
                    string sql2 = """
                              select PHONE.NAME, ORDERS_PHONE.PHONE_COUNT from ORDERS, ORDERS_PHONE
                              WHERE ORDERS_PHONE.ORDER_ID = @Id AND PHONE.ID = ORDERS_PHONE.PHONE_ID
                              ORDER BY PHONE.ID
                         """;
                    BindingList<ORDEREDPHONE> OrderedPhones = [];
                    using (var command2 = new SqlCommand(sql2, connection))
                    {
                        command2.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader2 = command.ExecuteReader();
                        while (reader2.Read())
                        {
                            OrderedPhones.Add(new ORDEREDPHONE()
                            {
                                boughtPhone = (string)reader2["NAME"],
                                quantity = (int)reader2["PHONE_COUNT"]
                            });
                        }
                    }

                    BindingList<String> Promo_List = [];
                    string sql3 = """
                              select PROMOTIONS.PROMO_NAME from PROMOTIONS, PROMO_ORDERS
                              WHERE PROMO_ORDERS.ORDER_ID = @Id AND PROMOTIONS.PROMO_ID = PROMO_ORDERS.PROMO_ID
                              ORDER BY PHONE.ID
                         """;
                    using (var command3 = new SqlCommand(sql3, connection))
                    {
                        command3.Parameters.Add("@Id", SqlDbType.Int).Value = OrderID;
                        var reader3 = command.ExecuteReader();
                        while (reader3.Read())
                        {
                            Promo_List.Add((string)reader3["PROMO_NAME"]);
                        }
                    }
                    orders.Add(new ORDER()
                    {
                        OrderID = OrderID,
                        CustomerName = FullName,
                        OrderDate = OrderDate,
                        OrderedPhone = OrderedPhones,
                        Promo_List = Promo_List
                    });
                }
            }

            var result = new Tuple<BindingList<ORDER>, int, int>(orders, totalItems, totalPages);
            return result;
        }
    }
}
