using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4.BUS
{
    internal class BUS_Promotions
    {
        static public Tuple<DataTable, int, int> GetAllPaging(SqlConnection connection, int page, int rowsPerPage)
        {
            return PROMOTIONS_Control.GetAllPaging(connection, page, rowsPerPage);
        }
        static public Tuple<DataTable, int, int> getOpenPromos(SqlConnection connection, int page, int rowsPerPage)
        {
            return PROMOTIONS_Control.getOpenPromos(connection, page, rowsPerPage);
        }

        static public Tuple<DataTable, int, int> getPromosWithKeyword(SqlConnection connection, int page, int rowsPerPage,string Keyword)
        {
            if(Keyword != null && Keyword != "" && Keyword != " ")
            {
                return PROMOTIONS_Control.getPromosWithKeyword(connection, page, rowsPerPage, Keyword);
            }
            else
            {
                return PROMOTIONS_Control.GetAllPaging(connection,page, rowsPerPage);
            }
        }

        static public bool updatePromotion(SqlConnection connection, int ID, string PromoName, DateTime StartDate, DateTime EndDate, int ManufacturerID, int PhoneID, double Discount, string Status)
        {
            return PROMOTIONS_Control.Update(connection, ID, PromoName, StartDate, EndDate, ManufacturerID, PhoneID, Discount, Status);
        }
        static public int insertPromotion(SqlConnection connection, int ID, string PromoName, DateTime StartDate, DateTime EndDate, int ManufacturerID, int PhoneID, double Discount, string Status)
        {
            return PROMOTIONS_Control.Insert(connection, ID, PromoName, StartDate, EndDate, ManufacturerID, PhoneID, Discount, Status);
        }
        static public bool deletePromotion(SqlConnection connection, int ID)
        {
            return PROMOTIONS_Control.Delete(connection, ID);
        }
    }
}
