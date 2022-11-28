using BuisnessLogic.Services;
using DAL;
using Generator;
using Microsoft.AspNetCore.SignalR;
using Models.DataModels;
using Simulator;
using System.Timers;
using Timer = System.Timers.Timer;

namespace BuisnessLogic
{
    public class MainLogic
    {
        int customersCounter;
        Timer timerLoop;
        ChooseQueue chooseQueueForEnquque;
        ChooseQueue chooseQueueForDequque;
        readonly DataService _dataService;
        readonly OrdersGenerator _ordersGenerator;
        readonly CustomersGenerator _customersGenerator;
        readonly CustomersHandling customersHandling;
        readonly OrdersHandling ordersHandling;
        readonly DelieveryHandling delieveryHandling;
        Buisness buisness;
        private  Customer? enteredCustomer;
        private Order? order;
        private List<ServiceStation>? serviceStationList;
        private List<Order> ordersToPrepare; // need thread-safety data structure
        private List<Order> ordersToDelievery; // need thread-safety data structure
        

        public Order Order
        {
            get { return order!; }
            set { 
                order = value;
                Console.WriteLine($"The order is {order.Id}");
                Task.Run(() => StartCustomerHandling());
                StartOrderHandling();
            }
        }

        public  Customer EnteredCustomer
        {
            get { return enteredCustomer!; }
            set
            {
                enteredCustomer = value;
                Console.WriteLine($"Customer {enteredCustomer.Id} entered the restaurant");
                CustomerEnter(enteredCustomer);
                if(customersCounter < 1)
                {
                  Task.Run(() => StartCustomerHandling()); // Only for first call
                }
            }
        }
   
        public MainLogic()
        {
            _dataService = new DataService();
            buisness = _dataService.Buisness;
            CreateServiceStations();
            _ordersGenerator = new OrdersGenerator();
            _customersGenerator = new CustomersGenerator();
            customersHandling = new CustomersHandling(buisness.ServiceStations);
            ordersHandling = new OrdersHandling(buisness.ProductionSlots, AddOrderToDelieveryList);
            delieveryHandling = new DelieveryHandling(RemoveOrderFromDelieveryList, buisness.DelieveryTime);
            timerLoop = new Timer();
            timerLoop.Elapsed += GetCustomerWithOrder;  
            timerLoop.Interval= 9000;
            timerLoop.Enabled= true;
            ordersToPrepare= new List<Order>();
            ordersToDelievery = new List<Order>();
            chooseQueueForEnquque = new ChooseQueue();
            chooseQueueForDequque = new ChooseQueue();
        }

        private void CreateServiceStations()
        {
            serviceStationList= new List<ServiceStation>();
            for (int i = 0; i < buisness.ServiceStations; i++)
            {
                serviceStationList.Add(new ServiceStation() {Id = i + 1 });
            }
        }

        private void GetCustomerWithOrder(object? sender, ElapsedEventArgs e)
        {
            var customer = _customersGenerator.GenerateCustomer(); 
            var order = _ordersGenerator.GenerateOrder(buisness.Products!);
            customer.Order = order;
            EnteredCustomer = customer;           
        }

        private void CustomerEnter(Customer customer)
        {
            serviceStationList[chooseQueueForEnquque.GetQueueIndex(serviceStationList)].Customers!.Enqueue(customer);
            Console.WriteLine($"Customer {customer.Id} Is enter to queue");
        }
        private async Task StartCustomerHandling()
        {
            customersCounter++;
            int selectedIndex;
            do
            {
                selectedIndex = chooseQueueForDequque.GetQueueIndex(serviceStationList);
            }
            while (serviceStationList[selectedIndex].Customers!.Count < 1);
            var customer = customersHandling.CustomerDequeue(serviceStationList[selectedIndex].Customers!);
            if (customer == null) return;
            Order =  Task.Run(() =>  customersHandling.CustomerHandlingAsync(customer)).Result;
            ordersToPrepare.Add(Order);
        }
        private async void StartOrderHandling()
        {
            if (ordersHandling.CheckMaterialAvailability(Order, _dataService.Materials))
            {
                Task.Run(() => ordersHandling.OrderHandlingAsync(Order));
            }
            else
            {
                Console.WriteLine("Order failed");
                return;
            }                    
        }
        private void AddOrderToDelieveryList(Order order) // this is a delegate, Invoked from OrdersHandling service
        {
            ordersToPrepare.Remove(order);
            ordersToDelievery.Add(order);
            Task.Run(() => delieveryHandling.DelieveryHandler(order));
        }

        private void RemoveOrderFromDelieveryList(Order order) // this is a delegate, Invoked from Delievery Handling service
        {
            ordersToDelievery.Remove(order);
        }

        public List<ServiceStation> GetServiceStations()
        {
            return this.serviceStationList!;
        }
        public List<Order> GetOrdersToPrepare()
        {
            return this.ordersToPrepare;
        }
        public List<Order> GetOrdersToDelievery()
        {
            return this.ordersToDelievery;
        }

    }
}