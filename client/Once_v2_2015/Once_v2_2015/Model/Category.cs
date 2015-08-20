using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Once_v2_2015.Model
{
    public class Category : ViewModelBase
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

        public List<MenuItem> menuList { get; set; }

        public Category()
        {
            menuList = new List<MenuItem>();
        }
    }

    public class CategoryProto : ViewModelBase
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

        public List<MenuItemProto> menuProtoList { get; set; }

        public CategoryProto()
        {
            menuProtoList = new List<MenuItemProto>();
        }
    }
}
