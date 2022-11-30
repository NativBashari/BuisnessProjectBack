using BuisnessProjectAPI.BuisnessLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuisnessProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceStationsController : ControllerBase
    {
        private readonly IMainLogic mainLogic;

        public ServiceStationsController(IMainLogic mainLogic)
        {
            this.mainLogic = mainLogic;
        }

        [HttpGet("{id}")]
        [Route("CloseServiceStation/{id:int}")]
        public IActionResult CloseServiceStation(int id)
        {
            mainLogic.CloseServiceStation(id);
            return Ok($"Service station {id} closed");
        }
    }
}
