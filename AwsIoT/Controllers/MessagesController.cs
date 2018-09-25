using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwsIoT.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AwsIoT.Controllers
{
    public class MessagesController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {

            return View();
        }


        [HttpPost]
        [Route("Messages")]
        public IActionResult Publish([FromBody] PublishMessageDTO messageDTO)
        {
            //Publisher().Publish(messageDTO);
            return Created(String.Empty, messageDTO);
        }

        private WebsocketPublisher Publisher()
        {
            return new WebsocketPublisher();
        }
    }
}
