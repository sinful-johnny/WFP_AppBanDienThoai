﻿using HW4.DAO;
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
        static public Tuple<BindingList<CUSTOMER>, int, int> GetCustomerByPaging(SqlConnection connection, int page, int rowsPerPage)
        {
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
    }
}