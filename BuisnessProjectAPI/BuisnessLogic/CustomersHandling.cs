using Models.DataModels;

namespace BuisnessProjectAPI.BuisnessLogic
{
    public class CustomersHandling
    {

        private readonly int numOfQueues;
        Action<Order> AddOrderToPreparingList;
        Semaphore semaphore;
        public CustomersHandling(int numOfQueues, Action<Order> AddOrderToPreparingList)
        {
            this.numOfQueues = numOfQueues;
            semaphore = new Semaphore(numOfQueues, numOfQueues);
            this.AddOrderToPreparingList = AddOrderToPreparingList;
        }

        public Customer CustomerDequeue(Queue<Customer> customers)
        {
            if (customers.Count < 1) return null!;
            return customers.Dequeue();
        }
        public async Task CustomerHandlingAsync(Customer customer)
        {
            semaphore.WaitOne();
            Console.WriteLine($"Customer {customer.Id} Can order now");
            await Task.Delay(customer.OrderingTime * 1000);
            AddOrderToPreparingList.Invoke(customer.Order);
            Console.WriteLine($"Order accept from customer {customer.Id}");
            semaphore.Release();
        }
    }
}
