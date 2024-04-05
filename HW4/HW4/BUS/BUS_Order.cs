using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Xps.Packaging;

namespace HW4.BUS
{
    internal class BUS_Order
    {
        static public Tuple<BindingList<ORDER>, int, int> SortOrder(SqlConnection conn, int page, int rowsPerPage, string sortCriteria)
        {
            if (sortCriteria == "This Week")
            {
                return ORDERControl.GetAllThisWeekPaging(conn, page, rowsPerPage);
            }

            if (sortCriteria == "Today")
            {
                return ORDERControl.GetAllTodayPaging(conn, page, rowsPerPage); 
            }

            if (sortCriteria == "This Month")
            {
                return ORDERControl.GetAllThisMonthPaging(conn, page, rowsPerPage);
            }

            return ORDERControl.GetAllPaging(conn, page, rowsPerPage);
        }

        static public Tuple<BindingList<ORDER>, int, int> getInRange(SqlConnection conn, int page, int rowsPerPage, DateTime begin, DateTime end)
        {
            return ORDERControl.FromDateToDate(conn, page, rowsPerPage, begin, end);
        }

        static public int NewOrder(SqlConnection conn, int CustomerID, BindingList<ORDEREDPHONE> OrderedPhone)
        {
            return ORDERControl.AddOrder(conn, CustomerID, OrderedPhone);
        }

        static public bool DeleteOrder(SqlConnection conn, int OrderID)
        {
            return ORDERControl.DeleteOrder(conn, OrderID);
        }

        static public bool CancelOrder(SqlConnection conn, int OrderID)
        {
            return ORDERControl.UpDateStatus(conn, "Cancelled", OrderID);
        }

        static public bool DeliverOrder(SqlConnection conn, int OrderID)
        {
            return ORDERControl.UpDateStatus(conn, "Delivered", OrderID);
        }

        static public bool EditPhoneInOrder(SqlConnection conn, ORDEREDPHONE OrderedPhone, int OrderID, string command)
        {
            if (command == "Add New Phone") 
            {
                return ORDERControl.NewPhoneInOrder(conn, OrderedPhone, OrderID);
            }

            if (command == "Delete Phone")
            {
                return ORDERControl.DeletePhoneInOrder(conn, OrderedPhone.PhoneID, OrderID);
            }

            return ORDERControl.ChangePhoneCount(conn, OrderedPhone, OrderID);
        }
    }
}
