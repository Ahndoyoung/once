using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Once_v2_2015.Model
{
    public class MenuItem : ViewModelBase
    {
        public string name { get; set; }
        public char? temperature { get; set; }
        public char? size { get; set; }
        public int price { get; set; }
        public bool isWhipping { get; set; }
    }

    public class MenuItemProto
    {
        public string name { get; set; }
        public bool temp { get; set; }
        public bool size { get; set; }
        public bool whip { get; set; }
        public int price { get; set; }
        public int priceL { get; set; }
    }
}
