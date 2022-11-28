using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DataModels
{
    public class ServiceStation
    {

        public ServiceStation()
        {
            Customers = new Queue<Customer>();
        }
        public int Id { get; set; }
        public Queue<Customer>? Customers { get; set; }
    }
}
