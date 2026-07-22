using IPService.Application.DTOs;
using IPService.Application.Interfaces;
using IPService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IPService.Infrastructure.Persistence.Commands
{
    public class EnsureCountryPersistence : IEnsureCountry
    {
        private readonly IpDbContext _context;

        public EnsureCountryPersistence(IpDbContext context)
        {
            _context = context;
        }

        public async Task EnsureExistsAsync(CountryDto country, CancellationToken cancellationToken = default)
        {
            var existing = await _context.Countries//check if the country which give an ip exist
            .FirstOrDefaultAsync(c => c.TwoLetterCode == country.TwoLetterCode);

            if (existing != null) return;//if exist do nothing, leave


            var newCountry = new Country(country.CountryName,country.TwoLetterCode,country.ThreeLetterCode);//make an object

            _context.Countries.Add(newCountry);//add it 

            await _context.SaveChangesAsync();

        }
    }
}