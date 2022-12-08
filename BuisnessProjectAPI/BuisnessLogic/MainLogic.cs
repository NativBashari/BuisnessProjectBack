using BuisnessProjectAPI.DataSender;
using BuisnessProjectAPI.DataSender.Contracts;
using DAL;
using Generator;
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
        readonly DataService _dataService;
        CustomersGenerator _customersGenerator;
        CustomersHandling customersHandling;
        OrdersHandling ordersHandling;
        DelieveryHandling delieveryHandling;
        Buisness buisness;
        private Customer? enteredCustomer;
        private Order? enteredOrder;
        private List<ServiceStation>? serviceStationList;
        private List<Order> ordersToPrepare; 
        private List<Order> ordersToDelievery;
        Buisness Buisness;


        private readonly IServiceStationsDataSender _serviceStationsDataSender;
        private readonly IOrdersToPrepareDataSender _ordersToPrepareDataSender;
        private readonly IOrdersToDelieveryDataSender _ordersToDelieveryDataSender;


        public Order EnteredOrder
        {
            get { return enteredOrder!; }
            set
            {
                enteredOrder = value;
                Task.Run(() => StartCustomerHandling());
                Task.Run(() =>StartOrderHandlingAsync(enteredOrder));
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
              if (customersCounter < 1 || enteredCustomer.Priority > 0) // If the customer is a celebrity, start handling immidiatly
               {
                    Task.Run(() => StartCustomerHandling()); // Only for first call
               }
            }
        }

        public MainLogic(IServiceStationsDataSender serviceStationsDataSender, IOrdersToPrepareDataSender ordersToPrepareDataSender, IOrdersToDelieveryDataSender ordersToDelieveryDataSender)
        {
            _dataService = new DataService();
            _customersGenerator = new CustomersGenerator();
            ordersToPrepare = new List<Order>();
            ordersToDelievery = new List<Order>();
            chooseQueueForEnquque = new ChooseQueue();
            //Data senders
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

        private void GetCustomer(object? sender, ElapsedEventArgs e)
        {
            var customer = _customersGenerator.GenerateCustomer();
            EnteredCustomer = customer;
        }

        private void CustomerEnter(Customer customer)
        {
            try
            {
            serviceStationList![chooseQueueForEnquque.GetQueueIndex(serviceStationList)].Customers!.Enqueue(customer); 
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            // send customers (queues) to clients -------------------
            _serviceStationsDataSender.SendServiceStationsData(serviceStationList!);
            Console.WriteLine($"Customer {customer.Id} Is enter to queue");
        }
        private async Task StartCustomerHandling()
        {
            customersCounter++;
            Task.Run(() => customersHandling.CustomerHandlingAsync(serviceStationList!)); // Not using await because it need to run asynchronusly, The semaphore is limit the number of tasks by the service stations count
        }
        private async Task StartOrderHandlingAsync(Order order)
        {
            if (order.IsFailed)
            {
                Console.WriteLine("Order failed");
                await Task.Run(() => RemoveFailedOrderFromList(order));
                return;
            }
            if (ordersHandling.CheckMaterialAvailability(EnteredOrder, buisness.Materials!)) 
            {
               await Task.Run(() => ordersHandling.OrderHandlingAsync(order));
            }
            else
            {
                order.IsFailed = true;
                Console.WriteLine("Order failed");
                await Task.Run(() =>RemoveFailedOrderFromList(order));
                return;
            }
        }

        private void Reproduce(Order order)
        {
            Console.WriteLine($"Order {order.Id} is Reproduce");
            Task.Run(() => StartOrderHandlingAsync(order));
        }

        private void RemoveFailedOrderFromList(Order order)
        {
            ordersToPrepare.Remove(order);
        }
        private void AddOrderToPreparingList(Order order) // An action , Invoked from Cutomers Handling service
        {
            EnteredOrder = order;
            ordersToPrepare.Add(order);
            _ordersToPrepareDataSender.SendOrdersData(ordersToPrepare);
        }

        private void RemoveOrderFromPreparingList(Order order) // An action , Invoked from OrdersHandling service
        {
            ordersToPrepare.Remove(order);
            _ordersToPrepareDataSender.SendOrdersData(ordersToPrepare);
        }
        private void AddOrderToDelieveryList(Order order) // An action , Invoked from OrdersHandling service
        {     
            ordersToDelievery.Add(order);
            Task.Run(() => delieveryHandling.DelieveryHandler(order));
            // send delievery data to clients---------
            _ordersToDelieveryDataSender.SendDelieveryData(ordersToDelievery);
        }

        private void RemoveOrderFromDelieveryList(Order order) // An action , Invoked from Delievery Handling service
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

        public void SetBuisnsess(Buisness buisness)
        {
            this.buisness = buisness;
            this.buisness.Materials = _dataService.materials;
        }
        public void StartLogic()
        {
            if(buisness != null)
            {
                CreateServiceStations();
                customersHandling = new CustomersHandling(buisness.ServiceStations, AddOrderToPreparingList, new OrdersGenerator(_dataService));
                ordersHandling = new OrdersHandling(buisness.ProductionSlots, AddOrderToDelieveryList, RemoveOrderFromPreparingList, UpdateOrdersState);
                delieveryHandling = new DelieveryHandling(RemoveOrderFromDelieveryList, buisness.DelieveryTime, Reproduce);
                timerLoop = new Timer();
                timerLoop.Elapsed += GetCustomer;
                timerLoop.Interval = buisness.CustomersEntryRate * 1000;
                timerLoop.Enabled = true;
            }
        }

        public void CloseServiceStation(int id)
        {
            if (serviceStationList!.Count > 1)
            {
                timerLoop.Stop();
                ChooseQueue chooseQueue = new ChooseQueue();
                var serviceStation = serviceStationList!.FirstOrDefault(s => s.Id == id);
                var customers = serviceStation!.Customers;
                serviceStationList!.Remove(serviceStation!);
                foreach (var c in customers!)
                {
                    serviceStationList[chooseQueue.GetQueueIndex(serviceStationList)].Customers!.Enqueue(c);
                }
                timerLoop.Start();
                _serviceStationsDataSender.SendServiceStationsData(serviceStationList);
            }
        }

    }
}
