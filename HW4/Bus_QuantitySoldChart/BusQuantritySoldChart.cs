using IBusChart;
using IDaoChart;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace Bus_QuantitySoldChart
{
    public class BusQuantritySoldChart:IBus
    {
        private SqlConnection _connection;
        public BusQuantritySoldChart(SqlConnection connection)
        {
            _connection = connection;
        }

        private IDao _IDao;
        public string stringQuery { get; set; }
        public void SetDao(IDao dao)
        {
            _IDao = dao;
            _IDao.stringQuery = stringQuery;
        }
        public IDao GetDao()
        {
            return _IDao;
        }

        public Tuple<int[], List<string>> takequantitysoldphonename(DateTime beginDate, DateTime endDate)
        {
            var quantitysoldchartlists = _IDao.getAll(beginDate, endDate);

            int numberOfPhone = quantitysoldchartlists.Rows.Count;

            int[] quantitysold = new int[numberOfPhone];

            var phoneName = new List<string>();

            int index = 0;

            foreach (DataRow row in quantitysoldchartlists.Rows)
            {
                quantitysold[index] = (int)row["QUANTITY_SOLD"];
                phoneName.Add((string)row["NAME"]);
                index++;
            }

            var res = new Tuple<int[], List<string>>(quantitysold, phoneName);

            return res;
        }

    }

}
