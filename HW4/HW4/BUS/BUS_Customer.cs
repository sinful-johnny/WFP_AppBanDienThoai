using HW4.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4.BUS
{
    internal class BUS_Customer
    {
        static public Tuple<BindingList<CUSTOMER>, int, int> GetCustomerByPaging(SqlConnection connection, int page, int rowsPerPage, int mode, string keyword)
        {   
            if (mode == 2)
            {
                return CUSTOMERControl.GetAllPagingWithKeyWord(connection, page, rowsPerPage,keyword);
            }
            return CUSTOMERControl.GetAllPaging(connection, page, rowsPerPage);

        }
        static public bool Update(SqlConnection connection, CUSTOMER info)
        {
            return CUSTOMERControl.Update(connection, info);
        }
        static public int Insert(SqlConnection connection, CUSTOMER info)
        {
            return CUSTOMERControl.Insert(connection, info);
        }
        static public bool Delete(SqlConnection connection, int ID)
        {
            return CUSTOMERControl.Delete(connection, ID);
        }
        static public DataTable getCustomerOders(SqlConnection connection, int ID) { 
            return ORDERControl.getCustomerOrder(connection, ID);
        }
        static public DataTable getCustomerPromos(SqlConnection connection, int ID)
        {
            return PROMOTIONS_Control.getCustomerPromos(connection, ID);
        }

        static public int addCustomerPromo(SqlConnection connection, int cusID, int promoID)
        {
            return CUSTOMERControl.InsertCustomerPromo(connection, cusID, promoID);
        }

        static public bool revokeCustomerPromo(SqlConnection connection, int cusID, int promoID)
        {
            return CUSTOMERControl.DeleteCustomerPromo(connection,cusID, promoID);
        }

        static public bool renewCustomerPromo(SqlConnection connection, int cusID, int promoID)
        {
            return CUSTOMERControl.updatePromoUsageStatus(connection,cusID, promoID,"Available");
        }
    }
}
