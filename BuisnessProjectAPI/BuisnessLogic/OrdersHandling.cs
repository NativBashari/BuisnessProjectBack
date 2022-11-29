using Models.DataModels;

namespace BuisnessProjectAPI.BuisnessLogic
{
    public class OrdersHandling
    {
        private readonly int numOfProductionSlots;
        private readonly Action<Order> addOrderToDelieveryAction;
        private readonly Action<Order> RemoveOrderFromPreparing;
        private readonly Action UpdateOrderState;
        Semaphore semaphore;

        public OrdersHandling(int numOfProductionSlots, Action<Order> AddOrderToDelievery, Action<Order> RemoveOrderFromPreparing, Action UpdateOrderState)
        {
            this.numOfProductionSlots = numOfProductionSlots;
            this.addOrderToDelieveryAction = AddOrderToDelievery;
            this.RemoveOrderFromPreparing= RemoveOrderFromPreparing;
            this.UpdateOrderState = UpdateOrderState;
            semaphore = new Semaphore(numOfProductionSlots, numOfProductionSlots);
        }

        public bool CheckMaterialAvailability(Order order, List<Material> materials)
        {
            foreach (Product product in order.Products!)
            {
                foreach (var productMaterial in product.Materials!)
                {
                    var m = materials.First((m) => m.Id == productMaterial.Id);
                    if (m.Amount < 1)
                    {
                        return false;
                    }
                    m.Amount--;
                }
            }
            return true;
        }

        public  async Task OrderHandlingAsync(Order order)
        {
            List<Task> tasks = new List<Task>();
            foreach (var product in order.Products!) // Need to await to all Tasks
            {
                tasks.Add(Task.Run( () => CreateProductAsync(product)));
            }
            await Task.WhenAll(tasks);
            RemoveOrderFromPreparing.Invoke(order);
            Console.WriteLine($"Order {order.Id} complete succesfully");
            addOrderToDelieveryAction.Invoke(order);
        }

        public async Task CreateProductAsync(Product product)
        {
            semaphore.WaitOne();
            Console.WriteLine($"Start Creating product: {product.Name}...");
            await Task.Delay(product.DeliveryTime * 1000);
            product.IsDone = true;
            UpdateOrderState.Invoke();
            Console.WriteLine($"product Creation Complete!");
            semaphore.Release();
        }
    }
}
