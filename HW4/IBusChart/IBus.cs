using IDaoChart;
using System.Windows.Controls;

namespace IBusChart
{
    public interface IBus
    {
        public string stringQuery { get; set; }
        void SetDao(IDao dao);
        IDao GetDao();
    }

}
