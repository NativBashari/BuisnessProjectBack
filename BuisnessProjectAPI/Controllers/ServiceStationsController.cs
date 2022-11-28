using BuisnessLogic;
using BuisnessProjectAPI.HubConfig;
using BuisnessProjectAPI.TimerFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BuisnessProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceStationsController : ControllerBase
    {
        private readonly IHubContext<ServiceStationsHub> _hubContext;
        private readonly MainLogic _mainLogic;

        public ServiceStationsController(IHubContext<ServiceStationsHub> hubContext, MainLogic mainLogic)
        {
            _hubContext = hubContext;
            _mainLogic = mainLogic;
        }

        public IActionResult Get()
        {
            var timerManager = new TimerManager(() => _hubContext.Clients.All.SendAsync("TransferServiceStationsData", _mainLogic.GetServiceStations()));
            return Ok(new { Message = "Request completed" });
        }
        
    }
}
