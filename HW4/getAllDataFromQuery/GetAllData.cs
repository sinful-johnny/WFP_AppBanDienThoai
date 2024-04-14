using IDaoChart;
using System.Data;
using System.Data.SqlClient;

namespace getAllDataFromQuery
{
    public class GetAllData : IDao
    {
        private SqlConnection _connection;

        GetAllData(SqlConnection connection)
        {
            _connection = connection;
        }
        public string stringQuery { get; set; }
        public DataTable getAll(DateTime begin, DateTime end)
        {
            var connection = _connection;
            string sqlquery = stringQuery;

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            DataTable dataTable = new DataTable();

            using (var command = new SqlCommand(sqlquery, connection))
            {
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = begin;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = end;
                //_connection.Open();
                var reader = command.ExecuteReader();

                dataTable.Load(reader);
                reader.Close();
            }
            connection.Close();

            var res = dataTable;
            return res;
        }
    }

}
