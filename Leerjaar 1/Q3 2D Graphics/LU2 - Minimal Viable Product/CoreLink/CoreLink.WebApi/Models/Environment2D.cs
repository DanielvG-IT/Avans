namespace CoreLink.WebApi.Models
{
    public class Environment2D
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required int MaxHeight { get; set; }
        public required int MaxLength { get; set; }
    }
}
