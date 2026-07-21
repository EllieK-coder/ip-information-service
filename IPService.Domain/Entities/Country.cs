namespace IPService.Domain.Entities
{
    public class Country
    {
        public string Name { get; } = string.Empty;
        public string TwoLetterCode { get; } = string.Empty;
        public string ThreeLetterCode { get; } = string.Empty;

        public Country(string name, string twoLetterCode, string threeLetterCode){
            Name = name;
            TwoLetterCode = twoLetterCode;
            ThreeLetterCode = threeLetterCode;
        }

        public Country() { }
    }
}