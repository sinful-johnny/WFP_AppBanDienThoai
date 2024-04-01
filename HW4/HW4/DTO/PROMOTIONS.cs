using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4.DTO
{
    internal class PROMOTIONS
    {
        public int PromoID {  get; set; }
        public string PromoName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public MANUFACTURER Manufacturer { get; set; }
        public PHONE AssignedPhone { get; set; }
        public float Discount { get; set; }
        public string Status { get; set; }
    }
}
