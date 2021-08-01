using System.Threading;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Api.Application;
using MediatR;

namespace Api.Controllers
{
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServicesService _service;

        public ServicesController(IServicesService service)
        {
            _service = service;
        }

        [HttpGet("service")]
        public async Task<IActionResult> GetServiceAsync(CancellationToken cancellationToken)
        {
            var response = await _service.GetServiceAsync(cancellationToken);

            return Ok(response);
        }

        [HttpGet("additional-info")]
        public async Task<IActionResult> GetServiceAdditionalInfoAsync(CancellationToken cancellationToken)
        {
            var response = await _service.GetServiceAdditionalInfo(cancellationToken);

            return Ok(response);
        }
    }
}
