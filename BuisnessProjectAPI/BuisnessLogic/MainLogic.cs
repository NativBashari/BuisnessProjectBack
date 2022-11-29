using BuisnessProjectAPI.DataSender;
using DAL;
using Generator;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Models.DataModels;
using Simulator;
using System.Timers;
using Timer = System.Timers.Timer;

namespace BuisnessProjectAPI.BuisnessLogic
{
    public class MainLogic: IMainLogic
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
        private Customer? enteredCustomer;
        private Order? order;
        private List<ServiceStation>? serviceStationList;
        private List<Order> ordersToPrepare; // need thread-safety data structure
        private List<Order> ordersToDelievery; // need thread-safety data structure


        private readonly ServiceStationsDataSender _serviceStationsDataSender;
        private readonly OrdersToPrepareDataSender _ordersToPrepareDataSender;
        private readonly OrdersToDelieveryDataSender _ordersToDelieveryDataSender;


        public Order Order
        {
            get { return order!; }
            set
            {
                order = value;
                Task.Run(() => StartCustomerHandling());
                StartOrderHandling(order);
            }
        }

        public Customer EnteredCustomer
        {
            get { return enteredCustomer!; }
            set
            {
                enteredCustomer = value;
                Console.WriteLine($"Customer {enteredCustomer.Id} entered the restaurant");
                CustomerEnter(enteredCustomer);
                if (customersCounter < 1)
                {
                    Task.Run(() => StartCustomerHandling()); // Only for first call
                }
            }
        }

        public MainLogic(ServiceStationsDataSender serviceStationsDataSender, OrdersToPrepareDataSender ordersToPrepareDataSender, OrdersToDelieveryDataSender ordersToDelieveryDataSender)
        {
            _dataService = new DataService();
            buisness = _dataService.Buisness!;
            CreateServiceStations();
            _ordersGenerator = new OrdersGenerator();
            _customersGenerator = new CustomersGenerator();
            customersHandling = new CustomersHandling(buisness.ServiceStations, AddOrderToPreparingList);
            ordersHandling = new OrdersHandling(buisness.ProductionSlots, AddOrderToDelieveryList, RemoveOrderFromPreparingList, UpdateOrdersState);
            delieveryHandling = new DelieveryHandling(RemoveOrderFromDelieveryList, buisness.DelieveryTime);
            timerLoop = new Timer();
            timerLoop.Elapsed += GetCustomerWithOrder;
            timerLoop.Interval = 7000;
            timerLoop.Enabled = true;
            ordersToPrepare = new List<Order>();
            ordersToDelievery = new List<Order>();
            chooseQueueForEnquque = new ChooseQueue();
            chooseQueueForDequque = new ChooseQueue();

            _serviceStationsDataSender= serviceStationsDataSender;
            _ordersToPrepareDataSender= ordersToPrepareDataSender;
            _ordersToDelieveryDataSender = ordersToDelieveryDataSender;
        }

        private void CreateServiceStations()
        {
            serviceStationList = new List<ServiceStation>();
            for (int i = 0; i < buisness.ServiceStations; i++)
            {
                serviceStationList.Add(new ServiceStation() { Id = i + 1 });
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
            serviceStationList![chooseQueueForEnquque.GetQueueIndex(serviceStationList)].Customers!.Enqueue(customer);
            // send customers (queues) to clients -------------------
            _serviceStationsDataSender.SendServiceStationsData(serviceStationList);
            Console.WriteLine($"Customer {customer.Id} Is enter to queue");
        }
        private void StartCustomerHandling()
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
            Task.Run( () => customersHandling.CustomerHandlingAsync(customer));
        }
        private void StartOrderHandling(Order order)
        {
            if (ordersHandling.CheckMaterialAvailability(Order, _dataService.Materials!))
            {
                Task.Run(() => ordersHandling.OrderHandlingAsync(order));
            }
            else
            {
                order.IsFailed = true;
                Console.WriteLine("Order failed");
                Task.Run(() =>RemoveFailedOrderFromList(order));
                return;
            }
        }

        private void RemoveFailedOrderFromList(Order order)
        {
            ordersToPrepare.Remove(order);
        }
        private void AddOrderToPreparingList(Order order) // this is a delegate, Invoked from OrdersHandling service
        {
            Order = order;
            ordersToPrepare.Add(order);
            _ordersToPrepareDataSender.SendOrdersData(ordersToPrepare);
        }

        private void RemoveOrderFromPreparingList(Order order) // this is a delegate, Invoked from OrdersHandling service
        {
            ordersToPrepare.Remove(order);
            _ordersToPrepareDataSender.SendOrdersData(ordersToPrepare);
        }
        private void AddOrderToDelieveryList(Order order) // this is a delegate, Invoked from OrdersHandling service
        {     
            ordersToDelievery.Add(order);
            Task.Run(() => delieveryHandling.DelieveryHandler(order));
            // send delievery data to clients---------
            _ordersToDelieveryDataSender.SendDelieveryData(ordersToDelievery);
        }

        private void RemoveOrderFromDelieveryList(Order order) // this is a delegate, Invoked from Delievery Handling service
        {
            ordersToDelievery.Remove(order);
            //send delievery data to clients------------
            _ordersToDelieveryDataSender.SendDelieveryData(ordersToDelievery);
        }

        private void UpdateOrdersState()
        {
            _ordersToPrepareDataSender.SendOrdersData(ordersToPrepare);
        }

        ///Methods for manager
        public void PauseCustomers()
        {
            this.timerLoop.Stop();
        }
        public void ContinueCustomers()
        {
            this.timerLoop.Start();
        }
        public void FastSimulator()
        {
            this.timerLoop.Interval = 3000;
            //do the ordering time faster 
            // do the proccessing time faster
            // do the delievery time faster

        }
    }
}
