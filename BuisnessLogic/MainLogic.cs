using BuisnessLogic.Services;
using DAL;
using Generator;
using Models.DataModels;
using Simulator;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Timers;
using Timer = System.Timers.Timer;

namespace BuisnessLogic
{
    public class MainLogic
    {
        Timer timerLoop;
        ChooseQueue chooseQueue;
        readonly DataService _dataService;
        readonly OrdersGenerator _ordersGenerator;
        readonly CustomersGenerator _customersGenerator;
        readonly CustomersHandling customersHandling;
        readonly OrdersHandling ordersHandling;
        Buisness buisness;
        private  Customer? enteredCustomer;
        private Order? order;
        private List<ServiceStation>? serviceStationList;
        private List<Order> ordersToDelievery;


        public Order Order
        {
            get { return order!; }
            set { 
                order = value;
                Console.WriteLine($"The order is {order.Id}");
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
                StartCustomerHandling();
            }
        }
   
        public MainLogic()
        {
            _dataService = new DataService();
            buisness = _dataService.Buisness;
            CreateServiceStations();
            _ordersGenerator = new OrdersGenerator();
            _customersGenerator = new CustomersGenerator();
            customersHandling = new CustomersHandling(buisness.ServiceStations, StartCustomerHandling);
            ordersHandling = new OrdersHandling(buisness.ProductionSlots, AddOrderToDelieveryList);
            timerLoop = new Timer();
            timerLoop.Elapsed += GetCustomerWithOrder;  
            timerLoop.Interval= 2000;
            timerLoop.Enabled= true;
            ordersToDelievery = new List<Order>();
            chooseQueue = new ChooseQueue();
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
            serviceStationList[chooseQueue.GetQueueIndex(serviceStationList)].Customers.Enqueue(customer);
            Console.WriteLine($"Customer {customer.Id} Is enter to queue");
        }
        private async void StartCustomerHandling()
        {
            ServiceStation selectedServiceStation;
            do
            {
                selectedServiceStation = serviceStationList![chooseQueue.GetQueueIndex(serviceStationList)];
            } while (selectedServiceStation.Customers!.Count < 2);
           
            var customer = customersHandling.CustomerDequeue(selectedServiceStation.Customers);
            if (customer == null) return;
             Order =  Task.Run(async () => await customersHandling.CustomerHandlingAsync(customer)).Result;
        }
        private async void StartOrderHandling()
        {
            if (ordersHandling.CheckMaterialAvailability(Order, _dataService.Materials))
            {
                ordersHandling.OrderHandlingAsync(Order);
            }
            else
            {
                Console.WriteLine("Order failed");
                return;
            }                    
        }
        private void AddOrderToDelieveryList(Order order) // this is a delegate, Invoked from OrdersHandling service
        {

            ordersToDelievery.Add(order);
        }

        public List<ServiceStation> GetServiceStations()
        {
            return this.serviceStationList!;
        }

    }
}