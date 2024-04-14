using System.Data;
using System.Windows.Controls;

namespace IGuiChart
{
    public interface IGui
    {
        void SetData(DataTable data);
        UserControl display { get; }
    }

}
