using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Once_v2_2015.Model
{
    public class OrderItems
    {
        public int orderNum { get; set; }
        public string way { get; set; }
        public int discount { get; set; }
        public int subtotal { get; set; }
        public int amount { get; set; }

        public ObservableCollection<SellingItem> items = null;

        public OrderItems(int _num, string _way, int _dis, int _sub, int _amount, ObservableCollection<SellingItem> _items)
        {
            orderNum = _num;
            way = _way;
            discount = _dis;
            subtotal = _sub;
            amount = _amount;
            items = _items;
        }
    }
}
