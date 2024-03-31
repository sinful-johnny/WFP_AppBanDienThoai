using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4
{
    internal class PHONEORDER
    {
        public string OrderID { get; set; }
        public string CustomerName { get; set; }
        public DateOnly OrderDate {  get; set; }
        public Tuple<BindingList<PHONE>, int> OrderedPhone { get; set; }
        public float totalPrice { get; set; }
        public string status { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
