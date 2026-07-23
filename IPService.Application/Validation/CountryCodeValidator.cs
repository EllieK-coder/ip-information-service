using System.Text.RegularExpressions;
using IPService.Domain.Exceptions;

namespace IPService.Application.Validation
{
    // FR-4: each provided code must be a well-formed two-letter code
    // ([A-Za-z]{2}), or the whole input is null/empty ("all countries").
    // Normalizes case here (upper-case) so "gr"/"GR" both match the
    // upper-case codes stored on Country/Ipv4Address -- otherwise Step 2's
    // Contains filter would silently return nothing for valid-but-wrong-case input.
    public static class CountryCodeValidator
    {
        private static readonly Regex TwoLetterCode = new("^[A-Za-z]{2}$", RegexOptions.Compiled);

        public static IReadOnlyCollection<string>? NormalizeAndValidate(IReadOnlyCollection<string>? codes)
        {
            if (codes is null || codes.Count == 0)
                return null; // FR-2: null/empty = "all countries"

            var normalized = new List<string>(codes.Count);

            foreach (var code in codes)
            {
                if (code is null || !TwoLetterCode.IsMatch(code))
                    throw new InvalidCountryCodeException(code ?? "null");

                normalized.Add(code.ToUpperInvariant());
            }

            return normalized;
        }
    }
}
