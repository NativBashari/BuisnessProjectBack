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
        public string? Name { get; set; }
        public int ServiceStations { get; set; }
        public IList<Material>? Materials{ get; set; }
        public int ProductionSlots { get; set; }
        public int DelieveryTime { get; set; }
        public double CustomersEntryRate { get; set; }
    }
}
