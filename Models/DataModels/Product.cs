using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DataModels
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public IList<Material>? Materials { get; set; }
        public int DeliveryTime { get; set; }
        public int Priority { get; set; }
        public string? Image { get; set; }
        public int Price { get; set; }
        public bool IsDone { get; set; } = false;
    }
}
