using IPService.Application.Queries;
using IPService.Domain.Entities;
using IPService.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IPService.Controllers
{
    [ApiController]
    [Route("ip")]
    public class IpController : ControllerBase
    {

        private readonly IMediator _mediator;//for not be necessary to know the handlers

        public IpController(IMediator mediator) { 
        _mediator = mediator;
        }


        [HttpGet("{address}")]
        public async Task<IActionResult> GetByIp(string address)
        {
            long addressId;
            try
            {
                addressId = Ipv4Address.ParseToLong(address);//convert 192.168.1.1 to 23213131
            }
            catch (InvalidIpv4InputException)//our exception for not valid ip
            {
                return BadRequest("Invalid IPv4 address");
            }

            var result = await _mediator.Send(new GetCountryByIpQuery(addressId));//check for the country pf ip

            if (result is null)
                return NotFound();//404

            return Ok(result);//200
        }
    }
}