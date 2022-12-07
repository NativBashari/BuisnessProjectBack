using BuisnessProjectAPI.BuisnessLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DataModels;

namespace BuisnessProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IMainLogic mainLogic;

        public ManagerController(IMainLogic mainLogic)
        {
            this.mainLogic = mainLogic;
        }
        [HttpGet]
        [Route("Start")]
        public IActionResult StartSimulator()
        {
            mainLogic.StartLogic();
            Console.WriteLine("Logic Started");
            return Ok("LogicStarted");
        }
        [HttpGet]
        [Route("Pause")]
        public IActionResult PauseSimulator()
        {
            Console.WriteLine("Logic paused");
            mainLogic.PauseCustomers();
            return Ok("Logic paused");   
        }
        [HttpGet]
        [Route("Continue")]
        public IActionResult ContinueSimulator()
        {
            Console.WriteLine("Logic continued");
            mainLogic.ContinueCustomers();
            return Ok("Logic continued");
        }

        [HttpPost]
        public IActionResult SetBuisness(Buisness buisness)
        {
            mainLogic.SetBuisnsess(buisness);
            Console.WriteLine(buisness.Name);
            return Ok(buisness);
        }

        
    }
}
