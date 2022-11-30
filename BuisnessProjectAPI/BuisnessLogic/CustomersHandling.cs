using Models.DataModels;

namespace BuisnessProjectAPI.BuisnessLogic
{
    public class CustomersHandling
    {

        private readonly int numOfQueues;
        Action<Order> AddOrderToPreparingList;
        ChooseQueue chooseQueueForDequque;
        Semaphore semaphore;
        public CustomersHandling(int numOfQueues, Action<Order> AddOrderToPreparingList)
        {
            this.numOfQueues = numOfQueues;
            semaphore = new Semaphore(numOfQueues, numOfQueues);
            this.AddOrderToPreparingList = AddOrderToPreparingList;
            chooseQueueForDequque = new ChooseQueue();
        }

        public Customer CustomerDequeue(Queue<Customer> customers)
        {
            if (customers.Count < 1) return null!;
            return customers.Dequeue();
        }
        public async Task CustomerHandlingAsync(List<ServiceStation> serviceStationList)
        {
            semaphore.WaitOne();

            int selectedIndex;
            try
            {
                do
                {
                    selectedIndex = chooseQueueForDequque.GetQueueIndex(serviceStationList!);
                }
                while (serviceStationList![selectedIndex].Customers!.Count < 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
            var customer = CustomerDequeue(serviceStationList[selectedIndex].Customers!);



            Console.WriteLine($"Customer {customer.Id} Can order now");
            await Task.Delay(customer.OrderingTime * 1000);
            await Task.Run (() => AddOrderToPreparingList.Invoke(customer.Order));
            Console.WriteLine($"Order accept from customer {customer.Id}");
            semaphore.Release();
        }
    }
}
