using Models.DataModels;

namespace BuisnessProjectAPI.BuisnessLogic
{
    public class DelieveryHandling
    {

        Action<Order> RemoveOrderFromDelievery;
        Action<Order> ReproduceFailedDelievery;
        private readonly int buisnessDelieveryTime;
        int counter;

        public DelieveryHandling(Action<Order> RemoveOrderFromDelievery, int buisnessDelieveryTime, Action<Order> reproduceFailedDelievery)
        {
            this.RemoveOrderFromDelievery = RemoveOrderFromDelievery;
            this.buisnessDelieveryTime = buisnessDelieveryTime;
            ReproduceFailedDelievery = reproduceFailedDelievery;
        }


        public async Task DelieveryHandler(Order order)
        {
            counter++;
           if(counter % 8 == 0)
            {
               Console.WriteLine($"Order {order.Id} is failed in delievery !!!!!");
               order.DelieveryFailed = true;
               RemoveOrderFromDelievery.Invoke(order);
               ReproduceFailedDelievery.Invoke(order);
               return;
            }
            await Task.Delay(buisnessDelieveryTime * 1000);
            RemoveOrderFromDelievery.Invoke(order);
        }
    }
}
