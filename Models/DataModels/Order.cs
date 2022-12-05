using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DataModels
{
    public class Order
    {
        public int Id { get; set; }
        public IList<Product>? Products { get; set; }
        public DateTime EstimatedTime { get; set; }
        public bool IsFailed { get; set; }
        public bool DelieveryFailed { get; set; }
        public int Price { get; set; }
        public bool IsDelievery { get; set; }


    }
}
