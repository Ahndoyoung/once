using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Once_v2_2015.Model
{
    public class SellingItem : MenuItem
    {
        public string content { get; set; }
        public int quantity { get; set; }
        public int total { get; set; }

        public SellingItem()
        {
            string temp = null;
            if (temperature == 'i')
                temp = "Ice";
            else if (temperature == 'h')
                temp = "Hot";
            else
                temp = "None";

            string s = null;
            if (size == 'r')
                s = "Regular";
            else if (size == 'l')
                s = "Large";
            else
                s = "None";

            content = name + "\n -" + temp + "\n -" + s;
            total = price * quantity;
        }
    }
}
