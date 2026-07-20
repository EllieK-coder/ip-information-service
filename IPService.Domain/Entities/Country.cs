namespace IPService.Domain.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? TwoLetterCode { get; set; }
        public string? ThreeLetterCode { get; set; }
    }
}