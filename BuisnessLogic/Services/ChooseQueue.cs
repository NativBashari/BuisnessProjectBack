using Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.Services
{
    internal  class ChooseQueue
    {
        Random r = new Random();
        public  int GetQueueIndex(IList<ServiceStation> queues)
        {
            var index = r.Next(0, queues.Count);
            return index;
        }
    }
}
