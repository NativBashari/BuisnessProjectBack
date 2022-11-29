using Models.DataModels;

namespace BuisnessProjectAPI.BuisnessLogic
{
    public class DelieveryHandling
    {

        Action<Order> RemoveOrderFromDelievery;
        private readonly int buisnessDelieveryTime;

        public DelieveryHandling(Action<Order> RemoveOrderFromDelievery, int buisnessDelieveryTime)
        {
            this.RemoveOrderFromDelievery = RemoveOrderFromDelievery;
            this.buisnessDelieveryTime = buisnessDelieveryTime;
        }


        public async Task DelieveryHandler(Order order)
        {
            await Task.Delay(buisnessDelieveryTime * 1000);
            RemoveOrderFromDelievery.Invoke(order);
        }
    }
}
