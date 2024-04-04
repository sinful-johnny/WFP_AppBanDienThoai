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
    class BUS_Phone
    {
        static public Tuple<BindingList<PHONE>, int, int> GetByPrice(SqlConnection conn, int page, int rowsPerPage, int MinPrice, int MaxPrice)
        {
            if (MaxPrice <= 0 || MinPrice < 0 || MaxPrice <= MinPrice)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (MaxPrice == 0) {
                return PHONEControl.GetAllPagingWithMinPrice(conn, page, rowsPerPage, MinPrice);
            }
            else
            {
                return PHONEControl.GetAllPagingWithMinMaxPrice(conn,page, rowsPerPage, MinPrice, MaxPrice);
            }
        }
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

        static public bool insertMany(SqlConnection connection, DataTable data)
        {
            try
            {
                foreach (DataRow row in data.Rows)
                {
                    string PhoneName = (string)row.ItemArray[0]!;
                    int ManufacturerID = int.Parse((string)row.ItemArray[1]!);
                    string Thumbnail = (string)row.ItemArray[2]!;
                    double Price = double.Parse((string)row.ItemArray[3]!);

                    PHONEControl.InsertPHONE(connection, PhoneName, ManufacturerID, Thumbnail, Price);
                }
            }catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return true;
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
