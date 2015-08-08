using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Once_v2_2015.Model
{
    public class Category
    {
        public string name { get; set; }
        public List<MenuItem> menuList { get; set; }

        public Category()
        {
            menuList = new List<MenuItem>();
        }
    }
}
