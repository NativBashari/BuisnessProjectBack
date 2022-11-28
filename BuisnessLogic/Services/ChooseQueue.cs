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
        int index = 0;
        public int GetQueueIndex(IList<ServiceStation> queues)
        {
            if(queues.Count - 1 == index)
            {
                index = 0;
                return index;
            }
            index++;
            return index;
        }
    }
}
