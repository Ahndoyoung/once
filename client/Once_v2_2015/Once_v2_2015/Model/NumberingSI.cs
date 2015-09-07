using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Once_v2_2015.Model
{
    public class NumberingSI
    {
        public int id { get; set; }
        public ObservableCollection<SellingItem> sellingItems = null;

        public NumberingSI(int _id, ObservableCollection<SellingItem> _si)
        {
            id = _id;
            sellingItems = _si;
        }
    }
}
