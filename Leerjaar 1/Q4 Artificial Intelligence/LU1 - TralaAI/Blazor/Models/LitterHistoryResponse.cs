namespace Blazor.Models;

public class LitterHistoryResponse
{
    public required LitterTypeAmount AmountPerLocation { get; set; }
    public required List<Litter> History { get; set; }
    public required LitterHistoryMetadata Metadata { get; set; }
}

public class LitterHistoryMetadata
{
    public required DateTime RetrievedAt { get; set; }
    public required int RecordCount { get; set; }
}