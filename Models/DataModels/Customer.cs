using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DataModels
{
    public class Customer
    {
        public int Id { get; set; }
        public int OrderingTime { get; set; }
        public Order? Order { get; set; }
    }
}
