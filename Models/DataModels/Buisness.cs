using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Models.DataModels
{
    public class Buisness
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        //public IList<ServiceStation>? Queues { get; set; }
        public int ServiceStations { get; set; }
        public IList<Product>? Products { get; set; }
        public int ProductionSlots { get; set; }
    }
}
