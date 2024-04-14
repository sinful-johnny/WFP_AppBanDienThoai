
using System.Data;

namespace IDaoChart
{
    public interface IDao
    {
        public string stringQuery { get; set; }
        DataTable getAll(DateTime begin, DateTime end);

    }

}
