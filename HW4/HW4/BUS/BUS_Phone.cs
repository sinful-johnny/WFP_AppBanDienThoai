using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4.BUS
{
    class BUS_Phone
    {
        static public Tuple<BindingList<PHONE>, int, int> GetPHONEs(SqlConnection connection, int page, int rowsPerPage, string keyword, string Manufacturer)
        {
            if((keyword != null && keyword != "" && keyword != " ") && (Manufacturer == null || Manufacturer == "" || Manufacturer == " "))
            {
                return PHONEControl.GetAllPagingWithKeyword(connection,page,rowsPerPage,keyword);
            } 
            else if((keyword != null && keyword != "" && keyword != " ") && (Manufacturer != null && Manufacturer != "" && Manufacturer != " "))
            {
                return PHONEControl.GetAllPagingWithKeywordAndManufacturer(connection,page,rowsPerPage,keyword,Manufacturer);
            }
            else if ((keyword == null || keyword == "" || keyword == " ") && (Manufacturer != null && Manufacturer != "" && Manufacturer != " "))
            {
                return PHONEControl.GetAllPagingWithManufacturer(connection,page,rowsPerPage,Manufacturer);
            }
            else
            {
                return PHONEControl.GetAllPaging(connection, page, rowsPerPage);
            }
        }
        static public bool delete(SqlConnection connection, int ID)
        {
            return PHONEControl.DeletePHONE(connection, ID);
        }

        static public int insert(SqlConnection connection, string name, int ManufacturerID, string Thumbnail, double Price)
        {
            return PHONEControl.InsertPHONE(connection, name, ManufacturerID, Thumbnail, Price);
        }
        static public bool update(SqlConnection connection, int ID, string name, int ManufacturerID, string Thumbnail, double Price)
        {
            return PHONEControl.UpdatePHONE(connection,ID, name, ManufacturerID, Thumbnail,Price);
        }
    }
}
