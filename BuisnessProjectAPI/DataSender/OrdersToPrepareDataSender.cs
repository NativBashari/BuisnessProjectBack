using BuisnessProjectAPI.DataSender.Contracts;
using BuisnessProjectAPI.HubConfig;
using Microsoft.AspNetCore.SignalR;
using Models.DataModels;

namespace BuisnessProjectAPI.DataSender
{
    public class OrdersToPrepareDataSender : IOrdersToPrepareDataSender
    {
        IHubContext<OrdersToPrepareHub> _hubContext;

        public OrdersToPrepareDataSender(IHubContext<OrdersToPrepareHub> hubContext)
        {
            _hubContext= hubContext;
        }

        public void SendOrdersData(List<Order> orders)
        {
           Task.Run(() => _hubContext.Clients.All.SendAsync("TransferOrdersToPrepareData", orders));
        }
    }
}
