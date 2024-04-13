using DocumentFormat.OpenXml.Wordprocessing;
using HW4.DAO;
using HW4.DTO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace HW4.BUS
{
    class BUS_Chart
    {
        static public DateTime NewestOrderDate(SqlConnection connection)
        {
            return CHARTCONTROL.TakeMaxOrderDate(connection);
        }

        static public int AmountProducts(SqlConnection connection, DateTime beginDate, DateTime endDate)
        {
            return CHARTCONTROL.AmountProducts(connection, beginDate, endDate);
        }

        static public int AmountOnSales(SqlConnection connection)
        {
            return CHARTCONTROL.AmountOnSales(connection);
        }

        static public Tuple<double[], double[], List<string>> takeIncomeProfitDateStringWithDay(SqlConnection connection, DateTime beginDate, DateTime endDate)
        {
            //List<double> profits;
            var incomechartlists = new ObservableCollection<INCOMECHART>();
            incomechartlists = CHARTCONTROL.incomeChartDataHandling(connection, beginDate, endDate);

            TimeSpan rangeTimeSpan = endDate.Subtract(beginDate); //declared prior as TimeSpan object
            int DaysSpan = rangeTimeSpan.Days + 1;
            double[] incomeEachDay = new double[DaysSpan];
            double[] profitEachDay = new double[DaysSpan];
            DateTime[] timeRange = new DateTime[DaysSpan];

            foreach (var incomechartlist in incomechartlists)
            {
                TimeSpan rangeTime = incomechartlist.OrderDate.Subtract(beginDate);
                int Day = rangeTime.Days;
                incomeEachDay[Day] += incomechartlist.TotalPrice;
                profitEachDay[Day] += incomechartlist.Profit;
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

        static public Tuple<double[], double[], List<string>> takeIncomeProfitDateStringWithMonth(SqlConnection connection, DateTime beginDate, DateTime endDate)
        {
            var incomechartlists = new ObservableCollection<INCOMECHART>();
            incomechartlists = CHARTCONTROL.incomeChartDataHandling(connection, beginDate, endDate);

            int rangeMonthSpan = (endDate.Month - beginDate.Month)
                                 + 12 * (endDate.Year - beginDate.Year) + 1;
            double[] incomeEachDay = new double[rangeMonthSpan];
            double[] profitEachDay = new double[rangeMonthSpan];
            DateTime[] MonthRange = new DateTime[rangeMonthSpan];

            foreach (var incomechartlist in incomechartlists)
            {
                int Month = (incomechartlist.OrderDate.Month - beginDate.Month)
                            + 12 * (incomechartlist.OrderDate.Year - beginDate.Year);
                incomeEachDay[Month] += incomechartlist.TotalPrice;
                profitEachDay[Month] += incomechartlist.Profit;
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

        static public Tuple<double[], double[], List<string>> takeIncomeProfitDateStringWithYear(SqlConnection connection, DateTime beginDate, DateTime endDate)
        {
            var incomechartlists = new ObservableCollection<INCOMECHART>();
            incomechartlists = CHARTCONTROL.incomeChartDataHandling(connection, beginDate, endDate);

            int rangeYearSpan = endDate.Year - beginDate.Year + 1;
            double[] incomeEachDay = new double[rangeYearSpan];
            double[] profitEachDay = new double[rangeYearSpan];
            DateTime[] YearRange = new DateTime[rangeYearSpan];

            foreach (var incomechartlist in incomechartlists)
            {
                int Year = incomechartlist.OrderDate.Year - beginDate.Year;
                incomeEachDay[Year] += incomechartlist.TotalPrice;
                profitEachDay[Year] += incomechartlist.Profit;
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

        static public Tuple<int[], List<string>> takequantitysoldphonename(SqlConnection connection, DateTime beginDate, DateTime endDate)
        {
            var selected = CHARTCONTROL.quantitysoldChartDataHandling(connection, beginDate, endDate);

            var quantitysoldchartlists = new ObservableCollection<QUANTITYSOLDCHART>();

            quantitysoldchartlists = selected.Item1;

            int numberOfPhone = selected.Item2;

            int[] quantitysold = new int[numberOfPhone];

            var phoneName = new List<string>();

            for (int i = 0; i < numberOfPhone; i++)
            {
                quantitysold[i] = quantitysoldchartlists[i].QuantitySold;
                phoneName.Add(quantitysoldchartlists[i].PhoneName);
            }

            var res = new Tuple<int[], List<string>>(quantitysold, phoneName);

            return res;
        }
    }
}
