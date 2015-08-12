using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Once_v2_2015.Model
{
    public class SellingItem : MenuItem
    {
        public string content { get; set; }

        private int _quantity;
        public int quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                RaisePropertyChanged("quantity");

                total = quantity * price;
            }
        }

        private int _total;
        public int total
        {
            get { return _total; }
            set
            {
                _total = value;
                RaisePropertyChanged("total");
            }
        }

        public SellingItem(string _name, char? _temp, char? _size, int _price)
        {
            name = _name;
            temperature = _temp;
            size = _size;
            price = _price;
            quantity = 1;
            total = price * quantity;

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

            if (name != " * 샷" && name != " * 시럽" && name != " * 휘핑크림" && name != " * 드리즐")
                content = name.Replace('^', ' ') + "\n -" + temp + "\n -" + s;
            else
                content = name;
        }
    }
}
