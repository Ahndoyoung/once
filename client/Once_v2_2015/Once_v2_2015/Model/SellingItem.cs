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

        public SellingItem(string _name, char? _temp, char? _size, int _price, int _qty)
        {
            name = _name;
            temperature = _temp;
            size = _size;
            price = _price;
            quantity = _qty;
            total = price * quantity;

            string temp = null;
            if (temperature == 'I')
                temp = "Ice";
            else if (temperature == 'H')
                temp = "Hot";
            else
                temp = "None";

            string s = null;
            if (size == 'R')
                s = "Regular";
            else if (size == 'L')
                s = "Large";
            else
                s = "None";

            if (name != " * 샷" && name != " * 시럽" && name != " * 휘핑크림" && name != " * 드리즐" && name != "# Discount")
                content = name.Replace('^', ' ') + "\n - " + temp + "\n - " + s;
            else
                content = name;
        }
    }
}
