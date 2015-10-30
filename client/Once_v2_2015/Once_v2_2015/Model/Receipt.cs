using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Once_v2_2015.Model
{
    public class Receipt
    {
        public int daily_num { get; set; }
        public int num { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string type { get; set; }
        public int discount { get; set; }
        public int subtotal { get; set; }
        public int amount { get; set; }
    }
}
