using Models.DataModels;

namespace BuisnessProjectAPI.BuisnessLogic
{
    public class ChooseQueue
    {
        int index = 0;
        public int GetQueueIndex(IList<ServiceStation> queues)
        {
            if (queues.Count - 1 == index)
            {
                index = 0;
                return index;
            }
            index++;
            return index;
        }
    }
}
