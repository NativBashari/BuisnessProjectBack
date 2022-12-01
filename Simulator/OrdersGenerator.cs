using DAL;
using Models.DataModels;

namespace Simulator
{
    public class OrdersGenerator
    {
        int counter;
        List<Product> products;
        public OrdersGenerator(DataService dataService)
        {
            products = dataService.GenerateProducts();
        }
        public Order GenerateOrder()
        {
            var seconds = 0;
            var productsCopy = new List<Product>(products);
            foreach (var product in productsCopy)
            {
                seconds += product.DeliveryTime;
            }
            return new Order { Id = counter += 1 , EstimatedTime = DateTime.Now.AddSeconds(seconds), Products= GenerateProducts() , Price = CulcPrice(products)};
        }
        
        private int CulcPrice(IList<Product> products)
        {
            int price =0;
            foreach (var item in products)
            {
                price += item.Price;
            }
            return price;
        }

        private IList<Product> GenerateProducts()
        {
            List<Product> product = new List<Product>(products);
            return product;
           
        }
    }
}