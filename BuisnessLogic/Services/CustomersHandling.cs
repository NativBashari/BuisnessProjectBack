using Models.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.Services
{
    internal class CustomersHandling
    {
        private readonly int numOfQueues;
        Semaphore semaphore;
        public CustomersHandling(int numOfQueues)
        {
            this.numOfQueues = numOfQueues;
            semaphore = new Semaphore(numOfQueues, numOfQueues);
        }
        
        public Customer CustomerDequeue(Queue<Customer> customers)
        {
            if (customers.Count < 1) return null!;
            return customers.Dequeue();
        }
        public async Task<Order> CustomerHandlingAsync(Customer customer)
        {
            semaphore.WaitOne();
            Console.WriteLine($"Customer {customer.Id} Can order now");
            await Task.Delay(customer.OrderingTime * 1000);
            Console.WriteLine($"Order accept from customer {customer.Id}");
            semaphore.Release();
            return customer.Order;
        }

    
    }
}
