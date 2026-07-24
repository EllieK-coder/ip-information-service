using IPService.Application.Queries;
using IPService.Domain.Entities;
using IPService.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IPService.Controllers
{
    [ApiController]
    [Route("ip")]
    public class Ipv4Controller : ControllerBase
    {

        private readonly IMediator _mediator;

        public Ipv4Controller(IMediator mediator) { 
        _mediator = mediator;
        }

        [HttpGet("{address}")]
        public async Task<IActionResult> GetCountryByIpv4(string address)
        {
            var addressId = Ipv4Address.ParseToLong(address);   // πετάει → πιάνεται κεντρικά
            var result = await _mediator.Send(new GetCountryByIpv4Query(addressId));
            return Ok(result);
        }

    }
}