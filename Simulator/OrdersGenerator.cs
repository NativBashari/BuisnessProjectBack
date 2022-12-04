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
            foreach (var product in products)
            {
                seconds += product.DeliveryTime;
            }
            return new Order { Id = counter += 1, EstimatedTime = DateTime.Now.AddSeconds(seconds), Products = GenerateProducts(), Price = CulcPrice(products) };
        }

        private int CulcPrice(IList<Product> products)
        {
            int price = 0;
            foreach (var item in products)
            {
                price += item.Price;
            }
            return price;
        }

        private IList<Product> GenerateProducts()
        {
            List<Product> product = new List<Product>();
            foreach(Product p in products)
            {
                product.Add(new Product() {Id = p.Id , Name = p.Name , Price = p.Price , DeliveryTime = p.DeliveryTime , Image = p.Image , IsDone = false  , Priority = p.Priority, Materials = p.Materials });
            }
            return product;
        }
    }
}