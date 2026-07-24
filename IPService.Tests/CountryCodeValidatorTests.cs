using IPService.Application.Validation;
using IPService.Domain.Exceptions;

namespace IPService.Tests
{
    public class CountryCodeValidatorTests
    {
        [Fact]
        public void NormalizeAndValidate_NullInput_ReturnsNull()//FR-2: null = all countries
        {
            var result = CountryCodeValidator.NormalizeAndValidate(null);

            Assert.Null(result);
        }

        [Fact]
        public void NormalizeAndValidate_EmptyInput_ReturnsNull()//FR-2: empty list treated same as null
        {
            var result = CountryCodeValidator.NormalizeAndValidate(Array.Empty<string>());

            Assert.Null(result);
        }

        [Fact]
        public void NormalizeAndValidate_LowerCaseCodes_NormalizesToUpperCase()//"gr" must match stored "GR"
        {
            var result = CountryCodeValidator.NormalizeAndValidate(new[] { "gr", "us" });

            Assert.NotNull(result);
            Assert.Equal(new[] { "GR", "US" }, result);
        }

        [Fact]
        public void NormalizeAndValidate_MixedCaseCodes_AllNormalized()
        {
            var result = CountryCodeValidator.NormalizeAndValidate(new[] { "Gr", "uS", "DE" });

            Assert.NotNull(result);
            Assert.Equal(new[] { "GR", "US", "DE" }, result);
        }

        [Theory]
        [InlineData("USA")]   // 3 letters
        [InlineData("U")]     // 1 letter
        [InlineData("U1")]    // contains a digit
        [InlineData("")]      // empty string entry
        [InlineData(" G")]    // whitespace, not a letter
        public void NormalizeAndValidate_MalformedCode_ThrowsInvalidCountryCodeException(string badCode)
        {
            var ex = Assert.Throws<InvalidCountryCodeException>(
                () => CountryCodeValidator.NormalizeAndValidate(new[] { badCode }));

            Assert.Equal(badCode, ex.ProvidedCode);
        }

        [Fact]
        public void NormalizeAndValidate_NullEntryInList_Throws()
        {
            var codes = new List<string> { "US", null! };

            Assert.Throws<InvalidCountryCodeException>(
                () => CountryCodeValidator.NormalizeAndValidate(codes));
        }

        [Fact]
        public void NormalizeAndValidate_OneBadCodeAmongGoodOnes_ThrowsAndStopsAtFirstBad()
        {
            //arrange: "GR" valid, "XX1" invalid -> whole call must throw, not silently drop it
            var codes = new[] { "GR", "XX1" };

            Assert.Throws<InvalidCountryCodeException>(
                () => CountryCodeValidator.NormalizeAndValidate(codes));
        }
    }
}
