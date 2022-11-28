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
    public class OrdersToDelieveryController : ControllerBase
    {
        private readonly IHubContext<OrdersToDelieveryHub> _hubContext;
        private readonly MainLogic _mainLogic;

        public OrdersToDelieveryController(IHubContext<OrdersToDelieveryHub> hubContext, MainLogic mainLogic)
        {
          _hubContext = hubContext;
          _mainLogic = mainLogic;
        }

        public IActionResult Get()
        {
            var timerManager = new TimerManager(() => _hubContext.Clients.All.SendAsync("TransferOrdersToDelieveryData", _mainLogic.GetOrdersToDelievery()));
            return Ok(new { Message = "Order to delievery Hub is connected"});
        }
    }
}
