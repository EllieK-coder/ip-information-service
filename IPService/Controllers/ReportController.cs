using IPService.Application.Queries.Report;
using IPService.Domain.Exceptions;
using IPService.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IPService.Controllers
{
    [ApiController]
    [Route("report")]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET /report                -> all countries
        // GET /report?codes=US,GR    -> only those countries (FR-2)
        [HttpGet]
        public async Task<IActionResult> GetReport([FromQuery] string? codes)
        {
            var twoLetterCodes = string.IsNullOrWhiteSpace(codes)
                ? null
                : codes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var report = await _mediator.Send(new GetIpReportQuery(twoLetterCodes));
            var response = report.Select(r => new CountryReportResponse(
                r.CountryName, r.AddressesCount, r.LastAddressUpdated));
            return Ok(response);
        }
        
    }
}
