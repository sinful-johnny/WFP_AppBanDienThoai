using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HW4
{
    internal class PHONEControl
    {
        private string _ConnectionString { get; set; }
        public PHONEControl(string Username,string Password) {
            _ConnectionString = $"""Server=DESKTOP-CDH2DEU\SQLSERVER;Database=QLDTHOAI;User ID= sa; Password= 123; TrustServerCertificate=True""";
        }

        public BindingList<PHONE> GetPHONEsByManufacturer(string manufacturer)
        {

            using (var connection = new SqlConnection(_ConnectionString))
            {

                string sql = $"select * from PHONE where MANUFACTURER=@Manufacturer";
                connection.Open();
                var command = new SqlCommand(sql, connection);
                command.Parameters.Add("@Manufacturer", SqlDbType.Text).Value = manufacturer;
                var reader = command.ExecuteReader();

                var phones = new BindingList<PHONE>();
                while (reader.Read())
                {
                    phones.Add(new PHONE()
                    {
                        ID = (int)reader["ID"],
                        PhoneName = (string)reader["NAME"],
                        Manufacturer = (string)reader["MANUFACTURER"],
                        Thumbnail = (string)reader["THUMBNAIL"],
                        Price = (int)reader["PRICE"]
                    });
                }
                return phones;
            }     
        }

        public BindingList<PHONE> GetPHONEs()
        {
            using (var connection = new SqlConnection(_ConnectionString))
            {

                string sql = $"select PHONE.ID,PHONE.NAME,M.NAME as MANUFACTURER,PHONE.THUMBNAIL,PHONE.PRICE from PHONE, MANUFACTURER as M where PHONE.MANUFACTURER_ID = M.ID";
                connection.Open();
                var command = new SqlCommand(sql, connection);
                var reader = command.ExecuteReader();

                var phones = new BindingList<PHONE>();
                while (reader.Read())
                {
                    phones.Add(new PHONE()
                    {
                        ID = (int)reader["ID"],
                        PhoneName = (string)reader["NAME"],
                        Manufacturer = (string)reader["MANUFACTURER"],
                        Thumbnail = (string)reader["THUMBNAIL"],
                        Price = (double)reader["PRICE"]
                    });
                    
                }
                return phones;
            }
        }


    }
}
