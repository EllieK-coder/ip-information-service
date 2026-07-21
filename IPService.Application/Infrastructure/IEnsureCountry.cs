using System.Threading;
using System.Threading.Tasks;
using IPService.Application.DTOs;

namespace IPService.Application.Interfaces
{
    /// <summary>
    /// Ensure a Country row exists for the given DTO (idempotent get-or-create).
    /// Public because it is injected into the public GetIpInfoHandler constructor.
    /// </summary>
    public interface IEnsureCountry
    {
        //Ensure a country exists, and if not save it 
        Task EnsureExistsAsync(CountryDto country, CancellationToken cancellationToken = default);
    }
}
