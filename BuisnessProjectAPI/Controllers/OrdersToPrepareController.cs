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
    public class OrdersToPrepareController : ControllerBase
    {
        private readonly IHubContext<OrdersToPrepareHub> _hubContext;
        private readonly MainLogic _mainLogic;

        public OrdersToPrepareController(IHubContext<OrdersToPrepareHub> hubContext ,MainLogic mainLogic)
        {
            _hubContext = hubContext;
            _mainLogic = mainLogic;
        }

        public IActionResult Get()
        {
            var timerManager = new TimerManager(() => _hubContext.Clients.All.SendAsync("TransferOrdersToPrepareData", _mainLogic.GetOrdersToPrepare()));
            return Ok(new { Message = "Orders to prepare Hub is conected" });
        }

    }
}
