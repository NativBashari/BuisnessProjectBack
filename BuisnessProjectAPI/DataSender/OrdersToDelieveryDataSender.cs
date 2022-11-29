using BuisnessProjectAPI.HubConfig;
using Microsoft.AspNetCore.SignalR;
using Models.DataModels;

namespace BuisnessProjectAPI.DataSender
{
    public class OrdersToDelieveryDataSender
    {
        private readonly IHubContext<OrdersToDelieveryHub> _hubContext;

        public OrdersToDelieveryDataSender(IHubContext<OrdersToDelieveryHub> hubContext)
        {
            _hubContext= hubContext;
        }
        public void SendDelieveryData(List<Order> orders)
        {
            Task.Run(() => _hubContext.Clients.All.SendAsync("TransferOrdersToDelieveryData", orders));
        }
    }
}
