using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4.DTO
{
    public class INCOMECHART : INotifyPropertyChanged, ICloneable
    {
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }
        public double Profit { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
