using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4
{
    public class ORDEREDPHONE : INotifyPropertyChanged
    {
        public int PhoneID { get; set; }
        public string PhoneName {  get; set; }
        public float Price {  get; set; }
        public int quantity { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
