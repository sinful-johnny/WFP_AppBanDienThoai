using HW4.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4
{
    public class ORDER
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate {  get; set; }
        public BindingList<ORDEREDPHONE> OrderedPhone { get; set; }
        public BindingList<PROMOTIONSINORDER> PromoList { get; set; }
        public float TotalPrice { get; set; }
        public string status { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
