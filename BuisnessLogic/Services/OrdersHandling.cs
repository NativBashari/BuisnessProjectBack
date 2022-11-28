using DAL;
using Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.Services
{
    internal class OrdersHandling
    {
        private readonly int numOfProductionSlots;
        private readonly Action<Order> addOrderToDelieveryAction;
        Semaphore semaphore;

        public OrdersHandling(int numOfProductionSlots, Action<Order> AddOrderToDelievery)
        {
            this.numOfProductionSlots = numOfProductionSlots;
            addOrderToDelieveryAction = AddOrderToDelievery;
            semaphore = new Semaphore(numOfProductionSlots, numOfProductionSlots);
        }

        public bool CheckMaterialAvailability(Order order, List<Material> materials)
        {
            foreach (Product product in order.Products!)
            {
                foreach(var productMaterial in product.Materials!)
                {
                    var m = materials.First((m) => m.Id == productMaterial.Id);
                    if(m.Amount < 1)
                    {
                        return false;
                    }
                    m.Amount--;
                }
            }
            return true;
        }

        public async Task OrderHandlingAsync(Order order)
        {
            foreach (var product in order.Products!)
            {
                await Task.Run(async() => await CreateProductAsync(product));
            }
            Console.WriteLine($"Order {order.Id} complete succesfully");
            addOrderToDelieveryAction.Invoke(order);
        }

        public async Task CreateProductAsync(Product product)
        {
            semaphore.WaitOne();
            Console.WriteLine($"Start Creating product: {product.Name}...");
            await Task.Delay(product.DeliveryTime * 1000);
            Console.WriteLine($"product Creation Complete!");
            semaphore.Release();
        }


    }
}
