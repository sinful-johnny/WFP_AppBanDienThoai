using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4
{
    public class PHONE : INotifyPropertyChanged, ICloneable
    {
        public int ID { get; set; }
        public string PhoneName { get; set; }
        public double OriginalPrice { get; set; }
        public double Price { get; set; }
        public string Thumbnail { get; set; }
        public string Manufacturer { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
