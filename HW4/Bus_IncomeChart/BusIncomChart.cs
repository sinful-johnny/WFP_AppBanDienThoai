using IBusChart;
using IDaoChart;
using System.Data;
using System.Data.SqlClient;

namespace Bus_IncomeChart
{
    public class BusIncomChart:IBus
    {
        private SqlConnection _connection;
        public BusIncomChart(SqlConnection connection)
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

        public Tuple<double[], double[], List<string>> takeIncomeProfitDateStringWithDay(SqlConnection connection, DateTime beginDate, DateTime endDate)
        {
            //List<double> profits;
            var incomechartlists = _IDao.getAll(beginDate, endDate);

            TimeSpan rangeTimeSpan = endDate.Subtract(beginDate); //declared prior as TimeSpan object
            int DaysSpan = rangeTimeSpan.Days + 1;
            double[] incomeEachDay = new double[DaysSpan];
            
            double[] profitEachDay = new double[DaysSpan];

            DateTime[] timeRange = new DateTime[DaysSpan];

            foreach (DataRow row in incomechartlists.Rows)
            {
                DateTime OrderedDate = (DateTime)row["CREATED_DATE"];
                TimeSpan rangeTime = OrderedDate.Subtract(beginDate);
                int Day = rangeTime.Days;
                incomeEachDay[Day] += (double)row["TOTAL"];
                profitEachDay[Day] += (double)row["PROFIT"];
            }

            //Store a DateTimes String 
            var datetimeString = new List<string>();
            for (int Day = 0; Day < DaysSpan; Day++)
            {
                timeRange[Day] = beginDate;
                datetimeString.Add(timeRange[Day].ToString("dd/MM/yyyy"));
                beginDate = beginDate.AddDays(1);
            }

            var res = new Tuple<double[], double[], List<string>>(incomeEachDay, profitEachDay, datetimeString);

            return res;
        }

        public Tuple<double[], double[], List<string>> takeIncomeProfitDateStringWithMonth(SqlConnection connection, DateTime beginDate, DateTime endDate)
        {
            //List<double> profits;
            var incomechartlists = _IDao.getAll(beginDate, endDate);

            int rangeMonthSpan = (endDate.Month - beginDate.Month)
                                 + 12 * (endDate.Year - beginDate.Year) + 1;

            double[] incomeEachDay = new double[rangeMonthSpan];

            double[] profitEachDay = new double[rangeMonthSpan];

            DateTime[] MonthRange = new DateTime[rangeMonthSpan];

            foreach (DataRow row in incomechartlists.Rows)
            {
                DateTime OrderedDate = (DateTime)row["CREATED_DATE"];
                int Month = (OrderedDate.Month - beginDate.Month)
                            + 12 * (OrderedDate.Year - beginDate.Year);
                incomeEachDay[Month] += (double)row["TOTAL"];
                profitEachDay[Month] += (double)row["PROFIT"];
            }

            //Store a DateTimes String 
            var datetimeMonthString = new List<string>();

            for (int Month = 0; Month < rangeMonthSpan; Month++)
            {
                MonthRange[Month] = beginDate;
                datetimeMonthString.Add(MonthRange[Month].ToString("MM/yyyy"));
                beginDate = beginDate.AddMonths(1);
            }

            var res = new Tuple<double[], double[], List<string>>(incomeEachDay, profitEachDay, datetimeMonthString);

            return res;
        }

        public Tuple<double[], double[], List<string>> takeIncomeProfitDateStringWithYear(SqlConnection connection, DateTime beginDate, DateTime endDate)
        {
            //List<double> profits;
            var incomechartlists = _IDao.getAll(beginDate, endDate);

            int rangeYearSpan = endDate.Year - beginDate.Year + 1;

            double[] incomeEachDay = new double[rangeYearSpan];

            double[] profitEachDay = new double[rangeYearSpan];

            DateTime[] YearRange = new DateTime[rangeYearSpan];

            foreach (DataRow row in incomechartlists.Rows)
            {
                DateTime OrderedDate = (DateTime)row["CREATED_DATE"];
                int Year= OrderedDate.Year - beginDate.Year;

                incomeEachDay[Year] += (double)row["TOTAL"];
                profitEachDay[Year] += (double)row["PROFIT"];
            }

            //Store a DateTimes String 
            var datetimeYearString = new List<string>();

            for (int Year = 0; Year < rangeYearSpan; Year++)
            {
                YearRange[Year] = beginDate;
                datetimeYearString.Add(YearRange[Year].ToString("yyyy"));
                beginDate = beginDate.AddMonths(1);
            }

            var res = new Tuple<double[], double[], List<string>>(incomeEachDay, profitEachDay, datetimeYearString);

            return res;
        }
    }

}
