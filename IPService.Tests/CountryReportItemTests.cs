using IPService.Application.Queries.Report;

namespace IPService.Tests
{
    // Step 1 has no branching logic (just an interface + a read-model record),
    // so this only pins down the shape the rest of Task 3 depends on:
    // positional properties assign correctly and value-equality works.
    public class CountryReportItemTests
    {
        [Fact]
        public void Constructor_AssignsAllPropertiesInOrder()
        {
            //arrange + act
            var lastUpdated = new DateTime(2026, 1, 5);
            var item = new CountryReportItem("GR", "Greece", 5, lastUpdated);

            //assert
            Assert.Equal("GR", item.TwoLetterCode);
            Assert.Equal("Greece", item.CountryName);
            Assert.Equal(5, item.AddressesCount);
            Assert.Equal(lastUpdated, item.LastAddressUpdated);
        }

        [Fact]
        public void Equality_SameValues_AreEqual()//record -> value equality, not reference
        {
            var lastUpdated = new DateTime(2026, 1, 5);
            var a = new CountryReportItem("GR", "Greece", 5, lastUpdated);
            var b = new CountryReportItem("GR", "Greece", 5, lastUpdated);

            Assert.Equal(a, b);
        }

        [Fact]
        public void Equality_DifferentAddressesCount_AreNotEqual()
        {
            var lastUpdated = new DateTime(2026, 1, 5);
            var a = new CountryReportItem("GR", "Greece", 5, lastUpdated);
            var b = new CountryReportItem("GR", "Greece", 6, lastUpdated);

            Assert.NotEqual(a, b);
        }
    }
}
