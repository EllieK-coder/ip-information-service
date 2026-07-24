using IPService.Application.DTOs;

namespace IPService.Application.Interfaces
{
    public interface IEnsureCountry
    {
        //Ensure a country exists, and if not save it 
        Task EnsureExistsAsync(CountryDto country, CancellationToken cancellationToken = default);
    }
}
