using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4.BUS
{
    class BUS_Manufacturer
    {
        static public BindingList<MANUFACTURER> getAllManufacturers(SqlConnection sqlConnection)
        {
            return MANUFACTURERControl.GetMANUFACTURERs(sqlConnection);
        }
        static public BindingList<MANUFACTURER> getAllManufacturersByKeyword(SqlConnection sqlConnection, string keyWord)
        {
            return MANUFACTURERControl.getByKeyWord(sqlConnection, keyWord);
        }
        static public bool deleteManufacturer(SqlConnection sqlConnection, string ManufacturerID)
        {
            return MANUFACTURERControl.delete(sqlConnection, ManufacturerID);
        }
        static public int insertManufacturer(SqlConnection sqlConnection, string ManufacturerName)
        {
            return MANUFACTURERControl.insert(sqlConnection,ManufacturerName);
        }
        static public bool updateManufacturer(SqlConnection connection,string ManufacturerID, string ManufacturerName)
        {
            return MANUFACTURERControl.update(connection, ManufacturerID, ManufacturerName);
        }
    }
}
