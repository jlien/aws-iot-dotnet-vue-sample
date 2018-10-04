using AwsIoT.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace AwsIoT.Controllers
{
    public class WebsocketProviderInfoController : Controller
    {
        [HttpPost]
        [Route("/WebsocketProviderInfo")]
        public IActionResult Create([FromBody] SocketProviderRequestDTO dto)
        {
            var result = new TopicAuthorizer(dto.userGuid).GenerateProfileAsync();
            return Created(string.Empty, result);
        }
    }
}
