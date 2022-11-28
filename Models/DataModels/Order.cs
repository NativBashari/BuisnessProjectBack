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

    }
}
