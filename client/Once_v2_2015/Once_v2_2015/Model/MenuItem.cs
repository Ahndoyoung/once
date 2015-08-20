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

    public class MenuItemProto : ViewModelBase
    {
        private string _name;

        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("name");
            }
        }


        private bool _temp;

        public bool temp
        {
            get { return _temp; }
            set
            {
                _temp = value;
                RaisePropertyChanged("temp");
            }
        }

        private bool _size;

        public bool size
        {
            get { return _size; }
            set
            {
                _size = value;
                RaisePropertyChanged("size");
            }
        }

        private bool _whip;

        public bool whip
        {
            get { return _whip; }
            set
            {
                _whip = value;
                RaisePropertyChanged("whip");
            }
        }

        private int _price;

        public int price
        {
            get { return _price; }
            set
            {
                _price = value;
                RaisePropertyChanged("price");
            }
        }

        private int _priceL;

        public int priceL
        {
            get { return _priceL; }
            set
            {
                _priceL = value;
                RaisePropertyChanged("priceL");
            }
        }
    }
}
