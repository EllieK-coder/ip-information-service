using System;
using System.Collections.Generic;
using System.Text;

namespace IPService.Application.Queries.Report
{
    // Read model for the report endpoint — one row per country.
    public record CountryReportItem(
        string CountryName,
        int AddressesCount,
        DateTime LastAddressUpdated);
}
