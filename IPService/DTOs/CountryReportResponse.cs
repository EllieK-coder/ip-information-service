namespace IPService.DTOs
{
    // FR-1's API contract for GET /report, decoupled from the internal
    // CountryReportItem read model.
    public record CountryReportResponse(string CountryName, int AddressesCount, DateTime LastAddressUpdated);
}
