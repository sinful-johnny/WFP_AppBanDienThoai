using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4
{
    public class CUSTOMER : INotifyPropertyChanged
    {
        public string Cus_ID {  get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string PhoneNum { get; set; }
        public DateTime DOB { get; set; }
        public string Pfp { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
