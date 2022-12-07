using BuisnessProjectAPI.DataSender.Contracts;
using BuisnessProjectAPI.HubConfig;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.SignalR;
using Models.DataModels;

namespace BuisnessProjectAPI.DataSender
{
    public class ServiceStationsDataSender : IServiceStationsDataSender
    {
        private readonly IHubContext<ServiceStationsHub> _hubContext;

        public ServiceStationsDataSender(IHubContext<ServiceStationsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void SendServiceStationsData(List<ServiceStation> serviceStations)
        {
            Task.Run(() =>  _hubContext.Clients.All.SendAsync("TransferServiceStationsData", serviceStations));
        }

       
    }
}
