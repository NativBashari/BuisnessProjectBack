using Models.DataModels;
using Simulator;

namespace BuisnessProjectAPI.BuisnessLogic
{
    public class CustomersHandling
    {
        OrdersGenerator ordersGenerator;
        Action<Order> AddOrderToPreparingList;
        ChooseQueue chooseQueueForDequque;
        Semaphore semaphore;
        public CustomersHandling(int numOfQueues, Action<Order> AddOrderToPreparingList, OrdersGenerator ordersGenerator)
        {
            semaphore = new Semaphore(numOfQueues, numOfQueues);
            this.AddOrderToPreparingList = AddOrderToPreparingList;
            chooseQueueForDequque = new ChooseQueue();
            this.ordersGenerator = ordersGenerator;
        }

        public Customer CustomerDequeue(Queue<Customer> customers)
        {
            if (customers.Count < 1) return null!;
            return customers.Dequeue();
        }
        public async void CustomerHandlingAsync(List<ServiceStation> serviceStationList)
        {
            semaphore.WaitOne();
            int tryNum =1;
            bool success = false;
            while(tryNum != 3 || success != true)
            {
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
                if (customer == null) return;
                Console.WriteLine($"Customer {customer.Id} Can order now");
                await Task.Delay(customer.OrderingTime * 1000);

                customer.Order = ordersGenerator.GenerateOrder();


                if (IsMoneyEnough(customer.AvailableMoney, customer.Order!.Price))
                {
                    Console.WriteLine($"Order accept from customer {customer.Id}");
                    await Task.Run(() => AddOrderToPreparingList.Invoke(customer.Order!));
                    success = true;
                }
                else
                {
                    Console.WriteLine($"Customer {customer.Id} is canceling his order, not enough money!");
                }
                tryNum++;
            }
            semaphore.Release();
        }

        private bool IsMoneyEnough(int availableMoney, int orderPrice)
        {
            if (availableMoney >= orderPrice) return true;
            else return false;
        }


    }
}
