using Models.DataModels;

namespace Simulator
{
    public class OrdersGenerator
    {
        int counter;
        public Order GenerateOrder( IList<Product> products)
        {
            var seconds = 0;
            foreach(var product in products)
            {
                seconds += product.DeliveryTime;
            }
            return new Order { Id = counter += 1 , EstimatedTime = DateTime.Now.AddSeconds(seconds), Products= products};
        }
    }
}