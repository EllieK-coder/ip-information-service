namespace IPService.Domain.Entities
{
    public class Country
    {
        public int Id { get; }
        public string Name { get;  }
        public string TwoLetterCode { get; }
        public string ThreeLetterCode { get; }

        public Country(int id, string name, string twoLetterCode, string threeLetterCode)
        {
            Id = id;
            Name = name;
            TwoLetterCode = twoLetterCode;
            ThreeLetterCode = threeLetterCode;
        }

        public Country() { }
    }
}