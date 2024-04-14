using IBusChart;
using System.Windows.Controls;

namespace IGuiChart
{
    public interface IGui
    {
        public string description { get; }
        void setBus(IBus bus);
        UserControl display { get; }
    }

}
