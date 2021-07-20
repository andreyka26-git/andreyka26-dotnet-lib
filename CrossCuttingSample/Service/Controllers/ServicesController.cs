using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [ApiController]
    public class ServicesController : ControllerBase
    {
        [HttpGet("service")]
        public IActionResult GetService()
        {
            var response = new Domain.Service
            {
                ServiceName = "Service Name 1",
                Url = "http://serviceurl.com"
            };
            
            return Ok(response);
        }

        [HttpGet("service/additional-info")]
        public IActionResult GetServiceAdditionalInfo()
        {
            var response = new Domain.ServiceAdditionalInfo
            {
                AdditionalInfo = "additional info 1"
            };

            return Ok(response);
        }
    }
}
